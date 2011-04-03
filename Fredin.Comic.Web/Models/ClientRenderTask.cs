using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Render;

namespace Fredin.Comic.Web.Models
{
	public class ClientRenderTask
	{
		public Guid TaskId { get; set; }
		public TaskStatus Status { get; set; }
		public int CompletedOperations { get; set; }
		public int TotalOperations { get; set; }
		public long OwnerUid { get; set; }

		public ComicEffectType Effect { get; set; }
		public string PhotoSource { get; set; }
		public long TemplateId { get; set; }
		public List<RenderFrame> Frames { get; set; }

		public ClientRenderTask(RenderTask source)
		{
			this.TaskId = source.TaskId;
			this.Status = source.Status;
			this.CompletedOperations = source.CompletedOperations;
			this.TotalOperations = source.TotalOperations;
			this.OwnerUid = source.OwnerUid;
			this.Effect = source.Effect;
			this.PhotoSource = source.PhotoSource;
			this.TemplateId = source.TemplateId;
			this.Frames = source.Frames;
		}
	}
}