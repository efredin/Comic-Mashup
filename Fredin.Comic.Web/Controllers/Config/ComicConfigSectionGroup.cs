using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Web.Configuration;
using System.Collections.Generic;

namespace Fredin.Comic.Web.Config
{
	public class ComicConfigSectionGroup : ConfigurationSectionGroup
	{
		//private static ComicConfigSectionGroup _current;
		//private EngageConfigSection _engage;
		//private S3ConfigSection _s3;
		//private ComicConfigSection _config;

		#region [Property]

		public static WebConfigSection Web
		{
			get { return ConfigurationManager.GetSection("fredin.comic/web") as WebConfigSection; }
		}

		public static EngageConfigSection Engage
		{
			get { return ConfigurationManager.GetSection("fredin.comic/engage") as EngageConfigSection; }
		}

		public static S3ConfigSection S3
		{
			get { return ConfigurationManager.GetSection("fredin.comic/s3") as S3ConfigSection; }
		}

		public static FacebookConfigSection Facebook
		{
			get { return ConfigurationManager.GetSection("fredin.comic/facebook") as FacebookConfigSection; }
		}

		//public static ComicConfigSectionGroup Current
		//{
		//    get
		//    {
		//        if (_current == null)
		//        {
		//            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~/");
		//            List<ConfigurationSectionGroup> sections = new List<ConfigurationSectionGroup>();
		//            foreach (ConfigurationSectionGroup section in config.SectionGroups)
		//            {
		//                if (section is ComicConfigSectionGroup)
		//                {
		//                    sections.Add(section);
		//                }
		//            }
		//            //var sections = config.Sections.OfType<ComicConfigSection>().Where(section => section is ComicConfigSection).ToList();
		//            if (sections.Count > 1)
		//            {
		//                throw new ConfigurationErrorsException(String.Format("Too many instances of ComicConfigSectionGroup were found. Expected 1, found {0}.", sections.Count));
		//            }
		//            else if (sections.Count == 0)
		//            {
		//                throw new ConfigurationErrorsException("A configuration section of the type ComicConfigSectionGroup was not found in the current configuration.");
		//            }
		//            else
		//            {
		//                _current = (ComicConfigSectionGroup)sections.Single();
		//            }
		//        }
		//        return _current;
		//    }
		//}

		#endregion
	}
}
