using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Fredin.Comic.Image;
using Fredin.Comic.Image.Filter;
using log4net;

namespace Fredin.Comic.Render
{
	public class Comic : ComicRenderBase
	{
		#region [Property]

		public int Coloring { get; set; }

		#endregion

		public Comic()
		{
			this.Coloring = 6;
		}

		#region [Method]

		public override List<RenderParameter> GetRenderParameters()
		{
			List<RenderParameter> renderParams = new List<RenderParameter>();

			//renderParams.Add(new RenderParameter("edging", "Edging", 3, 1, 5));
			//renderParams.Add(new RenderParameter("coloring", "Coloring", 12, 6, 18));

			return renderParams;
		}

		public override void SetRenderParameterValues(Dictionary<string, object> values)
		{
			if (values != null)
			{
				if (values.ContainsKey("coloring"))
				{
					this.Coloring = Convert.ToInt32(values["coloring"]);
				}
			}
		}

		/// <summary>
		/// Creates a comic rendered copy of the input image.
		/// </summary>
		public override Bitmap Render(Bitmap sourceImage)
		{
			GrayscaleToRGB convertColor = new GrayscaleToRGB();

			if (sourceImage.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				sourceImage = convertColor.Apply(sourceImage);
			}

			BilateralBlur blur = new BilateralBlur(3, 0.1);
			Bitmap comic = blur.Apply(sourceImage);

			// Edges
			Bitmap grayscale = Grayscale.CommonAlgorithms.Y.Apply(comic);
			SobelEdgeDetector sobelEdge = new SobelEdgeDetector();
			sobelEdge.ScaleIntensity = true;
			Bitmap edgeLayer = sobelEdge.Apply(grayscale);

			edgeLayer = convertColor.Apply(edgeLayer);

			Invert invertEdge = new Invert();
			invertEdge.ApplyInPlace(edgeLayer);

			HSLLinear edgeLinear = new HSLLinear();
			edgeLinear.InLuminance.Min = 0;
			edgeLinear.InLuminance.Max = 0.8;
			edgeLinear.ApplyInPlace(edgeLayer);


			// highlights
			Bitmap highlightLayer = invertEdge.Apply(edgeLayer);
			Dilatation highlightDilitation = new Dilatation();
			highlightDilitation.ApplyInPlace(highlightLayer);

			BrightnessCorrection highlightBright = new BrightnessCorrection(-0.35);
			highlightBright.ApplyInPlace(highlightLayer);
			ColorDodge highlightBlend = new ColorDodge(highlightLayer);
			highlightBlend.ApplyInPlace(comic);


			// Merge edges with working layer
			Multiply multEdge = new Multiply(edgeLayer);
			multEdge.ApplyInPlace(comic);


			return comic;
		}

		#endregion
	}
}