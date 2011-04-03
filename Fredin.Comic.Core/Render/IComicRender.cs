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
	public interface IComicRender
	{
		Bitmap Render(Bitmap sourceImage);

		void SetRenderParameterValues(Dictionary<string, object> values);

		List<RenderParameter> GetRenderParameters();
	}
}
