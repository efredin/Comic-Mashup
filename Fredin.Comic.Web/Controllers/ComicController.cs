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

		public ViewResult Create()
		{
			List<ClientTemplate> templates = this.EntityContext.ListTemplates()
				.ToList()
				.Select(t => new ClientTemplate(t))
				.ToList();

			List<ClientTextBubbleDirection> bubbles = this.EntityContext.ListTextBubbles()
				.SelectMany(b => b.TextBubbleDirections)
				.ToList()
				.Select(d => new ClientTextBubbleDirection(d))
				.ToList();

			return this.View(new ViewCreate(templates, this.GetEffects(), bubbles));
		}

		public ViewResult CreateWizard()
		{
			List<ClientTemplate> templates = this.EntityContext.ListTemplates()
				.ToList()
				.Select(t => new ClientTemplate(t))
				.ToList();

			return this.View(new ViewCreate(templates, this.GetEffects()));
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
		public JsonResult QueueRender(ComicEffectType effect, string photoSource, long templateId, List<RenderFrame> frames)
		{
			// Generate render task
			RenderTask task = new RenderTask();
			task.TaskId = Guid.NewGuid();
			task.Status = TaskStatus.Queued;
			task.CompletedOperations = 1;
			task.TotalOperations = frames.Count + 1; // Operations = 1 per frame + save
			task.Effect = effect;
			task.PhotoSource = photoSource;
			task.TemplateId = templateId;
			task.Frames = frames;
			task.OwnerUid = this.ActiveUser.Uid;
			task.FacebookToken = this.Facebook.AccessToken;

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