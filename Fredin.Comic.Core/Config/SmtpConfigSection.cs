using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class SmtpConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("server", IsRequired = true)]
		public string Server
		{
			get { return (string)this["server"]; }
			set { this["server"] = value; }
		}

		[ConfigurationProperty("port", IsRequired = true)]
		public int Port
		{
			get { return (int)this["port"]; }
			set { this["port"] = value; }
		}

		[ConfigurationProperty("username", IsRequired = true)]
		public string Username
		{
			get { return (string)this["username"]; }
			set { this["username"] = value; }
		}

		[ConfigurationProperty("password", IsRequired = true)]
		public string Password
		{
			get { return (string)this["password"]; }
			set { this["password"] = value; }
		}
	}
}
