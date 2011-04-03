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
	public class PencilSketch : ComicRenderBase
	{
		#region [Property]

		public Bitmap InputBitmap
		{
			get;
			private set;
		}

		#region [Config]

		public int PencilTipSize { get; set; }

		public double Range { get; set; }

		#endregion

		#endregion

		public PencilSketch()
		{
			this.PencilTipSize = 10;
			this.Range = 0;
		}

		#region [Method]

		public override List<RenderParameter> GetRenderParameters()
		{
			List<RenderParameter> renderParams = new List<RenderParameter>();

			renderParams.Add(new RenderParameter("pencilTipSize", "Edging", 10, 5, 15));
			renderParams.Add(new RenderParameter("range", "Coloring", 0, -3, 3));

			return renderParams;
		}

		public override void SetRenderParameterValues(Dictionary<string, object> values)
		{
			if (values != null)
			{
				if (values.ContainsKey("range"))
				{
					this.Range = (int)values["range"];
				}
				if (values.ContainsKey("pencilTipSize"))
				{
					this.PencilTipSize = (int)values["pencilTipSize"];
				}
			}
		}

		/// <summary>
		/// Creates a comic rendered copy of the input image.
		/// </summary>
		public override Bitmap Render(Bitmap sourceImage)
		{
			Bitmap sketchImage = AForge.Imaging.Image.Clone(sourceImage);

			GrayscaleY convertGray = new GrayscaleY();

			// Blur
			GaussianBlur filterBlur = new GaussianBlur();
			filterBlur.Sigma = this.PencilTipSize;
			filterBlur.Size = this.PencilTipSize;
			Bitmap overLayer = filterBlur.Apply(sketchImage);

			// Invert over layer
			Invert sketchInvert = new Invert();
			sketchInvert.ApplyInPlace(overLayer);

			BrightnessCorrection filterBrightness = new BrightnessCorrection(-this.Range * 0.01);
			filterBrightness.ApplyInPlace(overLayer);

			ContrastCorrection filterContrast = new ContrastCorrection(1 - (-this.Range * 0.01));
			filterContrast.ApplyInPlace(overLayer);

			// Convert to grayscale
			sketchImage = convertGray.Apply(sketchImage);
			overLayer = convertGray.Apply(overLayer);

			// Dodge blending for the win!
			ColorDodge dodgeBlend = new ColorDodge(overLayer);
			dodgeBlend.ApplyInPlace(sketchImage);

			return sketchImage;
		}

		#endregion
	}
}