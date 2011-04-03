using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class StorageConfigSection : ConfigurationSection
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

		[ConfigurationProperty("renderTaskDirectory", IsRequired = false, DefaultValue = "render-task/")]
		public string RenderTaskDirectory
		{
			get { return (string)this["renderTaskDirectory"]; }
			set { this["renderTaskDirectory"] = value; }
		}

		[ConfigurationProperty("comicPrefix", IsRequired = true)]
		public string ComicPrefix
		{
			get { return (string)this["comicPrefix"]; }
			set { this["comicPrefix"] = value; }
		}

		[ConfigurationProperty("framePrefix", IsRequired = true)]
		public string FramePrefix
		{
			get { return (string)this["framePrefix"]; }
			set { this["framePrefix"] = value; }
		}

		[ConfigurationProperty("thumbPrefix", IsRequired = true)]
		public string ThumbPrefix
		{
			get { return (string)this["thumbPrefix"]; }
			set { this["thumbPrefix"] = value; }
		}

		[ConfigurationProperty("frameThumbPrefix", IsRequired = true)]
		public string FrameThumbPrefix
		{
			get { return (string)this["frameThumbPrefix"]; }
			set { this["frameThumbPrefix"] = value; }
		}

		[ConfigurationProperty("photoPrefix", IsRequired = true)]
		public string PhotoPrefix
		{
			get { return (string)this["photoPrefix"]; }
			set { this["photoPrefix"] = value; }
		}
	}
}