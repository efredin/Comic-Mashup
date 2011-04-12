using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Render;

namespace Fredin.Comic.Web.Models
{
	public class ClientProfileTask
	{
		public Guid TaskId { get; set; }
		public TaskStatus Status { get; set; }
		public long OwnerUid { get; set; }
		public ComicEffectType Effect { get; set; }
		public string RenderUrl { get; set; }
		public int Intensity { get; set; }

		public ClientProfileTask(ProfileTask source)
		{
			this.TaskId = source.TaskId;
			this.Status = source.Status;
			this.OwnerUid = source.OwnerUid;
			this.Effect = source.Effect;
			this.RenderUrl = ComicUrlHelper.GetProfileRenderUrl(source.StorageKey);
			this.Intensity = source.Intensity;
		}
	}
}