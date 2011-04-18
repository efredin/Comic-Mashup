using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewFacebookRender
	{
		public ClientProfileTask Task { get; set; }
		public bool AutoShareFeed { get; set; }

		public ViewFacebookRender(ClientProfileTask task, bool autoShareFeed)
		{
			this.Task = task;
			this.AutoShareFeed = autoShareFeed;
		}
	}
}