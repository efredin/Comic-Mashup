using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Fredin.Comic.Web
{
	public class EmailAttribute : RegularExpressionAttribute
	{
		public EmailAttribute() : base("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$")
		{
		}
	}
}