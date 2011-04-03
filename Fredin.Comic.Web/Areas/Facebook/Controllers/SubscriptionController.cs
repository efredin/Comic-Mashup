//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//using Fredin.Util;
//using Fredin.Comic.Web.Models;
//using Fredin.Comic.Web.Controllers;
//using System.Web.Script.Serialization;
//using System.IO;

//namespace Fredin.Comic.Web.Areas.Facebook.Controllers
//{
//    public class SubscriptionController : ComicControllerBase
//    {
//        private const string TOKEN_STRING = "{0}Fredin";

//        /// <summary>
//        /// Graph API Callback handler
//        /// </summary>
//        /// <param name="uid"></param>
//        /// <returns></returns>
//        public ViewResult Update(long uid)
//        {
//            string mode = this.Request["hub.mode"];
//            string challenge = this.Request["hub.challenge"];
//            string requestToken = this.Request["hub.verify_token"];

//            string token = String.Format(TOKEN_STRING, uid).ComputeMd5();

//            if (token != requestToken)
//            {
//                throw new UnauthorizedAccessException("Invalid token.");
//            }

//            if(this.Request.HttpMethod == "GET" && mode == "subscribe")
//            {
//                return View();
//            }
//            else if(this.Request.HttpMethod == "POST")
//            {
//                string body = null;
//                try
//                {
//                    // Get request content
//                    using (StreamReader reader = new StreamReader(this.Request.InputStream))
//                    {
//                        body = reader.ReadToEnd();
//                    }

//                    JavaScriptSerializer serializer = new JavaScriptSerializer();
//                    Dictionary<string, object> data = (Dictionary<string, object>)serializer.DeserializeObject(body);

//                    if (data["object"] == "user")
//                    {
//                        // Updating users
//                        List<object> data["entry"]

//                        User activeUser = this.EntityContext.TryGetUser(uid);
//                        if (activeUser != null)
//                        {
//                            //activeUser.Email = 
//                        }
//                    }
//                }
//                catch (Exception x)
//                {
//                    this.Log.ErrorFormat("Unable to handle subscription update. Request Body:\n{0}\nException:\n{1}", body, x);
//                    throw;
//                }
//            }
//        }

//        //private void UpdateUsers(List<string> entries, JavaScriptSerializer serializer)
//        //{

//        //}
//    }
//}
