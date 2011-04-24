using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Render;

namespace Fredin.Comic.Web.Models
{
	public class ClientPhotoTask
	{
		public Guid TaskId { get; set; }
		public TaskStatus Status { get; set; }
		public long OwnerUid { get; set; }
		public ComicEffectType Effect { get; set; }
		public int Intensity { get; set; }
		public ClientPhoto Photo { get; set; }

		public ClientPhotoTask(PhotoTask source, ClientPhoto photo)
		{
			this.TaskId = source.TaskId;
			this.Status = source.Status;
			this.OwnerUid = source.OwnerUid;
			this.Effect = source.Effect;
			this.Intensity = source.Intensity;
			this.Photo = photo;
		}
	}
}