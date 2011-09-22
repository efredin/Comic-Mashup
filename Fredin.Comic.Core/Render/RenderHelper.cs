using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

using Fredin.Comic;
using Fredin.Comic.Image;
using AForge.Imaging.Filters;

namespace Fredin.Comic.Render
{
	public class RenderHelper
	{
		public Size MaxSize { get; set; }

		public RenderHelper()
		{
			this.MaxSize = new Size(580, 600);
		}

		public RenderHelper(Size maxSize)
		{
			this.MaxSize = maxSize;
		}

		protected IComicRender GetRenderEffect(ComicEffectType effect)
		{
			IComicRender renderEffect;
			switch (effect)
			{
				case ComicEffectType.ColorSketch:
				default:
					renderEffect = new ColorSketch();
					break;

				case ComicEffectType.PencilSketch:
					renderEffect = new PencilSketch();
					break;

				case ComicEffectType.Comic:
					renderEffect = new Comic();
					break;
			}
			return renderEffect;
		}

		public List<RenderParameter> GetRenderParameters(ComicEffectType effect)
		{
			IComicRender renderEffect = this.GetRenderEffect(effect);
			return renderEffect.GetRenderParameters();
		}

		public ImageRenderData RenderEffect(string rawUrl, ComicEffectType effectType, Dictionary<string, object> parameterValues)
		{
			HttpWebRequest photoRequest = (HttpWebRequest)HttpWebRequest.Create(rawUrl);
			photoRequest.Timeout = 5000; // 5 seconds
			HttpWebResponse photoResponse = photoRequest.GetResponse() as HttpWebResponse;
			Bitmap rawPhoto = new Bitmap(photoResponse.GetResponseStream());

			return this.RenderEffect(rawPhoto, effectType, parameterValues);
		}

		public ImageRenderData RenderEffect(Bitmap rawPhoto, ComicEffectType effectType, Dictionary<string, object> parameterValues)
		{
			// Resize
			rawPhoto = this.ResizeToMax(rawPhoto);

			// Convert bitmap to color
			if (rawPhoto.PixelFormat != PixelFormat.Format24bppRgb)
			{
				rawPhoto = rawPhoto.ConvertFormat(PixelFormat.Format24bppRgb);
			}
	
			// Convert 
			IComicRender renderEffect = this.GetRenderEffect(effectType);

			renderEffect.SetRenderParameterValues(parameterValues);
			Bitmap renderPhoto = renderEffect.Render(rawPhoto);

			// Read rendered photo into memory
			MemoryStream renderStream = new MemoryStream();
			renderPhoto.Save(renderStream, ImageFormat.Jpeg);
			renderStream.Seek(0, SeekOrigin.Begin);
			byte[] renderBuffer = new byte[renderStream.Length];
			renderStream.Read(renderBuffer, 0, renderBuffer.Length);

			// Read raw photo into memory
			MemoryStream rawStream = new MemoryStream();
			rawPhoto.Save(rawStream, ImageFormat.Jpeg);
			rawStream.Seek(0, SeekOrigin.Begin);
			byte[] rawBuffer = new byte[rawStream.Length];
			rawStream.Read(rawBuffer, 0, rawBuffer.Length);

			return new ImageRenderData(rawStream, renderStream);
		}

		public Bitmap ResizeToMax(Bitmap inputBitmap)
		{
			return this.Resize(this.MaxSize, inputBitmap);
		}

		public Bitmap Resize(Size resize, Bitmap inputBitmap)
		{
			Bitmap outputBitmap = inputBitmap;

			// Resize to max dimension
			ResizeBicubic filterResize = null;
			if (inputBitmap.Height > resize.Height)
			{
				double factor = System.Convert.ToDouble(inputBitmap.Height) / System.Convert.ToDouble(resize.Height);
				int newWidth = System.Convert.ToInt32(Math.Round(System.Convert.ToDouble(inputBitmap.Width) / factor, 0));

				filterResize = new ResizeBicubic(newWidth, resize.Height);
			}
			else if (inputBitmap.Width > resize.Width)
			{
				double factor = System.Convert.ToDouble(inputBitmap.Width) / System.Convert.ToDouble(resize.Width);
				int newHeight = System.Convert.ToInt32(Math.Round(System.Convert.ToDouble(inputBitmap.Height) / factor, 0));

				filterResize = new ResizeBicubic(resize.Width, newHeight);
			}
			if (filterResize != null)
			{
				outputBitmap = filterResize.Apply(inputBitmap);
			}

			return outputBitmap;
		}
	}
}
