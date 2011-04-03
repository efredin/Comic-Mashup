using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Fredin.Comic.Image.Filter;

namespace Fredin.Comic.Render
{
	public abstract class ComicRenderBase : IComicRender
	{
		public abstract Bitmap Render(Bitmap sourceImage);

		public abstract void SetRenderParameterValues(Dictionary<string, object> values);

		public abstract List<RenderParameter> GetRenderParameters();
	}
}
