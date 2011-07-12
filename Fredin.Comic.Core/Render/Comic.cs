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
			renderParams.Add(new RenderParameter("coloring", "Coloring", 12, 6, 18));

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
			HistogramEqualization autoLevels = new HistogramEqualization();

			// Convert grayscal images
			if (sourceImage.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				sourceImage = convertColor.Apply(sourceImage);
			}

			Bitmap smoothLayer = autoLevels.Apply(sourceImage);

			AdaptiveSmoothing adaptiveSmooth = new AdaptiveSmoothing(10);
			adaptiveSmooth.ApplyInPlace(smoothLayer);

			Bitmap grayscale = Grayscale.CommonAlgorithms.Y.Apply(smoothLayer);

			// Haven't been able to come up with a good edge detection for posterized comics.. Oh well
			SobelEdgeDetector sobelEdge = new SobelEdgeDetector();
			sobelEdge.ScaleIntensity = true;
			Bitmap edgeLayer = sobelEdge.Apply(grayscale);
			Threshold thresholdEdge = new Threshold(36);
			thresholdEdge.ApplyInPlace(edgeLayer);
			Invert invertEdge = new Invert();
			invertEdge.ApplyInPlace(edgeLayer);
			edgeLayer = convertColor.Apply(edgeLayer);

			ColorDodge dodgeEdge = new ColorDodge(edgeLayer);
			edgeLayer = dodgeEdge.Apply(smoothLayer);

			// Posterize
			Posterize posterColor = new Posterize(this.Coloring);
			Bitmap posterLayer = posterColor.Apply(smoothLayer);
			GaussianBlur gausPoster = new GaussianBlur(18);
			gausPoster.ApplyInPlace(posterLayer);
			Darken darkenPoster = new Darken(posterLayer);
			darkenPoster.ApplyInPlace(smoothLayer);

			// Merge edges with working layer
			Multiply multEdge = new Multiply(edgeLayer);
			multEdge.ApplyInPlace(smoothLayer);
			
			return smoothLayer;
		}

		#endregion
	}
}