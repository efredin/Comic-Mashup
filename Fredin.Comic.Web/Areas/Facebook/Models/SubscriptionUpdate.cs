using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Areas.Facebook.Models
{
	public class SubscriptionUpdate<TObject>
	{
		public string Object { get; set; }

		public List<TObject> Entry { get; set; }
	}
}