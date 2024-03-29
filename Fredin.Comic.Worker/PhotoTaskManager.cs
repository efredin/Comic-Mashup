﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Facebook;
using Fredin.Comic.Config;
using Fredin.Comic.Data;
using Fredin.Comic.Render;
using Fredin.Util;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Net;
using System.IO;
using log4net;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Fredin.Comic.Worker
{
	public class PhotoTaskManager
	{
		#region [Singelton]

		private static PhotoTaskManager _instance;
		public static PhotoTaskManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PhotoTaskManager();
				}
				return _instance;
			}
		}
		private PhotoTaskManager()
		{
			this.ExecuteMutex = new Mutex();

			this.Log = LogManager.GetLogger(typeof(PhotoTaskManager));
			this.InitAzure();
		}

		#endregion

		#region [Azure Services]

		private CloudStorageAccount StorageAccount { get; set; }
		private CloudBlobClient BlobClient { get; set; }
		private CloudQueueClient QueueClient { get; set; }

		private void InitAzure()
		{
			this.StorageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["ComicStorage"].ConnectionString);

			this.BlobClient = this.StorageAccount.CreateCloudBlobClient();
			this.BlobClient.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.Zero);

			this.QueueClient = this.StorageAccount.CreateCloudQueueClient();
			this.QueueClient.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.Zero);
		}

		#endregion

		private ILog Log { get; set; }
		private bool IsExecuting { get; set; }
		private Mutex ExecuteMutex { get; set; }
		private Timer ExecuteTimer { get; set; }

		public void Start()
		{
			this.Log.Info("Starting");

			try
			{
				CloudQueue queue = this.QueueClient.GetQueueReference(ComicConfigSectionGroup.Queue.PhotoTaskQueue);
				queue.CreateIfNotExist();

				CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
				container.CreateIfNotExist();
			}
			catch (Exception x)
			{
				this.Log.Error("Error while starting", x);
			}
			this.ExecuteTimer = new Timer(new TimerCallback(this.Execute), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
		}

		public void Stop()
		{
			this.Log.Info("Stopping");

			if (this.ExecuteTimer != null)
			{
				this.ExecuteTimer.Dispose();
			}
		}

		public void Execute(object state)
		{
			this.ExecuteMutex.WaitOne();
			bool execute = false;
			if(!this.IsExecuting)
			{
				execute = true;
				this.IsExecuting = true;
			}
			this.ExecuteMutex.ReleaseMutex();

			if (execute)
			{
				try
				{
					// Pop a task off the queue
					CloudQueue queue = this.QueueClient.GetQueueReference(ComicConfigSectionGroup.Queue.PhotoTaskQueue);

					foreach(CloudQueueMessage message in queue.GetMessages(10))
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteTask), message);
						queue.DeleteMessage(message);
					}

					this.CleanupTasks();
				}
				catch(Exception x)
				{
					this.Log.Error("Failed to execute.", x);
				}

				this.ExecuteMutex.WaitOne();
				this.IsExecuting = false;
				this.ExecuteMutex.ReleaseMutex();
			}
		}

		private void CleanupTasks()
		{
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.PhotoTaskDirectory);
			foreach (IListBlobItem item in directory.ListBlobs())
			{
				if (item is CloudBlob)
				{
					CloudBlob blob = (CloudBlob)item;
					if (blob.Properties.LastModifiedUtc.AddMinutes(10) < DateTime.UtcNow)
					{
						blob.DeleteIfExists();
					}
				}
			}
		}

		private void UpdateTask(PhotoTask task)
		{
			try
			{
				CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
				CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.PhotoTaskDirectory);
				CloudBlob progressBlob = directory.GetBlobReference(task.TaskId.ToString());
				progressBlob.UploadText(task.ToXml());
			}
			catch (Exception x)
			{
				this.Log.Error("Unable to update render progress", x);
			}
		}

		private void RemoveTask(PhotoTask task)
		{
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.PhotoTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(task.TaskId.ToString());
			blob.DeleteIfExists();
		}

		private void ExecuteTask(object state)
		{
			CloudQueueMessage queueMessage = (CloudQueueMessage)state;
			this.Log.DebugFormat("Executing image task {0}", queueMessage.AsString);

			string connectionString = ConfigurationManager.ConnectionStrings["ComicModelContext"].ConnectionString;
			ComicModelContext entityContext = new ComicModelContext(connectionString);


			PhotoTask task = null;
			try
			{
				// Read task details from storage
				CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
				CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Blob.PhotoTaskDirectory);
				CloudBlob blob = directory.GetBlobReference(queueMessage.AsString);

				XmlSerializer serializer = new XmlSerializer(typeof(PhotoTask));
				using (MemoryStream stream = new MemoryStream())
				{
					blob.DownloadToStream(stream);
					stream.Seek(0, SeekOrigin.Begin);
					task = (PhotoTask)serializer.Deserialize(stream);
					task.Status = TaskStatus.Executing;
					this.UpdateTask(task);
				}

				User user = entityContext.TryGetUser(task.OwnerUid);
				FacebookClient facebook = new FacebookClient(task.FacebookToken);

				// Get image from web
				Bitmap image = this.GetImage(task.SourceUrl);

				if (task.Effect != ComicEffectType.None)
				{
					// Apply render effect
					RenderHelper effectHelper = new RenderHelper(image.Size);

					// Translate intensity to render parameters using min / max range
					Dictionary<string, object> parameters = new Dictionary<string, object>();
					foreach (RenderParameter p in effectHelper.GetRenderParameters(task.Effect))
					{
						if (task.Intensity == 0)
						{
							parameters.Add(p.Name, p.MinValue);
						}
						else if (task.Intensity == 1)
						{
							parameters.Add(p.Name, p.DefaultValue);
						}
						else if (task.Intensity == 2)
						{
							parameters.Add(p.Name, p.MaxValue);
						}
					}

					ImageRenderData renderResult = effectHelper.RenderEffect(image, task.Effect, parameters);
					image = new Bitmap(renderResult.RenderStream);
				}

				// Read raw photo into memory
				MemoryStream imageStream = new MemoryStream();
				image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
				imageStream.Seek(0, SeekOrigin.Begin);

				// Save to storage
				Photo photo = new Photo();
				photo.CreateTime = DateTime.Now;
				photo.User = user;
				photo.StorageKey = Guid.NewGuid().ToString();
				photo.Width = image.Width;
				photo.Height = image.Height;
				entityContext.AddToPhotos(photo);
				entityContext.SaveChanges();

				CloudBlobContainer imageContainer = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Blob.RenderContainer);
				CloudBlobDirectory imageDirectory = imageContainer.GetDirectoryReference(ComicConfigSectionGroup.Blob.PhotoDirectory);
				CloudBlob imageBlob = imageDirectory.GetBlobReference(photo.StorageKey);
				imageBlob.Properties.ContentType = "image/jpeg";
				imageBlob.UploadFromStream(imageStream);

				// Mark task as completed
				task.PhotoId = photo.PhotoId;
				task.Status = TaskStatus.Complete;
				this.UpdateTask(task);
				this.Log.DebugFormat("Completed image task {0}", task.TaskId);
			}
			catch (Exception x)
			{
				this.Log.Error("Unable to complete image task.", x);

				if (task != null)
				{
					task.Status = TaskStatus.Failed;
					this.UpdateTask(task);
				}
			}
		}

		private Bitmap GetImage(string url)
		{
			Bitmap image = null;
			try
			{
				HttpWebRequest bitmapRequest = (HttpWebRequest)HttpWebRequest.Create(url);
				using (HttpWebResponse bitmapResponse = (HttpWebResponse)bitmapRequest.GetResponse())
				{
					image = new Bitmap(bitmapResponse.GetResponseStream());
				}
			}
			catch (Exception x)
			{
				this.Log.Error(String.Format("Unable to load image {0}", url), x);
			}
			return image;
		}
	}
}
