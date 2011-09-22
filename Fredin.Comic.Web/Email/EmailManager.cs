using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

using Fredin.Comic.Data;
using Fredin.Util;
using System.Text;
using System.Net.Mail;
using Fredin.Comic.Config;
using System.Net;

namespace Fredin.Comic.Web.Email
{
	public sealed class EmailManager
	{
		private HttpServerUtilityBase Server { get; set; }

		private string Master { get; set; }

		public EmailManager(HttpServerUtilityBase server)
		{
			this.Server = server;
			this.Master = this.GetTemplate("Master.html");
		}

		private string GetTemplate(string templateFile)
		{
			string filePath = Path.Combine(this.Server.MapPath(@"\Email"), templateFile);
			return File.ReadAllText(filePath);
		}

		public void SendEmail(User to, string templateFile, Dictionary<string, string> data)
		{
			if (!data.ContainsKey("id"))
			{
				throw new Exception("Cannot send email without an engagement history ID.");
			}
			if (!data.ContainsKey("to.address"))
			{
				data.Add("to.address", to.Email);
			}
			if (!data.ContainsKey("to.name"))
			{
				data.Add("to.name", to.Nickname);
			}
			if (!data.ContainsKey("title"))
			{
				data.Add("title", "Comic Mashup");
			}
			if (!data.ContainsKey("to.hash"))
			{
				data.Add("to.hash", to.Email.ComputeMd5());
			}

			string template = this.ExecuteTemplate(this.GetTemplate(templateFile), data);
			data.Add("content", template);

			string message = this.ExecuteTemplate(this.Master, data);

			// send email
			MailMessage mail = new MailMessage(new MailAddress(ComicConfigSectionGroup.Smtp.From, "Comic Mashup"), new MailAddress(data["to.address"], data["to.name"]));
			mail.IsBodyHtml = true;
			mail.Subject = data["title"];
			mail.Body = message;

			SmtpClient client = new SmtpClient(ComicConfigSectionGroup.Smtp.Server, ComicConfigSectionGroup.Smtp.Port);
			client.EnableSsl = true;
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(ComicConfigSectionGroup.Smtp.Username, ComicConfigSectionGroup.Smtp.Password);

			client.Send(mail);
		}

		private string ExecuteTemplate(string template, Dictionary<string, string> data)
		{
			// parse template
			StringBuilder builder = new StringBuilder(template.Length);
			int lastIndex = 0;

			Regex templateRegex = new Regex(@"\{[^}]+\}");
			Match match = templateRegex.Match(template);
			while (match != null && match.Success)
			{
				string key = match.Value.TrimStart('{').TrimEnd('}');
				string value = String.Empty;
				if (data.ContainsKey(key))
				{
					value = data[key];
				}

				builder.Append(template.Substring(lastIndex, match.Index - lastIndex));
				builder.Append(value);

				lastIndex = match.Index + match.Length;
				match = match.NextMatch();
			}
			builder.Append(template.Substring(lastIndex));

			return builder.ToString();
		}
	}
}