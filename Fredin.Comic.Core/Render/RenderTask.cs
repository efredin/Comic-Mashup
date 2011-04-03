using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Render
{
	[Serializable()]
	public class RenderTask : Task
	{
		public int CompletedOperations { get; set; }
		public int TotalOperations { get; set; }
		public long OwnerUid { get; set; }
		public string FacebookToken { get; set; }

		public ComicEffectType Effect { get; set; }
		public string PhotoSource { get; set; }
		public long TemplateId { get; set; }
		public List<RenderFrame> Frames { get; set; }
	}
}