using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

using log4net;

using Fredin.Comic.Web.Config;
using Fredin.Comic.Web.Model;
using Fredin.Util;

namespace Fredin.Comic.Web
{
	public class SessionManager
	{
		#region [Prameter]

		public const string ParamUser = "User";
		public const string ParamTempUploadFileName = "FileUpload";
		public const string ParamTempUploadThumb = "FileUploadThumb";
		public const string ParamQueueFeedback = "QueueFeedback";
		public const string ParamQueueFeedbackType = "QueueFeedbackType";

		#endregion

		#region [Property]

		protected ILog Log { get; set; }

		protected HttpContextBase HttpContext { get; set; }

		protected ComicModelContext EntityContext { get; set; }

		public User ActiveUser
		{
			get { return this.HttpContext.Session[ParamUser] as User; }
			set { this.HttpContext.Session[ParamUser] = value; }
		}

		public bool IsAuthenticated
		{
			get { return this.ActiveUser != null; }
		}

		public string TempUploadFileName
		{
			get { return this.HttpContext.Session[ParamTempUploadFileName] as String; }
			set 
			{
				if (!String.IsNullOrEmpty(this.TempUploadFileName))
				{
					System.IO.File.Delete(this.TempUploadFileName);
				}

				this.HttpContext.Session[ParamTempUploadFileName] = value; 
			}
		}

		public string TempUploadThumb
		{
			get { return this.HttpContext.Session[ParamTempUploadThumb] as String; }
			set 
			{
				if (!String.IsNullOrEmpty(this.TempUploadThumb))
				{
					System.IO.File.Delete(this.TempUploadThumb);
				}

				this.HttpContext.Session[ParamTempUploadThumb] = value; 
			}
		}

		public string RedirectUrl
		{
			get { return this.HttpContext.Session["redirectUrl"] as String; }
			set { this.HttpContext.Session["redirectUrl"] = value; }
		}

		public bool RedirectTargetParent
		{
			get { return (bool)(this.HttpContext.Session["redirectTargetParent"] ?? false); }
			set { this.HttpContext.Session["redirectTargetParent"] = value; }
		}

		//public string QueueFeedback
		//{
		//    get { return (string)this.HttpContext.Session[ParamQueueFeedback]; }
		//    set { this.HttpContext.Session[ParamQueueFeedback] = value; }
		//}

		//public ApplicationMaster.FeedbackType QueueFeedbackType
		//{
		//    get { return (ApplicationMaster.FeedbackType)this.HttpContext.Session[ParamQueueFeedbackType]; }
		//    set { this.HttpContext.Session[ParamQueueFeedbackType] = value; }
		//}

		#endregion

		#region [Method]

		public SessionManager(HttpContextBase httpContext, ComicModelContext entityContext)
		{
			this.Log = LogManager.GetLogger(this.GetType());

			this.EntityContext = entityContext;
			this.HttpContext = httpContext;
		}

		//public bool Login(string email, string password)
		//{
		//    bool loginSuccess = false;
		//    string hash = password.ComputeMd5();

		//    return this.EntityContext.User.OfType<EmailUser>()
		//        .FirstOrDefault(e => e.Email == email && e.PasswordHash == hash);


		//    using (UserController controller = new UserController(this.EntityContext))
		//    {
		//        EmailUser user = controller.TryLoginEmailUser(email, password);

		//        if (user != null)
		//        {
		//            this.ActiveUser = new ClientEmailUser(user);
		//            if (!user.IsValidated)
		//            {
		//                // May cause Server.Transfer
		//                this.RedirectValidate();
		//            }
		//            else
		//            {
		//                loginSuccess = true;
		//            }
		//        }
		//    }

		//    return loginSuccess;
		//}

		#endregion
	}
}