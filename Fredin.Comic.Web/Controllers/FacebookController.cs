﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fredin.Comic.Web.Models;
using Fredin.Comic.Render;
using Facebook.Web.Mvc;
using Microsoft.WindowsAzure.StorageClient;
using Fredin.Comic.Config;
using Fredin.Util;
using System.Xml.Serialization;
using System.IO;

namespace Fredin.Comic.Web.Controllers
{
    public class FacebookController : ComicControllerBase
    {
		[AcceptVerbs("POST", "GET")]
        public ActionResult Index()
        {
            return View(new ViewFacebook(this.GetEffects()));
        }

		[FacebookAuthorize(LoginUrl = "~/Facebook/Login")]
		[HttpPost]
		public ActionResult QueueRender(ComicEffectType effect, int intensity)
		{
			// Generate render task
			ProfileTask task = new ProfileTask();
			task.TaskId = Guid.NewGuid();
			task.Status = TaskStatus.Queued;
			task.Effect = effect;
			task.Intensity = intensity;
			task.OwnerUid = this.ActiveUser.Uid;
			task.FacebookToken = this.Facebook.AccessToken;
			task.StorageKey = this.ActiveUser.Uid.ToString();

			// Queue the task up using Azure Queue services.  Store full task information using Blob storage.  Only the task id is queued.
			// This is done because we need public visibility on render tasks before, during and after the task completes.

			// Save task to storage
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.ProfileTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(task.TaskId.ToString());
			blob.UploadText(task.ToXml());

			// Queue up task
			CloudQueue queue = this.QueueClient.GetQueueReference(ComicConfigSectionGroup.Queue.ProfileTaskQueue);
			CloudQueueMessage message = new CloudQueueMessage(task.TaskId.ToString());
			queue.AddMessage(message, TimeSpan.FromMinutes(5));

			return this.View("Render", new ClientProfileTask(task));
		}

		[FacebookAuthorize(LoginUrl = "~/Facebook/Login")]
		[HandleError(View = "JsonError")]
		public ActionResult RenderProgress(string taskId)
		{
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.ProfileTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(taskId);

			XmlSerializer serializer = new XmlSerializer(typeof(ProfileTask));
			using (MemoryStream stream = new MemoryStream())
			{
				blob.DownloadToStream(stream);
				stream.Seek(0, SeekOrigin.Begin);
				ProfileTask task = (ProfileTask)serializer.Deserialize(stream);

				if (task.OwnerUid != this.ActiveUser.Uid)
				{
					throw new Exception("Unknown task");
				}

				ClientProfileTask clientTask = new ClientProfileTask(task);
				return this.Json(clientTask, JsonRequestBehavior.AllowGet);
			}

			throw new Exception("Unknown task");
		}

		public ViewResult Login()
		{
			return this.View();
		}

		protected List<ClientEffect> GetEffects()
		{
			List<ClientEffect> effects = new List<ClientEffect>();
			effects.Add(new ClientEffect("Comic", ComicUrlHelper.GetImageUrl("Effect/comic.png"), ComicEffectType.Comic));
			effects.Add(new ClientEffect("Color Sketch", ComicUrlHelper.GetImageUrl("Effect/color-sketch.png"), ComicEffectType.ColorSketch));
			effects.Add(new ClientEffect("Pencil Sketch", ComicUrlHelper.GetImageUrl("Effect/pencil-sketch.png"), ComicEffectType.PencilSketch));
			return effects;
		}
    }
}