using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace Fredin.Comic.Web
{
	public class ComicFormViewEngine : WebFormViewEngine
	{
		public ComicFormViewEngine()
		{
			base.MasterLocationFormats = new string[]
			{
				"~/View/{2}/{0}.master"
			};

			base.ViewLocationFormats = new string[]
			{
				"~/View/{2}/{1}/{0}.aspx"
			};

			base.PartialViewLocationFormats = new string[]
			{
				"~/View/{2}/{1}/{0}.ascx"
			};
		}

		public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			ViewEngineResult found;
			string[] viewSearched;
			string[] masterSearched;

			if (controllerContext == null)
			{
				throw new ArgumentException("controllerContext");
			}
			if (string.IsNullOrEmpty(viewName))
			{
				throw new ArgumentException("viewName");
			}

			string appName = this.GetAppName(controllerContext);
			string controller = controllerContext.RouteData.GetRequiredString("controller");

			string viewPath = this.GetPath(controllerContext, this.ViewLocationFormats, "ViewLocationFormats",
					viewName, appName, controller, "View", useCache, out viewSearched);

			string masterPath = this.GetPath(controllerContext, this.MasterLocationFormats, "MasterLocationFormats",
					masterName, appName, controller, "Master", useCache, out masterSearched);

			if (!string.IsNullOrEmpty(viewPath) && (!string.IsNullOrEmpty(masterPath) || string.IsNullOrEmpty(masterName)))
			{
				found = new ViewEngineResult(this.CreateView(controllerContext, viewPath, masterPath), this);
			}
			else
			{
				found = new ViewEngineResult(viewSearched.Union<string>(masterSearched));
			}

			return found;
		}

		private string GetAppName(ControllerContext controllerContext)
		{
			string appName = controllerContext.HttpContext.Items["appName"] as string;
			if (appName == null)
			{
				appName = "Web";
			}
			return appName;
		}

		private string GetPath(ControllerContext controllerContext, string[] locations, string locationsPropertyName,
			string name, string themeName, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
		{
			searchedLocations = null;
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			if ((locations == null) || (locations.Length == 0))
			{
				throw new InvalidOperationException("locations must not be null or emtpy.");
			}

			bool flag = IsSpecificPath(name);
			string key = this.CreateCacheKey(cacheKeyPrefix, name, flag ? string.Empty : controllerName, themeName);
			if (useCache)
			{
				string viewLocation = this.ViewLocationCache.GetViewLocation(controllerContext.HttpContext, key);
				if (viewLocation != null)
				{
					return viewLocation;
				}
			}
			if (!flag)
			{
				return this.GetPathFromGeneralName(controllerContext, locations, name, controllerName, themeName, key, ref searchedLocations);
			}
			return this.GetPathFromSpecificName(controllerContext, name, key, ref searchedLocations);
		}

		private static bool IsSpecificPath(string name)
		{
			char ch = name[0];
			if (ch != '~')
			{
				return (ch == '/');
			}
			return true;
		}

		private string CreateCacheKey(string prefix, string viewName, string controllerName, string appName)
		{
			return String.Format(CultureInfo.InvariantCulture, ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}",
				new object[] { base.GetType().AssemblyQualifiedName, prefix, viewName, controllerName, appName });
		}

		private string GetPathFromGeneralName(ControllerContext controllerContext, string[] locations, string viewName,
			string controllerName, string appName, string cacheKey, ref string[] searchedLocations)
		{
			string virtualPath = String.Empty;
			searchedLocations = new string[locations.Length];
			for (int i = 0; i < locations.Length; i++)
			{
				string str2 = string.Format(CultureInfo.InvariantCulture, locations[i], new object[] { viewName, controllerName, appName });

				if (this.FileExists(controllerContext, str2))
				{
					searchedLocations = null;
					virtualPath = str2;
					this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
					return virtualPath;
				}
				searchedLocations[i] = str2;
			}
			return virtualPath;
		}

		private string GetPathFromSpecificName(ControllerContext controllerContext, string viewName, string cacheKey, ref string[] searchedLocations)
		{
			string virtualPath = viewName;
			if (!this.FileExists(controllerContext, viewName))
			{
				virtualPath = string.Empty;
				searchedLocations = new string[] { viewName };
			}
			this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
			return virtualPath;
		}

	}
}
/*

public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
    {
        string[] strArray;
        string[] strArray2;

        if (controllerContext == null)
        {
            throw new ArgumentNullException("controllerContext");
        }
        if (string.IsNullOrEmpty(viewName))
        {
            throw new ArgumentException("viewName must be specified.", "viewName");
        }

        string themeName = this.GetThemeToUse(controllerContext);

        string requiredString = controllerContext.RouteData.GetRequiredString("controller");

        string viewPath = this.GetPath(controllerContext, this.ViewLocationFormats, "ViewLocationFormats",
                viewName, themeName, requiredString, "View", useCache, out strArray);

        string masterPath = this.GetPath(controllerContext, this.MasterLocationFormats, "MasterLocationFormats",
                masterName, themeName, requiredString, "Master", useCache, out strArray2);

        if (!string.IsNullOrEmpty(viewPath) && (!string.IsNullOrEmpty(masterPath) || string.IsNullOrEmpty(masterName)))
        {
            return new ViewEngineResult(this.CreateView(controllerContext, viewPath, masterPath), this);
        }
        return new ViewEngineResult(strArray.Union<string>(strArray2));
    }
private string GetThemeToUse(ControllerContext controllerContext)
    {
        string themeName = controllerContext.HttpContext.Items["themeName"] as string;
        if (themeName == null) themeName = "Default";
        return themeName;
    }

    private static readonly string[] _emptyLocations;

    private string GetPath(ControllerContext controllerContext, string[] locations, string locationsPropertyName,
        string name, string themeName, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
    {
        searchedLocations = _emptyLocations;
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }
        if ((locations == null) || (locations.Length == 0))
        {
            throw new InvalidOperationException("locations must not be null or emtpy.");
        }

        bool flag = IsSpecificPath(name);
        string key = this.CreateCacheKey(cacheKeyPrefix, name, flag ? string.Empty : controllerName, themeName);
        if (useCache)
        {
            string viewLocation = this.ViewLocationCache.GetViewLocation(controllerContext.HttpContext, key);
            if (viewLocation != null)
            {
                return viewLocation;
            }
        }
        if (!flag)
        {
            return this.GetPathFromGeneralName(controllerContext, locations, name, controllerName, themeName, key, ref searchedLocations);
        }
        return this.GetPathFromSpecificName(controllerContext, name, key, ref searchedLocations);
    }
*/