using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Fredin.Comic.Web.Controllers
{
	public class JsonActionAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			string contentType = filterContext.HttpContext.Request.ContentType;
			if(contentType.Contains("application/json"))
			{
				using(StreamReader reader = new StreamReader(filterContext.HttpContext.Request.InputStream))
				{
					JavaScriptSerializer serializer = new JavaScriptSerializer();
					IDictionary<string, object> parameters = (IDictionary<string, object>)serializer.DeserializeObject(reader.ReadToEnd());
					MethodInfo method = serializer.GetType().GetMethod("ConvertToType", new Type[] { typeof(object) });
					
					foreach(ParameterDescriptor p in filterContext.ActionDescriptor.GetParameters())
					{
						if(parameters == null || !parameters.ContainsKey(p.ParameterName))
						{
							throw new ArgumentException("Missing parameter", p.ParameterName);
						}
						filterContext.ActionParameters[p.ParameterName] = method.MakeGenericMethod(p.ParameterType).Invoke(serializer, new[] {parameters[p.ParameterName]});
					}
				}
			}
		}

		//public override void OnResultExecuted(ResultExecutedContext filterContext)
		//{
		//    string contentType = filterContext.HttpContext.Request.ContentType;
		//    if(contentType.Contains("application/json") && filterContext.Result.GetType() == typeof(ActionResult))
		//    {
		//        JsonResult result = new JsonResult();
		//        result.Data = filterContext.Result;
		//        filterContext.Result = result;
		//    }
		//}
	}
}