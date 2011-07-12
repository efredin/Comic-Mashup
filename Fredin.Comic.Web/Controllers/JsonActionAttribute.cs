using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using log4net;

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
						if(filterContext.ActionParameters[p.ParameterName] == null && (parameters == null || !parameters.ContainsKey(p.ParameterName)))
						{
							throw new ArgumentException(String.Format("Missing parameter for action {0}", filterContext.ActionDescriptor.ActionName), p.ParameterName);
						}
						filterContext.ActionParameters[p.ParameterName] = method.MakeGenericMethod(p.ParameterType).Invoke(serializer, new[] {parameters[p.ParameterName]});
					}
				}
			}
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext.Exception != null)
			{
				LogManager.GetLogger(filterContext.Controller.GetType()).Error(filterContext.Exception);

#if DEBUG
				object data = new { error = filterContext.Exception.ToString() };
#else
				object data = new { error = filterContext.Exception.Message };
#endif

				filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
				filterContext.Result = new JsonResult()
				{
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					ContentType = "application/json",
					Data = data
				};
				filterContext.ExceptionHandled = true;
			}

			base.OnActionExecuted(filterContext);
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