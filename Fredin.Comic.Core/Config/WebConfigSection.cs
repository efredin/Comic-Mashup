using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class WebConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("staticBaseUrl", IsRequired = true)]
		public string StaticBaseUrl
		{
			get { return (string)this["staticBaseUrl"]; }
			set { this["staticBaseUrl"] = value; }
		}

		[ConfigurationProperty("renderBaseUrl", IsRequired = true)]
		public string RenderBaseUrl
		{
			get { return (string)this["renderBaseUrl"]; }
			set { this["renderBaseUrl"] = value; }
		}

		[ConfigurationProperty("trackerId", IsRequired = true)]
		public string TrackerId
		{
			get { return (string)this["trackerId"]; }
			set { this["trackerId"] = value; }
		}
	}
}