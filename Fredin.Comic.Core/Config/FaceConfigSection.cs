using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class FaceConfigSection : ConfigurationSection
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
	}
}