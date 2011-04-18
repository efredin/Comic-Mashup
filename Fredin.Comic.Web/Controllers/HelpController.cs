using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fredin.Comic.Data;
using Fredin.Util;
using Fredin.Comic.Web.Models;
using System.Net.Mail;
using Fredin.Comic.Config;
using System.Net;
using System.Configuration;

namespace Fredin.Comic.Web.Controllers
{
    public class HelpController : ComicControllerBase
    {
		public ActionResult Faq()
		{
			return this.View();
		}

		public ActionResult Contact()
		{
			ViewContact data = new ViewContact();

			if (this.ActiveUser != null)
			{
				data.Nickname = this.ActiveUser.Nickname;
				data.Email = this.ActiveUser.Email;
			}

			return this.View(data);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Contact(ViewContact data)
		{
			MailMessage message = new MailMessage(new MailAddress(ComicConfigSectionGroup.Smtp.From), new MailAddress(ComicConfigSectionGroup.Smtp.From));
			message.Subject = "Comic Mashup Contact Submission";
			message.Body = String.Format("From: {0}\nMessage: {1}", data.Email, data.Message);

			SmtpClient client = new SmtpClient(ComicConfigSectionGroup.Smtp.Server, ComicConfigSectionGroup.Smtp.Port);
			client.EnableSsl = true;
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(ComicConfigSectionGroup.Smtp.Username, ComicConfigSectionGroup.Smtp.Password);
			
			client.Send(message);

			return this.RedirectToAction("ContactConfirm");
		}

		public ActionResult ContactConfirm()
		{
			return this.View();
		}
    }
}
