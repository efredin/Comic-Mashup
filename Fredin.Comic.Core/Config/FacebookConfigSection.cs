using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class FacebookConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("appId", IsRequired = true)]
		public string AppId
		{
			get { return (string)this["appId"]; }
			set { this["appId"] = value; }
		}

		[ConfigurationProperty("permissions", IsRequired = false, DefaultValue = "")]
		public string Permissions
		{
			get { return (string)this["permissions"]; }
			set { this["permissions"] = value; }
		}

		[ConfigurationProperty("cookieSupport", IsRequired = false, DefaultValue = true)]
		public bool CookieSupport
		{
			get { return (bool)this["cookieSupport"]; }
			set { this["cookieSupport"] = value; }
		}

		[ConfigurationProperty("useXfbml", IsRequired = false, DefaultValue = true)]
		public bool UseXfbml
		{
			get { return (bool)this["useXfbml"]; }
			set { this["useXfbml"] = value; }
		}
	}
}