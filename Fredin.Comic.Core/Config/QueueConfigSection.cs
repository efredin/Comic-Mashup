using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class QueueConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("renderTaskQueue", IsRequired = true)]
		public string RenderTaskQueue
		{
			get { return (string)this["renderTaskQueue"]; }
			set { this["renderTaskQueue"] = value; }
		}

		[ConfigurationProperty("profileTaskQueue", IsRequired = true)]
		public string ProfileTaskQueue
		{
			get { return (string)this["profileTaskQueue"]; }
			set { this["profileTaskQueue"] = value; }
		}
	}
}
