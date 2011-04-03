using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Areas.Facebook.Models
{
	public class UserUpdate
	{
		public long Uid { get; set; }
		public List<string> ChangedFields { get; set; }
		//public 
	}
}