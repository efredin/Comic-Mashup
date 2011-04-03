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
				this.EntityContext.TryAttach(this.ActiveUser);

				Data.Comic comic = this.EntityContext.TryGetComic(comicId, this.ActiveUser);
				if (comic == null || (comic.IsPrivate && !this.IsFriendOrSelf(comic.Author)))
				{
					throw new Exception("Unable to find the requested comic.");
				}

				// Track read
				ComicRead read = this.EntityContext.TryGetComicRead(comic, this.ActiveUser);
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

				result = (ActionResult)this.View(new ViewRead(comic, read));
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
				Data.Comic comic = this.EntityContext.TryGetComic(comicId, this.ActiveUser);
				if (comic == null || !comic.IsPublished || !this.IsFriendOrSelf(comic.Author))
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

		#region [Wizard]

		public ViewResult CreateWizard()
		{
			return this.View(new ViewCreateWizard(this.GetTemplates(), this.GetEffects()));
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult RenderWizard(ComicEffectType effect, string photoSource, long templateId, List<RenderFrame> frames)
		{
			// Generate render task
			RenderTask task = new RenderTask();
			task.TaskId = Guid.NewGuid();
			task.Status = TaskStatus.Pending;
			task.CompletedOperations = 0;
			task.TotalOperations = frames.Count + 1; // Operations = 1 per frame + save
			task.Effect = effect;
			task.PhotoSource = photoSource;
			task.TemplateId = templateId;
			task.Frames = frames;
			task.FacebookToken = this.Facebook.AccessToken;

			// Save task to storage and it will be processed by RenderWorker
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Storage.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Storage.RenderTaskDirectory);
			CloudBlob progressBlob = directory.GetBlobReference(task.TaskId.ToString());
			progressBlob.UploadText(new JavaScriptSerializer().Serialize(task));

			return this.Json(new ClientRenderTask(task), JsonRequestBehavior.DenyGet);
		}

		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[HandleError(View = "JsonError")]
		public JsonResult PublishWizard(long comicId, string title, string description, bool isPrivate)
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

		#region [Template]

		protected List<ClientTemplate> GetTemplates()
		{
			List<ClientTemplate> templates = new List<ClientTemplate>();
			foreach (Template t in this.EntityContext.ListTemplates())
			{
				templates.Add(new ClientTemplate(t));
			}
			return templates;
		}

		#endregion

		#region [Effect]

		protected List<ClientEffect> GetEffects()
		{
			List<ClientEffect> effects = new List<ClientEffect>();
			effects.Add(new ClientEffect("None", ComicUrlHelper.GetImageUrl("Effect/none.png"), ComicEffectType.None));
			//effects.Add(new ClientEffect("Comic", ComicUrlHelper.GetImageUrl("Effect/comic.png"), ComicEffectType.Comic));
			effects.Add(new ClientEffect("Color Sketch", ComicUrlHelper.GetImageUrl("Effect/color-sketch.png"), ComicEffectType.ColorSketch));
			effects.Add(new ClientEffect("Pencil Sketch", ComicUrlHelper.GetImageUrl("Effect/pencil-sketch.png"), ComicEffectType.PencilSketch));
			return effects;
		}

		#endregion
	}
}