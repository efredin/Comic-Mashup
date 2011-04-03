using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace Fredin.Comic.Web.Config
{
	public class WebConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("baseUrl", IsRequired = true)]
		public string BaseUrl
		{
			get { return (string)this["baseUrl"]; }
			set { this["baseUrl"] = value; }
		}

		[ConfigurationProperty("staticBaseUrl", IsRequired = true)]
		public string StaticBaseUrl
		{
			get { return (string)this["staticBaseUrl"]; }
			set { this["staticBaseUrl"] = value; }
		}

		[ConfigurationProperty("applicationBaseUrl", IsRequired = true)]
		public string ApplicationBaseUrl
		{
			get { return (string)this["applicationBaseUrl"]; }
			set { this["applicationBaseUrl"] = value; }
		}

		[ConfigurationProperty("staticFilePath", IsRequired = true)]
		public string StaticFilePath
		{
			get { return (string)this["staticFilePath"]; }
			set { this["staticFilePath"] = value; }
		}

		[ConfigurationProperty("renderStaticUrl", IsRequired = true)]
		public string RenderStaticUrl
		{
			get { return (string)this["renderStaticUrl"]; }
			set { this["renderStaticUrl"] = value; }
		}

		[ConfigurationProperty("geoIpDatabase", IsRequired = true)]
		public string GeoIpDatabase
		{
			get { return (string)this["geoIpDatabase"]; }
			set { this["geoIpDatabase"] = value; }
		}

		[ConfigurationProperty("trackerId", IsRequired = true)]
		public string TrackerId
		{
			get { return (string)this["trackerId"]; }
			set { this["trackerId"] = value; }
		}
	}
}