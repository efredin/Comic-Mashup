using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Facebook
{
	public class FacebookApiException : Exception
	{
		public FacebookApiException()
			: base()
		{
		}

		public FacebookApiException(string message)
			: base(message)
		{
		}

		public FacebookApiException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}