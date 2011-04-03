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

namespace Fredin.Comic.RenderWorker
{
	public sealed class RenderTaskManager
	{
		#region [Singelton]

		private static RenderTaskManager _instance;
		public static RenderTaskManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RenderTaskManager();
				}
				return _instance;
			}
		}
		private RenderTaskManager()
		{
			this.ExecuteMutex = new Mutex();
			this.TaskMutex = new Mutex();

			this.Log = LogManager.GetLogger(typeof(RenderTaskManager));
			this.InitEntityContext();
			this.InitStorage();
		}

		#endregion

		#region [Azure Storage]

		private CloudStorageAccount StorageAccount { get; set; }
		private CloudBlobClient BlobClient { get; set; }

		private void InitStorage()
		{
#if DEBUG
			this.StorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
#else
			this.StorageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["ComicStorage"].ConnectionString);
#endif

			this.BlobClient = this.StorageAccount.CreateCloudBlobClient();
			this.BlobClient.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.Zero);
		}

		#endregion

		#region [Entity]

		private string ConnectionString { get; set; }

		private ComicModelContext EntityContext { get; set; }

		private void InitEntityContext()
		{
			this.ConnectionString = ConfigurationManager.ConnectionStrings["ComicModelContext"].ConnectionString;
			this.EntityContext = new ComicModelContext(this.ConnectionString);
		}

		#endregion

		private ILog Log { get; set; }
		private bool IsExecuting { get; set; }
		private Mutex ExecuteMutex { get; set; }
		private Mutex TaskMutex { get; set; }
		private Timer ExecuteTimer { get; set; }
		private List<RenderTask> Tasks { get; set; }

		public void Start()
		{
			this.ExecuteTimer = new Timer(new TimerCallback(this.Execute), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
		}

		public void Stop()
		{
			this.ExecuteTimer.Dispose();
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
				this.LoadTasks();

				// Use thread pooling to queue up all the pending tasks
				this.TaskMutex.WaitOne();
				foreach (RenderTask task in this.Tasks)
				{
					if (task.Status == TaskStatus.Pending)
					{
						task.Status = TaskStatus.Queued;
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteTask), task);
						this.UpdateTask(task);
					}
				}
				this.TaskMutex.ReleaseMutex();

				this.CleanupTasks();

				this.ExecuteMutex.WaitOne();
				this.IsExecuting = false;
				this.ExecuteMutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Load tasks from storage and merge with existing task list
		/// </summary>
		private void LoadTasks()
		{
			this.TaskMutex.WaitOne();

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Storage.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Storage.RenderTaskDirectory);
			foreach (IListBlobItem item in directory.ListBlobs())
			{
				if (item is CloudBlob && this.Tasks.FirstOrDefault(t => t.TaskId.ToString() == item.Uri.Segments.Last()) == null)
				{
					CloudBlob blob = (CloudBlob)item;
					RenderTask task = serializer.Deserialize<RenderTask>(blob.DownloadText());
					this.Tasks.Add(task);
				}
			}

			this.TaskMutex.ReleaseMutex();
		}

		private void CleanupTasks()
		{
			this.TaskMutex.WaitOne();

			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Storage.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Storage.RenderTaskDirectory);
			foreach (IListBlobItem item in directory.ListBlobs())
			{
				if (item is CloudBlob)
				{
					CloudBlob blob = (CloudBlob)item;
					DateTime modified;
					DateTime.TryParse(blob.Metadata["Last-Modified"], out modified);
					if (modified <= DateTime.Now.AddMinutes(20))
					{
						blob.DeleteIfExists();
					}
				}
			}

			this.TaskMutex.ReleaseMutex();
		}

		private void UpdateTask(RenderTask task)
		{
			this.TaskMutex.WaitOne();
			try
			{
				CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Storage.TaskContainer);
				CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Storage.RenderTaskDirectory);
				CloudBlob progressBlob = directory.GetBlobReference(task.TaskId.ToString());
				progressBlob.UploadText(new JavaScriptSerializer().Serialize(task));
			}
			catch (Exception x)
			{
				this.Log.Error("Unable to update render progress", x);
			}
			this.TaskMutex.ReleaseMutex();
		}

		private void RemoveTask(RenderTask task)
		{
			this.TaskMutex.WaitOne();

			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Storage.TaskContainer);
			CloudBlobDirectory directory = container.GetDirectoryReference(ComicConfigSectionGroup.Storage.RenderTaskDirectory);
			CloudBlob blob = directory.GetBlobReference(task.TaskId.ToString());
			blob.DeleteIfExists();
			this.Tasks.Remove(task);

			this.TaskMutex.ReleaseMutex();
		}

		private void ExecuteTask(object state)
		{
			RenderTask task = (RenderTask)state;
			task.Status = TaskStatus.Executing;
			this.UpdateTask(task);

			Data.Comic comic = null;

			try
			{
				User user = this.EntityContext.TryGetUser(task.OwnerUid);

				FacebookApp facebook = new FacebookApp(task.FacebookToken);

				// Get template
				Template template = this.EntityContext.ListTemplates().First(t => t.TemplateId == task.TemplateId);
				List<TemplateItem> templateItems = template.TemplateItems.OrderBy(t => t.Ordinal).ToList();

				// Get text bubble - only speech for now
				TextBubble speechBubble = this.EntityContext.ListTextBubbles().First(b => b.Title == "speech");
				TextBubble bubbleShout = this.EntityContext.ListTextBubbles().First(b => b.Title == "shout");
				TextBubble squareBubble = this.EntityContext.ListTextBubbles().First(b => b.Title == "square");

				comic = new Data.Comic();
				this.EntityContext.AddToComics(comic);

				comic.Author = user;
				comic.Template = template;
				comic.CreateTime = DateTime.Now;
				comic.UpdateTime = DateTime.Now;
				comic.PublishTime = null;
				comic.FeatureTime = null;
				comic.Title = "Test";
				comic.Description = "Descriptionating";
				comic.ShareText = "Sharable";
				comic.IsPublished = false;
				comic.IsPrivate = false;
				comic.IsDeleted = false;

				// Comic generator only used to size text
				ComicGenerator generator = new ComicGenerator(template.Width, template.Height);

				// Render effect parameters
				Dictionary<string, object> parameterValues = new Dictionary<string, object>();
				parameterValues.Add("edging", 2);
				parameterValues.Add("coloring", 35);

				// Get photos for each frame
				for (int f = 0; f < task.Frames.Count; f++)
				{
					Bitmap image = null;
					string imageUrl = String.Empty;
					Point tag = Point.Empty;

					// Tagged facebook photos
					if (task.PhotoSource == "Tagged")
					{
						// List photos of the user
						Dictionary<string, object> args = new Dictionary<string, object>();
						args.Add("limit", "50");

						dynamic photoResult = facebook.Get(String.Format("/{0}/photos", task.Frames[f].Id), args);

						if (photoResult.data.Count > 0)
						{
							// Pick a random photo with only 1 tagged person
							dynamic photoData = ((IList<dynamic>)photoResult.data)
								.OrderBy(p => Guid.NewGuid())
								.FirstOrDefault(p => p.tags.data.Count <= 3);

							if (photoData != null)
							{
								imageUrl = (string)photoData.source;

								// Look for user tag location
								int id;
								dynamic tagData = ((IList<dynamic>)photoData.tags.data)
									.FirstOrDefault(t => int.TryParse(t.id, out id) && id == task.Frames[f].Id);

								if (tagData != null)
								{
									tag = new Point((int)Math.Round((double)tagData.x), (int)Math.Round((double)tagData.y));
								}
							}
						}
					}

					// Look for any photo of the user
					else// if(photoSource == "Any")
					{
						FaceRestAPI faceApi = this.CreateFaceApi(facebook);
						List<string> ids = new List<string>(new string[] { String.Format("{0}@facebook.com", task.Frames[f].Id) });
						FaceRestAPI.FaceAPI anyResult = faceApi.facebook_get(ids, null, "1", null, "random");

						if (anyResult.status == "success" && anyResult.photos.Count > 0)
						{
							FaceRestAPI.Photo p = anyResult.photos[0];
							imageUrl = p.url;
							tag = new Point((int)Math.Round(p.tags.First().mouth_center.x), (int)Math.Round(p.tags.First().mouth_center.y));
						}
					}

					// Use profile photo
					if (String.IsNullOrEmpty(imageUrl))
					{
						imageUrl = String.Format("https://graph.facebook.com/{0}/picture?access_token={1}&type=large", task.Frames[f].Id, facebook.AccessToken);
					}
					image = this.GetImage(imageUrl);

					// Find faces - overrides facebook tag locations since people tag randomly at times
					FaceRestAPI tagApi = this.CreateFaceApi(facebook);
					List<string> tagIds = new List<string>(new string[] { String.Format("{0}@facebook.com", task.Frames[f].Id) });
					List<string> urls = new List<string>(new string[] { imageUrl });
					FaceRestAPI.FaceAPI tagResult = tagApi.faces_recognize(urls, tagIds, null, null, null, null, null);

					if (tagResult.status == "success" && tagResult.photos.Count > 0 && tagResult.photos[0].tags.Count > 0)
					{
						FaceRestAPI.Tag t = tagResult.photos[0].tags.First();
						tag = new Point((int)Math.Round(t.mouth_center.x), (int)Math.Round(t.mouth_center.y));
					}

					// Text Bubbles
					ComicTextBubble comicBubble = new ComicTextBubble();
					this.EntityContext.AddToComicTextBubbles(comicBubble);
					comicBubble.Comic = comic;
					comicBubble.Text = task.Frames[f].Message;

					// Remove newlines
					comicBubble.Text = comicBubble.Text.Replace('\n', ' ');

					// Max text length
					if (comicBubble.Text.Length > 120)
					{
						string[] words = comicBubble.Text.Split(new char[] { ' ' });
						comicBubble.Text = String.Empty;
						for (int w = 0; w < words.Length && comicBubble.Text.Length < 120; w++)
						{
							comicBubble.Text += words[w] + " ";
						}
						comicBubble.Text += "...";
					}

					// Shouting / excited?
					TextBubble bubble = speechBubble;
					if (comicBubble.Text.Contains('!') || Regex.Matches(comicBubble.Text, "[A-Z]").Count > comicBubble.Text.Length / 4)
					{
						bubble = bubbleShout;
					}

					ComicGenerator.ImageAlign imageAlignment = ComicGenerator.ImageAlign.Center;
					if (tag != Point.Empty && tag.Y <= image.Height / 3)
					{
						imageAlignment = ComicGenerator.ImageAlign.Top;
					}

					this.PositionFrameBubble(comicBubble, image, generator, bubble, squareBubble, templateItems[f], tag, imageAlignment);

					// Resize to fit frame
					image = ComicGenerator.FitImage(new Size(templateItems[f].Width, templateItems[f].Height), image);

					// Apply render effect
					if (task.Effect != ComicEffectType.None)
					{
						RenderHelper effectHelper = new RenderHelper(image.Size);
						ImageRenderData renderResult = effectHelper.RenderEffect(image, task.Effect, parameterValues);
						image = new Bitmap(renderResult.RenderStream);
					}

					// Read raw photo into memory
					MemoryStream imageStream = new MemoryStream();
					image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
					imageStream.Seek(0, SeekOrigin.Begin);

					// Add photo as template item
					Photo photo = new Photo();
					photo.User = user;
					photo.CreateTime = DateTime.Now;
					photo.ImageData = imageStream.ToArray();
					this.EntityContext.AddToPhotos(photo);

					ComicPhoto comicPhoto = new ComicPhoto();
					comicPhoto.Comic = comic;
					comicPhoto.Photo = photo;
					comicPhoto.TemplateItem = templateItems[f];
					comicPhoto.Alignment = imageAlignment;
					comic.ComicPhotos.Add(comicPhoto);

					// Update task progress
					task.CompletedOperations++;
					this.UpdateTask(task);
				}

				this.SaveComic(comic);
			}
			catch (Exception x)
			{
				this.Log.Error("Unable to complete render task.", x);
			}
			finally
			{
				task.Status = TaskStatus.Complete;
				this.UpdateTask(task);
			}
		}

		private void PositionFrameBubble(ComicTextBubble comicBubble, Bitmap image, ComicGenerator generator, TextBubble bubble, TextBubble squareBubble, TemplateItem templateItem, Point tag, ComicGenerator.ImageAlign alignment)
		{
			Rectangle textArea = new Rectangle();

			// Measure text
			Size rawTextSize = generator.MeasureText(comicBubble.Text).ToSize();
			int charWidth = rawTextSize.Width / comicBubble.Text.Length;

			// Calc text bubble dimensions based on bubble x:y ratio
			Size textSize = this.CalculateTextSize(comicBubble.Text, rawTextSize, bubble);
			textSize.Width = Math.Min(textSize.Width, templateItem.Width); // Don't exceede template item area

			// Final measure
			textSize = generator.MeasureText(comicBubble.Text, textSize.Width).ToSize();
			textSize.Width += 6; // Measure error ?

			// Photos are anchored at the top. Sides and bottom are cropped to fit
			Size templateSize = new Size(templateItem.Width, templateItem.Height);
			Rectangle templateArea = new Rectangle(templateItem.X, templateItem.Y, templateItem.Width, templateItem.Height);

			if (tag != Point.Empty)
			{
				Size fitSize = ComicGenerator.GetFitImageSize(new Size(image.Width, image.Height), templateSize);
				Rectangle cropArea = ComicGenerator.GetCropImageSize(fitSize, templateSize, alignment);

				// Get fitSize face location
				int tagX = fitSize.Width * tag.X / 100 - cropArea.X + templateItem.X;
				int tagY = fitSize.Height * tag.Y / 100 - cropArea.Y + templateItem.Y;
				int bufferX = textSize.Width / 4 + 10;
				int bufferY = textSize.Height / 2 + 30;

				// Attempt to position around the tag
				Rectangle t = new Rectangle(tagX - (textSize.Width / 2), tagY - textSize.Height - bufferY, textSize.Width, textSize.Height);
				Rectangle tl = new Rectangle(tagX - textSize.Width - bufferX, tagY - textSize.Height - bufferY, textSize.Width, textSize.Height);
				Rectangle tr = new Rectangle(tagX + bufferX, tagY - textSize.Height - bufferY, textSize.Width, textSize.Height);
				Rectangle bl = new Rectangle(tagX - textSize.Width - bufferX, tagY + bufferY, textSize.Width, textSize.Height);
				Rectangle br = new Rectangle(tagX + bufferX, tagY + bufferY, textSize.Width, textSize.Height);
				Rectangle b = new Rectangle(tagX - (textSize.Width / 2), tagY + bufferY, textSize.Width, textSize.Height);

				// Allow for text to appear slightly outside the frame, but not outside the bounds of the comic
				Rectangle bubbleArea = new Rectangle(Math.Max(0, templateArea.X - 25), Math.Max(0, templateArea.Y - 25), templateArea.Width + 25, templateArea.Height);
				if (bubbleArea.X + bubbleArea.Width > templateItem.Template.Width) bubbleArea.Width -= bubbleArea.X + bubbleArea.Width - templateItem.Template.Width;
				if (bubbleArea.Y + bubbleArea.Height > templateItem.Template.Height) bubbleArea.Height -= bubbleArea.Y + bubbleArea.Height - templateItem.Template.Height;

				if (bubbleArea.Contains(tr) && !comicBubble.Comic.ComicTextBubbles.Any(c => c.Position.IntersectsWith(tr)))
				{
					textArea = tr;
					comicBubble.TextBubbleDirection = bubble.TextBubbleDirections.First(d => d.Direction == "bl");
					comicBubble.Position = tr;
				}
				else if (bubbleArea.Contains(br) && !comicBubble.Comic.ComicTextBubbles.Any(c => c.Position.IntersectsWith(br)))
				{
					textArea = br;
					comicBubble.TextBubbleDirection = bubble.TextBubbleDirections.First(d => d.Direction == "tl");
					comicBubble.Position = br;
				}
				else if (bubbleArea.Contains(tl) && !comicBubble.Comic.ComicTextBubbles.Any(c => c.Position.IntersectsWith(tl)))
				{
					textArea = tl;
					comicBubble.TextBubbleDirection = bubble.TextBubbleDirections.First(d => d.Direction == "br");
					comicBubble.Position = tl;
				}
				else if (bubbleArea.Contains(bl) && !comicBubble.Comic.ComicTextBubbles.Any(c => c.Position.IntersectsWith(bl)))
				{
					textArea = bl;
					comicBubble.TextBubbleDirection = bubble.TextBubbleDirections.First(d => d.Direction == "tr");
					comicBubble.Position = bl;
				}
				else if (bubbleArea.Contains(b) && !comicBubble.Comic.ComicTextBubbles.Any(c => c.Position.IntersectsWith(b)))
				{
					textArea = b;
					comicBubble.TextBubbleDirection = bubble.TextBubbleDirections.First(d => d.Direction == "t");
					comicBubble.Position = b;
				}
				else if (bubbleArea.Contains(t) && !comicBubble.Comic.ComicTextBubbles.Any(c => c.Position.IntersectsWith(t)))
				{
					comicBubble.TextBubbleDirection = bubble.TextBubbleDirections.First(d => d.Direction == "b");
					comicBubble.Position = t;
				}
				else
				{
					if (tag.Y <= 50)
					{
						// Position at bottom - face is at top
						comicBubble.TextBubbleDirection = squareBubble.TextBubbleDirections.First(d => d.Direction == "n");
						comicBubble.Position = new Rectangle(templateItem.X, templateItem.Y + templateItem.Height - textSize.Height, textSize.Width, textSize.Height);
					}
					else
					{
						// Position at top - face is at the bottom
						comicBubble.TextBubbleDirection = squareBubble.TextBubbleDirections.First(d => d.Direction == "n");
						comicBubble.Position = new Rectangle(templateItem.X, templateItem.Y, textSize.Width, textSize.Height);
					}
				}
			}
			else
			{
				comicBubble.TextBubbleDirection = squareBubble.TextBubbleDirections.First(d => d.Direction == "n");
				comicBubble.Position = new Rectangle(templateItem.X, templateItem.Y + templateItem.Height - textSize.Height, textSize.Width, textSize.Height);
			}
		}

		private Size CalculateTextSize(string text, Size rawTextSize, TextBubble bubble)
		{
			Size textSize = rawTextSize;
			if (rawTextSize.Width >= bubble.TextScaleX)
			{
				double area = Convert.ToDouble(rawTextSize.Width) * Convert.ToDouble(rawTextSize.Height);
				double factor = Math.Sqrt(area / (Convert.ToDouble(bubble.TextScaleX) * Convert.ToDouble(bubble.TextScaleY)));

				textSize.Width = Convert.ToInt32(Math.Round(factor * Convert.ToDouble(bubble.TextScaleX)));
				textSize.Height = Convert.ToInt32(Math.Round(factor * Convert.ToDouble(bubble.TextScaleY)));
			}
			else
			{
				textSize.Width += 40; // Short shouldn't get wrapped
			}

			return textSize;
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

		/// <summary>
		/// Creates a comic generator and loads the appropriate images and text bubbles into it.
		/// </summary>
		private ComicGenerator CreateComicGenerator(Fredin.Comic.Data.Comic comic, bool firstFrameOnly)
		{
			int firstFrameOrdinal = comic.ComicPhotos
					.Min(p => p.TemplateItem.Ordinal);

			ComicPhoto firstFrame = comic.ComicPhotos
					.First(p => p.TemplateItem.Ordinal == firstFrameOrdinal);

			// Create generator
			ComicGenerator generator = null;
			if (firstFrameOnly)
			{
				generator = new ComicGenerator(firstFrame.TemplateItem.Width, firstFrame.TemplateItem.Height);
			}
			else
			{
				generator = new ComicGenerator(comic.Template.Width, comic.Template.Height);
			}

			// Add frame photos
			foreach (ComicPhoto item in comic.ComicPhotos)
			{
				if (firstFrameOnly && firstFrameOrdinal == item.TemplateItem.Ordinal)
				{
					using (MemoryStream imageStream = new MemoryStream(item.Photo.ImageData))
					{
						Bitmap image = new Bitmap(imageStream);
						generator.AddFitImage(image, item.TemplateItem.Width, item.TemplateItem.Height, 0, 0, item.Alignment);
					}
				}
				else if (!firstFrameOnly)
				{
					using (MemoryStream imageStream = new MemoryStream(item.Photo.ImageData))
					{
						Bitmap image = new Bitmap(imageStream);
						generator.AddFitImage(image, item.TemplateItem.Width, item.TemplateItem.Height, item.TemplateItem.X, item.TemplateItem.Y, item.Alignment);
					}
				}
			}

			CloudBlobContainer blobContainer = this.BlobClient.GetContainerReference("static");
			foreach (ComicTextBubble comicBubble in comic.ComicTextBubbles)
			{
				if (!comicBubble.Position.IsEmpty)
				{
					if (!firstFrameOnly || ((comicBubble.X < firstFrame.TemplateItem.X + firstFrame.TemplateItem.Width) && comicBubble.Y < firstFrame.TemplateItem.Y + firstFrame.TemplateItem.Height))
					{
						// Get text bubble image from storage
						string bubbleKey = String.Format("Image/TextBubble/{0}-{1}.png", comicBubble.TextBubbleDirection.TextBubble.Title, comicBubble.TextBubbleDirection.Direction);
						CloudBlob bubbleBlob = blobContainer.GetBlobReference(bubbleKey);

						using (MemoryStream bubbleStream = new MemoryStream(bubbleBlob.DownloadByteArray()))
						{
							Bitmap bubbleImage = new Bitmap(bubbleStream);

							// Scale background using base : text ratio
							int width = Convert.ToInt32(Convert.ToDouble(comicBubble.TextBubbleDirection.TextBubble.BaseScaleX) / Convert.ToDouble(comicBubble.TextBubbleDirection.TextBubble.TextScaleX) * (double)comicBubble.Position.Width);
							int height = Convert.ToInt32(Convert.ToDouble(comicBubble.TextBubbleDirection.TextBubble.BaseScaleY) / Convert.ToDouble(comicBubble.TextBubbleDirection.TextBubble.TextScaleY) * (double)comicBubble.Position.Height);
							int x = comicBubble.Position.X - ((width - Convert.ToInt32(comicBubble.Position.Width)) / 2);
							int y = comicBubble.Position.Y - ((height - Convert.ToInt32(comicBubble.Position.Height)) / 2);

							generator.AddScaleImage(bubbleImage, width, height, x, y);
							generator.AddText(comicBubble.Text, new RectangleF(comicBubble.Position.X, comicBubble.Position.Y, comicBubble.Position.Width, comicBubble.Position.Height));
						}
					}
				}
			}

			return generator;
		}

		private void SaveComic(Fredin.Comic.Data.Comic comic)
		{
			// Generate comic
			ComicGenerator generator = this.CreateComicGenerator(comic, false);
			ComicGenerator frameGenerator = this.CreateComicGenerator(comic, true);
			this.EntityContext.SaveChanges();

			// Storage container for all renders
			CloudBlobContainer container = this.BlobClient.GetContainerReference(ComicConfigSectionGroup.Storage.RenderContainer);

			// Push full size comic to render storage
			using (MemoryStream comicStream = new MemoryStream())
			{
				generator.ComicImage.Save(comicStream, System.Drawing.Imaging.ImageFormat.Jpeg);
				comicStream.Seek(0, SeekOrigin.Begin);

				string comicPath = String.Format("{0}{1}", ComicConfigSectionGroup.Storage.ComicPrefix, comic.ComicId);
				CloudBlob comicBlob = container.GetBlobReference(comicPath);
				comicBlob.UploadFromStream(comicStream);
			}

			// Push thumb comic
			using (MemoryStream thumbStream = new MemoryStream())
			{
				ComicGenerator.CropImage(new Size(350, 109), generator.GenerateThumb(350), ComicGenerator.ImageAlign.Top)
					 .Save(thumbStream, System.Drawing.Imaging.ImageFormat.Jpeg);
				thumbStream.Seek(0, SeekOrigin.Begin);

				string thumbPath = String.Format("{0}{1}", ComicConfigSectionGroup.Storage.ThumbPrefix, comic.ComicId);
				CloudBlob thumbBlob = container.GetBlobReference(thumbPath);
				thumbBlob.UploadFromStream(thumbStream);
			}

			// Push 1st frame
			using (MemoryStream frameStream = new MemoryStream())
			{
				frameGenerator.ComicImage.Save(frameStream, System.Drawing.Imaging.ImageFormat.Jpeg);
				frameStream.Seek(0, SeekOrigin.Begin);

				string framePath = String.Format("{0}{1}", ComicConfigSectionGroup.Storage.FramePrefix, comic.ComicId);
				CloudBlob frameBlob = container.GetBlobReference(framePath);
				frameBlob.UploadFromStream(frameStream);
			}

			// Push 1st frame thumb
			using (MemoryStream frameThumbStream = new MemoryStream())
			{
				frameGenerator.GenerateThumb(150).Save(frameThumbStream, System.Drawing.Imaging.ImageFormat.Jpeg);
				frameThumbStream.Seek(0, SeekOrigin.Begin);

				string frameThumbPath = String.Format("{0}{1}", ComicConfigSectionGroup.Storage.FrameThumbPrefix, comic.ComicId);
				CloudBlob frameThumbBlob = container.GetBlobReference(frameThumbPath);
				frameThumbBlob.UploadFromStream(frameThumbStream);
			}
		}

		private FaceRestAPI CreateFaceApi(FacebookApp facebook)
		{
			return new FaceRestAPI(ComicConfigSectionGroup.Face.ApiKey, ComicConfigSectionGroup.Face.ApiSecret, null, false, "json", facebook.UserId.ToString(), facebook.AccessToken);
		}
	}
}