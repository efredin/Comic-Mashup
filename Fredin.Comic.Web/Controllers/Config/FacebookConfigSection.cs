using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace Fredin.Comic.Web.Config
{
	public class FacebookConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("apiKey", IsRequired = true)]
		public string ApiKey
		{
			get { return (string)this["apiKey"]; }
			set { this["apiKey"] = value; }
		}

		[ConfigurationProperty("apiSecret", IsRequired = true)]
		public string ApiSecret
		{
			get { return (string)this["apiSecret"]; }
			set { this["apiSecret"] = value; }
		}

		[ConfigurationProperty("cookieSupport", IsRequired = false, DefaultValue = true)]
		public bool CookieSupport
		{
			get { return (bool)this["cookieSupport"]; }
			set { this["cookieSupport"] = value; }
		}

		[ConfigurationProperty("useXfbml", IsRequired = false, DefaultValue = false)]
		public bool UseXfbml
		{
			get { return (bool)this["useXfbml"]; }
			set { this["useXfbml"] = value; }
		}

		[ConfigurationProperty("permissions", IsRequired = false, DefaultValue = "")]
		public string Permissions
		{
			get { return (string)this["permissions"]; }
			set { this["permissions"] = value; }
		}
	}
}