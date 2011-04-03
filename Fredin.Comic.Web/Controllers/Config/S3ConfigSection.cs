using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace Fredin.Comic.Web.Config
{
	public class S3ConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("publicKey", IsRequired = true)]
		public string PublicKey
		{
			get { return (string)this["publicKey"]; }
			set { this["publicKey"] = value; }
		}

		[ConfigurationProperty("privateKey", IsRequired = true)]
		public string PrivateKey
		{
			get { return (string)this["privateKey"]; }
			set { this["privateKey"] = value; }
		}

		[ConfigurationProperty("renderBucket", IsRequired = true)]
		public string RenderBucket
		{
			get { return (string)this["renderBucket"]; }
			set { this["renderBucket"] = value; }
		}

		[ConfigurationProperty("comicPrefix", IsRequired = true)]
		public string ComicPrefix
		{
			get { return (string)this["comicPrefix"]; }
			set { this["comicPrefix"] = value; }
		}

		[ConfigurationProperty("thumb50Prefix", IsRequired = true)]
		public string Thumb50Prefix
		{
			get { return (string)this["thumb50Prefix"]; }
			set { this["thumb50Prefix"] = value; }
		}

		[ConfigurationProperty("thumb150Prefix", IsRequired = true)]
		public string Thumb150Prefix
		{
			get { return (string)this["thumb150Prefix"]; }
			set { this["thumb150Prefix"] = value; }
		}

		[ConfigurationProperty("framePrefix", IsRequired = true)]
		public string FramePrefix
		{
			get { return (string)this["framePrefix"]; }
			set { this["framePrefix"] = value; }
		}

		[ConfigurationProperty("frameThumb50Prefix", IsRequired = true)]
		public string FrameThumb50Prefix
		{
			get { return (string)this["frameThumb50Prefix"]; }
			set { this["frameThumb50Prefix"] = value; }
		}

		[ConfigurationProperty("frameThumb150Prefix", IsRequired = true)]
		public string FrameThumb150Prefix
		{
			get { return (string)this["frameThumb150Prefix"]; }
			set { this["frameThumb150Prefix"] = value; }
		}
	}
}