using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientTemplateItem
	{
		public long TemplateItemId { get; set; }
		public long TemplateId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Ordinal { get; set; }

		public ClientTemplateItem(TemplateItem source)
		{
			this.TemplateItemId = source.TemplateItemId;
			this.TemplateId = source.TemplateId;
			this.X = source.X;
			this.Y = source.Y;
			this.Width = source.Width;
			this.Height = source.Height;
			this.Ordinal = source.Ordinal;
		}
	}
}