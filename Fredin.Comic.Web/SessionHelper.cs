using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Fredin.Util;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web
{
	public class SessionHelper
	{
		public const string KEY_THEME = "theme";
		public const string KEY_ACTIVE_USER = "user";
		public const string KEY_FRIENDS = "friends";
		public const string KEY_GUEST_USER = "guest";
		public const string KEY_LOCALE = "locale";
		public const string KEY_FB = "fb";
		
		protected HttpContextBase HttpContext { get; set; }

		/// <summary>
		/// Active session user. Guest users will return null
		/// </summary>
		public virtual User ActiveUser
		{
			get { return this.HttpContext.Session[KEY_ACTIVE_USER] as User; }
			set { this.HttpContext.Session[KEY_ACTIVE_USER] = value; }
		}

		public virtual List<long> Friends
		{
			get { return this.HttpContext.Session[KEY_FRIENDS] as List<long>; }
			set { this.HttpContext.Session[KEY_FRIENDS] = value; }
		}

		public virtual User GuestUser
		{
			get { return this.HttpContext.Session[KEY_GUEST_USER] as User; }
			set { this.HttpContext.Session[KEY_GUEST_USER] = value; }
		}

		public virtual string Theme
		{
			get { return this.HttpContext.Session[KEY_THEME] as string ?? "mashup"; }
			set { this.HttpContext.Session[KEY_THEME] = value; }
		}

		public string Locale
		{
			get { return (string)this.HttpContext.Session[KEY_LOCALE]; }
			set { this.HttpContext.Session[KEY_LOCALE] = value; }
		}

		public bool Fb
		{
			get { return this.HttpContext.Session[KEY_FB] == null ? false : (bool)this.HttpContext.Session[KEY_FB]; }
			set { this.HttpContext.Session[KEY_FB] = value; }
		}

		public SessionHelper(HttpContextBase httpContext)
		{
			this.HttpContext = httpContext;
		}
	}
}