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
	public class ColorSketch : ComicRenderBase
	{
		#region [Member]

		private static readonly int[,] ConvolutionKernel = 
		{ 
			{ -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1 },
			{ -1, -1, 29, -1, -1 },
			{ -1, -1, -1, -1, -1 },
			{ -1, -1, -5, -1, -1 }
		};

		#endregion

		#region [Property]

		public double Edging { get; set; }
		public double Coloring { get; set; }

		#endregion

		public ColorSketch()
		{
			this.Edging = 26;
			this.Coloring = 6;
		}

		#region [Method]

		public override List<RenderParameter> GetRenderParameters()
		{
			List<RenderParameter> renderParams = new List<RenderParameter>();

			renderParams.Add(new RenderParameter("edging", "Edging", 25, 1, 50));
			renderParams.Add(new RenderParameter("coloring", "Coloring", 1, 0, 5));

			return renderParams;
		}

		public override void SetRenderParameterValues(Dictionary<string, object> values)
		{
			if (values != null)
			{
				if (values.ContainsKey("edging"))
				{
					this.Edging = Convert.ToDouble(values["edging"]);
				}
				if (values.ContainsKey("coloring"))
				{
					this.Coloring = Convert.ToDouble(values["coloring"]);
				}
			}
		}

		/// <summary>
		/// Creates a comic rendered copy of the input image.
		/// </summary>
		public override Bitmap Render(Bitmap sourceImage)
		{
			// Converters
			GrayscaleY convertGray = new GrayscaleY();
			GrayscaleToRGB convertColor = new GrayscaleToRGB();

			// Convert grayscal images
			if (sourceImage.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				sourceImage = convertColor.Apply(sourceImage);
			}

			Bitmap comicImage = AForge.Imaging.Image.Clone(sourceImage);
			Bitmap edgeLayer = null;
			Bitmap glowLayer = null;

			// Glow for smooth colors
			GaussianBlur filterBlur = new GaussianBlur();
			filterBlur.Sigma = 2.0;
			filterBlur.Size = 4;
			glowLayer = filterBlur.Apply(comicImage);

			//SmartBlur filterBlur = new SmartBlur(10, 0.2);
			//glowLayer = filterBlur.Apply(comicImage);

			ContrastCorrection filterContrast = new ContrastCorrection(1 - (-this.Coloring * 0.1));
			filterContrast.ApplyInPlace(glowLayer);

			BrightnessCorrection filterBrightness = new BrightnessCorrection((-this.Coloring * 0.1) + 0.1);
			filterBrightness.ApplyInPlace(glowLayer);

			Screen blendScreen = new Screen(glowLayer);
			blendScreen.ApplyInPlace(comicImage);


			// Create a layer for edges
			Convolution filterConvolution = new Convolution(ConvolutionKernel);
			edgeLayer = filterConvolution.Apply(comicImage);

			// Convert to grayscale
			edgeLayer = convertGray.Apply(edgeLayer);

			// Threshold (edge thickness)
			Threshold filterThreshold = new Threshold((byte)(this.Edging * 255 / 100));
			filterThreshold.ApplyInPlace(edgeLayer);
			edgeLayer = convertColor.Apply(edgeLayer);

			// intersect comic with top layer (Darken blend)
			Intersect blendIntersect = new Intersect(edgeLayer);
			blendIntersect.ApplyInPlace(comicImage);

			return comicImage;
		}

		#endregion
	}
}