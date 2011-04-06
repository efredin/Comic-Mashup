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
			SmtpClient client = new SmtpClient(ComicConfigSectionGroup.Smtp.Server, ComicConfigSectionGroup.Smtp.Port);
			client.Credentials = new NetworkCredential(ComicConfigSectionGroup.Smtp.Username, ComicConfigSectionGroup.Smtp.Password);
			client.EnableSsl = true;

			MailMessage message = new MailMessage(new MailAddress(data.Email, data.Nickname), new MailAddress("efredin@gmail.com"));
			message.Subject = "Comic Mashup Contact Submission";
			message.Body = data.Message;

			client.Send(message);

			return this.RedirectToAction("ContactConfirm");
		}

		public ActionResult ContactConfirm()
		{
			return this.View();
		}
    }
}
