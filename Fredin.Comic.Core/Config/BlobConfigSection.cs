using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class BlobConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("renderContainer", IsRequired = true)]
		public string RenderContainer
		{
			get { return (string)this["renderContainer"]; }
			set { this["renderContainer"] = value; }
		}

		[ConfigurationProperty("taskContainer", IsRequired = true)]
		public string TaskContainer
		{
			get { return (string)this["taskContainer"]; }
			set { this["taskContainer"] = value; }
		}

		[ConfigurationProperty("renderTaskDirectory", IsRequired = false, DefaultValue = "render/")]
		public string RenderTaskDirectory
		{
			get { return (string)this["renderTaskDirectory"]; }
			set { this["renderTaskDirectory"] = value; }
		}

		[ConfigurationProperty("profileTaskDirectory", IsRequired = false, DefaultValue = "profile/")]
		public string ProfileTaskDirectory
		{
			get { return (string)this["profileTaskDirectory"]; }
			set { this["profileTaskDirectory"] = value; }
		}

		[ConfigurationProperty("photoTaskDirectory", IsRequired = false, DefaultValue = "photo/")]
		public string PhotoTaskDirectory
		{
			get { return (string)this["photoTaskDirectory"]; }
			set { this["photoTaskDirectory"] = value; }
		}

		[ConfigurationProperty("comicDirectory", IsRequired = true)]
		public string ComicDirectory
		{
			get { return (string)this["comicDirectory"]; }
			set { this["comicDirectory"] = value; }
		}

		[ConfigurationProperty("frameDirectory", IsRequired = true)]
		public string FrameDirectory
		{
			get { return (string)this["frameDirectory"]; }
			set { this["frameDirectory"] = value; }
		}

		[ConfigurationProperty("thumbDirectory", IsRequired = true)]
		public string ThumbDirectory
		{
			get { return (string)this["thumbDirectory"]; }
			set { this["thumbDirectory"] = value; }
		}

		[ConfigurationProperty("frameThumbDirectory", IsRequired = true)]
		public string FrameThumbDirectory
		{
			get { return (string)this["frameThumbDirectory"]; }
			set { this["frameThumbDirectory"] = value; }
		}

		[ConfigurationProperty("photoDirectory", IsRequired = true)]
		public string PhotoDirectory
		{
			get { return (string)this["photoDirectory"]; }
			set { this["photoDirectory"] = value; }
		}

		[ConfigurationProperty("profileDirectory", IsRequired = true)]
		public string ProfileDirectory
		{
			get { return (string)this["profileDirectory"]; }
			set { this["profileDirectory"] = value; }
		}
	}
}