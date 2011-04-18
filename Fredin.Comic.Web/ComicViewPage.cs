using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

namespace Fredin.Comic.Web
{
	public class ComicViewPage<TModel> : ViewPage<TModel>
	{
		protected SessionHelper SessionManager { get; set; }

		public override void ProcessRequest(HttpContext context)
		{
			this.SessionManager = new SessionHelper(this.ViewContext.HttpContext);
			base.ProcessRequest(context);
		}

		protected override void InitializeCulture()
		{
			if (this.SessionManager.Locale != null)
			{
				try
				{
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(this.SessionManager.Locale);
					Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(this.SessionManager.Locale);

					this.UICulture = this.SessionManager.Locale;
					this.Culture = this.SessionManager.Locale;
				}
				catch (Exception x)
				{
					log4net.LogManager.GetLogger(this.GetType()).ErrorFormat("Unable to set locale culture to {0}", this.SessionManager.Locale);
				}
			}
			base.InitializeCulture();
		}
	}
}