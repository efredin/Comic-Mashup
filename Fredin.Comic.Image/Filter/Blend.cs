using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging.Filters;
using AForge.Imaging;

namespace Fredin.Comic.Image.Filter
{
	public abstract class Blend : BaseInPlaceFilter2
	{
		private Dictionary<PixelFormat, PixelFormat> _formatTransalations;
		public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
		{
			get { return this._formatTransalations; }
		}

		public Point OverlayPosition { get; set; }

		public Blend() 
		{
			this.InitFormatTranslations();
		}

		public Blend(Bitmap overlay)
		{
			this.OverlayImage = overlay;
			this.OverlayPosition = new Point(0, 0);
			this.InitFormatTranslations();
		}

		public Blend(Bitmap overlay, Point position)
		{
			this.OverlayImage = overlay;
			this.OverlayPosition = position;
			this.InitFormatTranslations();
		}

		public Blend(UnmanagedImage overlay)
		{
			this.UnmanagedOverlayImage = overlay;
			this.OverlayPosition = new Point(0, 0);
			this.InitFormatTranslations();
		}

		public Blend(UnmanagedImage overlay, Point position)
		{
			this.UnmanagedOverlayImage = overlay;
			this.OverlayPosition = position;
			this.InitFormatTranslations();
		}

		protected virtual void InitFormatTranslations()
		{
			this._formatTransalations = new Dictionary<PixelFormat, PixelFormat>();

			// All grayscale formats
			this.FormatTransalations.Add(PixelFormat.Format8bppIndexed, PixelFormat.Format8bppIndexed);
			this.FormatTransalations.Add(PixelFormat.Format16bppGrayScale, PixelFormat.Format16bppGrayScale);

			// All color formats
			this.FormatTransalations.Add(PixelFormat.Format16bppArgb1555, PixelFormat.Format16bppArgb1555);
			this.FormatTransalations.Add(PixelFormat.Format24bppRgb, PixelFormat.Format24bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppRgb, PixelFormat.Format32bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppArgb, PixelFormat.Format32bppArgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppPArgb, PixelFormat.Format32bppPArgb);
			this.FormatTransalations.Add(PixelFormat.Format48bppRgb, PixelFormat.Format48bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format4bppIndexed, PixelFormat.Format4bppIndexed);
			this.FormatTransalations.Add(PixelFormat.Format64bppArgb, PixelFormat.Format64bppArgb);
			this.FormatTransalations.Add(PixelFormat.Format64bppPArgb, PixelFormat.Format64bppPArgb);
		}

		protected abstract byte BlendFunction(byte a, byte b);

		protected override unsafe void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
		{
			// get image dimension
			int width = image.Width;
			int height = image.Height;

			// overlay position and dimension
			int ovrX = this.OverlayPosition.X;
			int ovrY = this.OverlayPosition.Y;
			int ovrW = overlay.Width;
			int ovrH = overlay.Height;

			// initialize other variables
			int pixelSize = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int offset = image.Stride - pixelSize * width;
			int ovrOffset, lineSize;

			// do the job
			byte* ptr = (byte*)image.ImageData.ToPointer();
			byte* ovr = (byte*)overlay.ImageData.ToPointer();

			if ((width == ovrW) && (height == ovrH) && (ovrX == 0) && (ovrY == 0))
			{
				// overlay image has the same size as the source image and its position is (0, 0)
				lineSize = width * pixelSize;

				// for each line
				for (int y = 0; y < height; y++)
				{
					// for each pixel
					for (int x = 0; x < lineSize; x++, ptr++, ovr++)
					{
						*ptr = this.BlendFunction(*ptr, *ovr);
					}
					ptr += offset;
					ovr += offset;
				}
			}
			else
			{
				// align Y
				if (ovrY >= 0)
				{
					ptr += image.Stride * ovrY;
				}
				else
				{
					ovr -= overlay.Stride * ovrY;
					ovrH += ovrY;
					ovrY = 0;
				}

				// align X
				if (ovrX >= 0)
				{
					ptr += pixelSize * ovrX;
				}
				else
				{
					ovr -= pixelSize * ovrX;
					ovrW += ovrX;
					ovrX = 0;
				}

				// update overlay width and height
				ovrW = Math.Min(ovrW, width - ovrX);
				ovrH = Math.Min(ovrH, height - ovrY);

				// update offset
				ovrOffset = overlay.Stride - ovrW * pixelSize;
				offset = image.Stride - ovrW * pixelSize;

				if ((ovrW > 0) && (ovrH > 0) && (ovrX < width) && (ovrY < height))
				{
					lineSize = pixelSize * ovrW;

					// for each line
					for (int y = 0; y < ovrH; y++)
					{
						// for each pixel
						for (int x = 0; x < lineSize; x++, ptr++, ovr++)
						{
							*ptr = this.BlendFunction(*ptr, *ovr);
						}
						ptr += offset;
						ovr += ovrOffset;
					}
				}
			}
		}
	}
}

