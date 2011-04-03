using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Fredin.Comic.Config
{
	public class ComicConfigSectionGroup : ConfigurationSectionGroup
	{
		public static WebConfigSection Web
		{
			get { return ConfigurationManager.GetSection("fredin.comic/web") as WebConfigSection; }
		}

		public static StorageConfigSection Storage
		{
			get { return ConfigurationManager.GetSection("fredin.comic/storage") as StorageConfigSection; }
		}

		public static FacebookConfigSection Facebook
		{
			get { return ConfigurationManager.GetSection("fredin.comic/facebook") as FacebookConfigSection; }
		}

		public static FaceConfigSection Face
		{
			get { return ConfigurationManager.GetSection("fredin.comic/face") as FaceConfigSection; }
		}
	}
}