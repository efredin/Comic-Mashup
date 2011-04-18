using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Text.RegularExpressions;

namespace Fredin.Comic.Web
{
	public class NotRouteConstraint : IRouteConstraint
	{
		public string Not { get; set; }

		public NotRouteConstraint(string not)
		{
			this.Not = not;
		}

		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			return !Regex.IsMatch(values[parameterName] as string, this.Not);
		}
	}
}