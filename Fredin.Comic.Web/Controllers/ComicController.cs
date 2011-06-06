using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Script.Serialization;

using Facebook;

using Fredin.Comic.Render;
using Fredin.Comic.Web.Models;
using Fredin.Comic.Config;
using Fredin.Util;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Facebook.Web.Mvc;
using Fredin.Comic.Data;
using System.Xml.Serialization;

namespace Fredin.Comic.Web.Controllers
{
	[HandleError]
	[JsonAction]
	public class ComicController : ComicControllerBase
	{
		#region [Read]

		public ActionResult Read(long comicId, string title)
		{
			ActionResult result;

			try
			{
				Data.Comic comic = null;

				// Facebook shared comics
				if (this.ActiveUser == null && this.HttpContext.Request.UrlReferrer != null && this.HttpContext.Request.UrlReferrer.Host.Contains("facebook.com"))
				{
					comic = this.EntityContext.TryGetComic(comicId, null, true);
					if (comic != null)
					{
						this.LoginGuestUser(comic.Author);
					}
				}
				// Guest user logged in
				else if (this.GuestUser != null)
				{
					comic = this.EntityContext.TryGetComic(comicId, this.GuestUser);
				}
				else
				{
					// All other entry points
					this.EntityContext.TryAttach(this.ActiveUser);
					comic = this.EntityContext.TryGetComic(comicId, this.ActiveUser, this.Friends);
				}

				if (comic == null)
				{
					result = this.View("ReadNotFound");
				}
				else
				{
					// Track read
					ComicRead read = this.EntityContext.TryGetComicRead(comic, this.ActiveUser);
					try // Having an entity issue.. trying to track it down with this.
					{
						if (read == null)
						{
							read = new ComicRead();
							read.Comic = comic;
							read.Reader = this.ActiveUser;
							read.ReadTime = DateTime.Now;

							if (this.ActiveUser != null)
							{
								this.EntityContext.AddToComicRead(read);
								this.EntityContext.SaveChanges();
							}
						}
					}
					catch (Exception x)
					{
						this.Log.Error("Reader error", x);
					}

					// Load tags
					comic.ComicTags.Load();
					List<ClientComicTag> tags = comic.ComicTags.Select(t => new ClientComicTag(t)).ToList();

					result = (ActionResult)this.View(new ViewRead(new ClientComic(comic), new ClientComicRead(read), tags));
				}
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
			}

			return result;
		}

		[HandleError]
		public RedirectResult Random()
		{
			RedirectResult result;
			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);

				Data.Comic comic = this.EntityContext.TryGetRandomComic(this.ActiveUser, this.Friends);
				if (comic == null || (comic.IsPrivate && !this.IsFriendOrSelf(comic.Author)))
				{
					throw new Exception("Unable to find the requested comic.");
				}
				result = this.Redirect(ComicUrlHelper.GetReadUrl(comic));
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
			}
			return result;
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		public JsonResult ReaderFunny(long comicId)
		{
			ComicRead read = this.GetReader(comicId);
			read.IsFunny = !read.IsFunny;
			this.EntityContext.SaveChanges();

			return this.Json(new { result = "ok" });
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		public JsonResult ReaderSmart(long comicId)
		{
			ComicRead read = this.GetReader(comicId);
			read.IsSmart = !read.IsSmart;
			this.EntityContext.SaveChanges();

			return this.Json(new { result = "ok" });
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		public JsonResult ReaderRandom(long comicId)
		{
			ComicRead read = this.GetReader(comicId);
			read.IsRandom = !read.IsRandom;
			this.EntityContext.SaveChanges();

			return this.Json(new { result = "ok" });
		}

		protected ComicRead GetReader(long comicId)
		{
			ComicRead read = null;
			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);
				Data.Comic comic = this.EntityContext.TryGetComic(comicId, this.ActiveUser, this.Friends);
				if (comic == null || !comic.IsPublished)
				{ 
					throw new Exception("Unable to find the requested comic.");
				}

				read = this.EntityContext.TryGetComicRead(comic, this.ActiveUser);

				if (read == null)
				{
					read = new ComicRead();
					read.Comic = comic;
					read.Reader = this.ActiveUser;
					read.ReadTime = DateTime.Now;
					this.EntityContext.AddToComicRead(read);
				}
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
			}

			return read;
		}

		#endregion

		[HttpGet]
		public ActionResult Delete(long? id)
		{
			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);

				Data.Comic comic = this.EntityContext.TryGetAuthoredComic(id.Value, this.ActiveUser);
				if (comic == null)
				{
					throw new Exception("Unable to find the requested comic.");
				}

