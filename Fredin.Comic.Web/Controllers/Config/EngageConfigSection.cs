using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace Fredin.Comic.Web.Config
{
	public class EngageConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("supportEmail", IsRequired = true)]
		public string SupportEmail
		{
			get { return (string)this["supportEmail"]; }
			set { this["supportEmail"] = value; }
		}

		[ConfigurationProperty("smtpHost", IsRequired = true)]
		public string SmtpHost
		{
			get { return (string)this["smtpHost"]; }
			set { this["smtpHost"] = value; }
		}

		[ConfigurationProperty("smtpPort", IsRequired = true)]
		public int SmtpPort
		{
			get { return (int)this["smtpPort"]; }
			set { this["smtpPort"] = value; }
		}

		/// <summary>
		/// Frequency to engage users that haven't created an album in days.
		/// </summary>
		[ConfigurationProperty("createAlbumFrequency", IsRequired = false, DefaultValue = 7)]
		public int CreateAlbumFrequency
		{
			get { return (int)this["createAlbumFrequency"]; }
			set { this["createAlbumFrequency"] = value; }
		}

		/// <summary>
		/// Frequency to run the job in hours
		/// </summary>
		[ConfigurationProperty("createAlbumInterval", IsRequired = false, DefaultValue = 2)]
		public int CreateAlbumInterval
		{
			get { return (int)this["createAlbumInterval"]; }
			set { this["createAlbumInterval"] = value; }
		}

		/// <summary>
		/// Frequency to engage users that haven't published an album in days.
		/// </summary>
		[ConfigurationProperty("publishAlbumFrequency", IsRequired = false, DefaultValue = 7)]
		public int PublishAlbumFrequency
		{
			get { return (int)this["publishAlbumFrequency"]; }
			set { this["publishAlbumFrequency"] = value; }
		}

		/// <summary>
		/// Frequency to run the job in hours
		/// </summary>
		[ConfigurationProperty("publishAlbumInterval", IsRequired = false, DefaultValue = 2)]
		public int PublishAlbumInterval
		{
			get { return (int)this["publishAlbumInterval"]; }
			set { this["publishAlbumInterval"] = value; }
		}
	}
}
