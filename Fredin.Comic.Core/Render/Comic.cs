using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Fredin.Comic.Image.Filter;
using log4net;

namespace Fredin.Comic.Render
{
	public class Comic : ComicRenderBase
	{
		#region [Property]

		public int Edging { get; set; }
		public int Coloring { get; set; }
		public int Shading { get; set; }

		#endregion

		public Comic()
		{
			this.Edging = 3;
			this.Coloring = 5;
			this.Shading = 65;
		}

		#region [Method]

		public override List<RenderParameter> GetRenderParameters()
		{
			List<RenderParameter> renderParams = new List<RenderParameter>();

			renderParams.Add(new RenderParameter("edging", "Edging", 3, 1, 5));
			renderParams.Add(new RenderParameter("coloring", "Coloring", 8, 16, 24));

			return renderParams;
		}

		public override void SetRenderParameterValues(Dictionary<string, object> values)
		{
			if (values != null)
			{
				if (values.ContainsKey("edging"))
				{
					this.Edging = Convert.ToInt32(values["edging"]);
				}
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
			Bitmap smoothLayer;
			Bitmap edgeLayer;
			Bitmap posterLayer;
			Bitmap noiseLayer;
			Bitmap grayscale;

			GrayscaleToRGB convertColor = new GrayscaleToRGB();

			// Convert grayscal images
			if (sourceImage.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				sourceImage = convertColor.Apply(sourceImage);
			}

			//HistogramEqualization autoLevels = new HistogramEqualization();
			//smoothLayer = autoLevels.Apply(sourceImage);

			AdaptiveSmoothing smooth = new AdaptiveSmoothing(10);
			smoothLayer = smooth.Apply(sourceImage);

			

			grayscale = Grayscale.CommonAlgorithms.Y.Apply(smoothLayer);

			// Edges
			SobelEdgeDetector sobelEdge = new SobelEdgeDetector();
			sobelEdge.ScaleIntensity = true;
			edgeLayer = sobelEdge.Apply(grayscale);
			Invert invertEdge = new Invert();
			invertEdge.ApplyInPlace(edgeLayer);
			Threshold thresholdEdge = new Threshold(128);
			thresholdEdge.ApplyInPlace(edgeLayer);
			edgeLayer = convertColor.Apply(edgeLayer);
			smooth.ApplyInPlace(edgeLayer);

			// Noise
			//FloydSteinbergDithering noiseDither = new FloydSteinbergDithering();
			//noiseLayer = noiseDither.Apply(grayscale);
			//noiseLayer = convertColor.Apply(noiseLayer);
			//Darken darkenNoise = new Darken(noiseLayer);
			//darkenNoise.ApplyInPlace(noiseLayer);

			// Posterize
			Posterize posterColor = new Posterize(16);
			posterLayer = posterColor.Apply(smoothLayer);
			Darken darkenPoster = new Darken(posterLayer);
			darkenPoster.ApplyInPlace(smoothLayer);

			Multiply multEdge = new Multiply(edgeLayer);
			multEdge.ApplyInPlace(smoothLayer);

			return smoothLayer;
		}

		#endregion
	}
}