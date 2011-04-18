using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewMessage
	{
		public string Message { get; set; }

		public ViewMessage(string message)
		{
			this.Message = message;
		}
	}
}