				comic.IsDeleted = true;
				this.EntityContext.SaveChanges();
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
			}

			return this.View();
		}

		//[HttpGet]
		//[FacebookJsonAuthorize]
		//[HandleError(View = "~/Shared/JsonError.aspx")]
		//public JsonResult Privacy(long comicId, bool isPrivate)
		//{
		//    bool isPrivate;
		//}

		#region [Create]

		//public ViewResult Create()
		//{
		//    return this.Create(null);
		//}

		public ViewResult Create(long? id)
		{
			ClientComic comic = null;
			
			List<ClientTemplate> templates = this.EntityContext.ListTemplates()
				.ToList()
				.Select(t => new ClientTemplate(t))
				.ToList();

			foreach (ClientTemplate t in templates)
			{
				int width = Math.Min(t.Width, 734);
				double factor = Convert.ToDouble(width) / Convert.ToDouble(t.Width);
				t.Width = width;
				t.Height = Convert.ToInt32(Convert.ToDouble(t.Height) * factor);
				foreach (ClientTemplateItem i in t.TemplateItems)
				{
					i.Width = Convert.ToInt32(Convert.ToDouble(i.Width) * factor);
					i.Height = Convert.ToInt32(Convert.ToDouble(i.Height) * factor);
					i.X = Convert.ToInt32(Convert.ToDouble(i.X) * factor);
					i.Y = Convert.ToInt32(Convert.ToDouble(i.Y) * factor);
				}
			}

			if (id.HasValue)
			{
				Data.Comic c = this.EntityContext.TryGetComic(id.Value, this.ActiveUser) ?? this.EntityContext.TryGetUnpublishedComic(id.Value, this.ActiveUser);
				if (c != null)
				{
					c.ComicTextBubbles.Load();
					c.ComicPhotos.Load();
					comic = new ClientComic(c);

					// inject newlines into text
					int maxWidth = templates.SelectMany(t => t.TemplateItems).Select(t => t.Width).Min();
					Font measureFont = new Font(ComicGenerator.ComicFont, 7, FontStyle.Regular, GraphicsUnit.Point);
					ComicGenerator generator = new ComicGenerator(templates.Min(t => t.Width), templates.Min(t => t.Height));
					foreach(ClientComicTextBubble b in comic.Bubbles)
					{
						Size textSize = generator.MeasureText(b.Text, maxWidth, measureFont).ToSize();
						int lines = textSize.Height / measureFont.Height;
						int cutWidth = b.Text.Length / lines - 4;
						for (int l = 1; l < lines; l++)
						{
							int cut = cutWidth * l;
							b.Text = b.Text.ReplaceAt(b.Text.IndexOf(' ', cut), '\n');
						}
					}
				}
			}


			List<ClientTextBubbleDirection> bubbles = this.EntityContext.ListTextBubbles()
				.SelectMany(b => b.TextBubbleDirections)
				.ToList()
				.Select(d => new ClientTextBubbleDirection(d))
				.ToList();

			return this.View(new ViewCreate(comic, templates, this.GetEffects(), bubbles));
		}

		public ViewResult CreateWizard()
		{
			List<ClientTemplate> templates = this.EntityContext.ListTemplates()
				.ToList()
				.Select(t => new ClientTemplate(t))
				.ToList();

			return this.View(new ViewCreate(null, templates, this.GetEffects()));
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult QueuePhotoRender(ComicEffectType effect, string photoSource, int intensity)
		{
			// Generate render task
			PhotoTask task = new PhotoTask();
			task.TaskId = Guid.NewGuid();
			task.Status = TaskStatus.Queued;
			task.Effect = effect;
			task.Intensity = intensity;
			task.FacebookToken = this.Facebook.AccessToken;
			task.SourceUrl = photoSource;
			task.OwnerUid = this.ActiveUser.Uid;

			// Queue the task up using Azure Queue services.  Store full task information using Blob storage.  Only the task id is queued.
			// This is done because we need public visibility on render tasks before, during and after the task completes.

			// Save task to storage
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.PhotoTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(task.TaskId.ToString());
			blob.UploadText(task.ToXml());

			// Queue up task
			CloudQueue queue = this.QueueClient.GetQueueReference(ComicConfigSectionGroup.Queue.PhotoTaskQueue);
			CloudQueueMessage message = new CloudQueueMessage(task.TaskId.ToString());
			queue.AddMessage(message, TimeSpan.FromMinutes(5));

			return this.Json(new ClientPhotoTask(task, null), JsonRequestBehavior.DenyGet);
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult PhotoRenderProgress(string taskId)
		{
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.PhotoTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(taskId);

			XmlSerializer serializer = new XmlSerializer(typeof(PhotoTask));
			using (MemoryStream stream = new MemoryStream())
			{
				blob.DownloadToStream(stream);
				stream.Seek(0, SeekOrigin.Begin);
				PhotoTask task = (PhotoTask)serializer.Deserialize(stream);

				if (task.OwnerUid != this.ActiveUser.Uid)
				{
					throw new Exception("Unknown task");
				}

				ClientPhoto clientPhoto = null;
				if (task.PhotoId.HasValue)
				{
					// Load photo from database
					Photo photo = this.EntityContext.TryGetPhoto(task.PhotoId.Value);
					if (photo != null)
					{
						clientPhoto = new ClientPhoto(photo);
					}
				}

				ClientPhotoTask clientTask = new ClientPhotoTask(task, clientPhoto);
				return this.Json(clientTask, JsonRequestBehavior.AllowGet);
			}

			throw new Exception("Unknown task");
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult QueueRenderAdvanced(long? remixComicId, long templateId, List<RenderFrame> frames, List<RenderBubble> bubbles)
		{
			// Generate advanced render task
			RenderTask task = new RenderTask();
			task.TaskId = Guid.NewGuid();
			task.Status = TaskStatus.Queued;
			task.CompletedOperations = 1;

			task.TotalOperations = 2; // Operations = frames + bubbles + save + queue
			if (frames != null) task.TotalOperations += frames.Count;

			task.RemixComicId = remixComicId;
			task.Effect = ComicEffectType.None;
			task.PhotoSource = "Internal";
			task.TemplateId = templateId;
			task.Frames = frames;
			task.Bubbles = bubbles;
			task.OwnerUid = this.ActiveUser.Uid;
			task.FacebookToken = this.Facebook.AccessToken;

			return this.QueueRender(task);
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult QueueRenderWizard(ComicEffectType effect, string photoSource, long templateId, List<RenderFrame> frames)
		{
			// Generate wizard render task
			RenderTask task = new RenderTask();
			task.TaskId = Guid.NewGuid();
			task.Status = TaskStatus.Queued;
			task.CompletedOperations = 1;
			task.TotalOperations = frames.Count + 2; // Operations = 1 per frame + save + queue
			task.Effect = effect;
			task.PhotoSource = photoSource;
			task.TemplateId = templateId;
			task.Frames = frames;
			task.OwnerUid = this.ActiveUser.Uid;
			task.FacebookToken = this.Facebook.AccessToken;

			return this.QueueRender(task);
		}

		protected JsonResult QueueRender(RenderTask task)
		{
			// Queue the task up using Azure Queue services.  Store full task information using Blob storage.  Only the task id is queued.
			// This is done because we need public visibility on render tasks before, during and after the task completes.

			// Save task to storage
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.RenderTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(task.TaskId.ToString());
			blob.UploadText(task.ToXml());

			// Queue up task
			CloudQueue queue = this.QueueClient.GetQueueReference(ComicConfigSectionGroup.Queue.RenderTaskQueue);
			CloudQueueMessage message = new CloudQueueMessage(task.TaskId.ToString());
			queue.AddMessage(message, TimeSpan.FromMinutes(5));

			return this.Json(new ClientRenderTask(task), JsonRequestBehavior.DenyGet);
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult RenderProgress(string taskId)
		{
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.RenderTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(taskId);

			XmlSerializer serializer = new XmlSerializer(typeof(RenderTask));
			using(MemoryStream stream = new MemoryStream())
			{
				blob.DownloadToStream(stream);
				stream.Seek(0, SeekOrigin.Begin);
				RenderTask task = (RenderTask)serializer.Deserialize(stream);

				if (task.OwnerUid != this.ActiveUser.Uid)
				{
					throw new Exception("Unknown task");
				}

				ClientRenderTask clientTask = new ClientRenderTask(task);
				if (task.Status == TaskStatus.Complete && task.ComicId.HasValue)
				{
					// Load completed comic from database
					try
					{
						this.EntityContext.TryAttach(this.ActiveUser);
						Data.Comic comic = this.EntityContext.TryGetUnpublishedComic(task.ComicId.Value, this.ActiveUser);
						clientTask.Comic = new ClientComic(comic);
					}
					finally
					{
						this.EntityContext.TryDetach(this.ActiveUser);
					}
				}

				return this.Json(clientTask, JsonRequestBehavior.AllowGet);
			}

			throw new Exception("Unknown task");
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult Publish(long comicId, string title, string description, bool isPrivate)
		{
			Data.Comic comic = null;

			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);

				comic = this.EntityContext.TryGetUnpublishedComic(comicId, this.ActiveUser);
				if(comic == null || comic.Uid != this.ActiveUser.Uid)
				{
					throw new Exception("Could not find the requested comic.");
				}

				comic.Title = title;
				comic.Description = description;
				comic.IsPrivate = isPrivate;
				this.EntityContext.PublishComic(comic, this.ActiveUser);

				this.EntityContext.SaveChanges();
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
			}

			return this.Json(comic == null ? null : new ClientComic(comic), JsonRequestBehavior.DenyGet);
		}

		#endregion

		#region [Effect]

		protected List<ClientEffect> GetEffects()
		{
			List<ClientEffect> effects = new List<ClientEffect>();
			effects.Add(new ClientEffect("None", ComicUrlHelper.GetImageUrl("Effect/none.png"), ComicEffectType.None));
			effects.Add(new ClientEffect("Comic", ComicUrlHelper.GetImageUrl("Effect/comic.png"), ComicEffectType.Comic));
			effects.Add(new ClientEffect("Color Sketch", ComicUrlHelper.GetImageUrl("Effect/color-sketch.png"), ComicEffectType.ColorSketch));
			//effects.Add(new ClientEffect("Pencil Sketch", ComicUrlHelper.GetImageUrl("Effect/pencil-sketch.png"), ComicEffectType.PencilSketch));
			return effects;
		}

		#endregion
	}
}