#region [Formulas]

/*
private byte BlendDarken(ref byte ptr, ref byte ovr)
		{
			return ((ptr < ovr) ? ptr : ovr);
		}

		// Multiply
		private byte BlendMultiply(ref byte ptr, ref byte ovr)
		{
			return (byte)Math.Max(Math.Min((ptr / 255.0f * ovr / 255.0f) * 255.0f, 255), 0);
		}

		// Screen
		private byte BlendScreen(ref byte ptr, ref byte ovr)
		{
			return (byte)Math.Max(Math.Min(255 - ((255 - ptr) / 255.0f * (255 - ovr) / 255.0f) * 255.0f, 255), 0);
		}

		// Choose lightest color 
		private byte BlendLighten(ref byte ptr, ref byte ovr)
		{
			return ((ptr > ovr) ? ptr : ovr);
		}

		// hard light 
		private byte BlendHardLight(ref byte ptr, ref byte ovr)
		{
			return ((ptr < 128) ? (byte)Math.Max(Math.Min((ptr / 255.0f * ovr / 255.0f) * 255.0f * 2, 255), 0) : (byte)Math.Max(Math.Min(255 - ((255 - ptr) / 255.0f * (255 - ovr) / 255.0f) * 255.0f * 2, 255), 0));
		}

		// difference 
		private byte BlendDifference(ref byte ptr, ref byte ovr)
		{
			return (byte)((ptr > ovr) ? ptr - ovr : ovr - ptr);
		}

		// pin light 
		private byte BlendPinLight(ref byte ptr, ref byte ovr)
		{
			return (ptr < 128) ? ((ovr > ptr) ? ptr : ovr) : ((ovr < ptr) ? ptr : ovr);
		}

		// overlay 
		private byte BlendOverlay(ref byte ptr, ref byte ovr)
		{
			return ((ovr < 128) ? (byte)Math.Max(Math.Min((ptr / 255.0f * ovr / 255.0f) * 255.0f * 2, 255), 0) : (byte)Math.Max(Math.Min(255 - ((255 - ptr) / 255.0f * (255 - ovr) / 255.0f) * 255.0f * 2, 255), 0));
		}

		// exclusion 
		private byte BlendExclusion(ref byte ptr, ref byte ovr)
		{
			return (byte)(ptr + ovr - 2 * (ovr * ptr) / 255f);
		}

		// Soft Light (XFader formula)  
		private byte BlendSoftLight(ref byte ptr, ref byte ovr)
		{
			return (byte)Math.Max(Math.Min((ovr * ptr / 255f) + ovr * (255 - ((255 - ovr) * (255 - ptr) / 255f) - (ovr * ptr / 255f)) / 255f, 255), 0);
		}

		// Color Burn 
		private byte BlendColorBurn(ref byte ptr, ref byte ovr)
		{
			return (ptr == 0) ? (byte)0 : (byte)Math.Max(Math.Min(255 - (((255 - ovr) * 255) / ptr), 255), 0);
		}

		// Color Dodge 
		private byte BlendColorDodge(ref byte ptr, ref byte ovr)
		{
			return (ptr == 255) ? (byte)255 : (byte)Math.Max(Math.Min((ovr * 255) / (255 - ptr), 255), 0);
		}
*/

#endregion