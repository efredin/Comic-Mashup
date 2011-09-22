using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public class Posterize : BaseInPlaceFilter
	{
		#region [Property]

		public int Value { get; set; }

		private Dictionary<PixelFormat, PixelFormat> _formatTransalations;
		public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
		{
			get { return this._formatTransalations; }
		}

		#endregion

		public Posterize(int value)
		{
			this.Value = value;

			this._formatTransalations = new Dictionary<PixelFormat, PixelFormat>();
			this.FormatTransalations.Add(PixelFormat.Format24bppRgb, PixelFormat.Format24bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppRgb, PixelFormat.Format32bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format48bppRgb, PixelFormat.Format48bppRgb);
		}

		protected override unsafe void ProcessFilter(UnmanagedImage image)
		{
			//IntPtr scan0 = image.Scan0;
			//int stride = image.Stride;
			int offset = (image.Stride - image.Width * 3);

			int factor = 100 / this.Value;

			byte* ptr = (byte*)image.ImageData.ToPointer();
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++, ptr += 3)
				{
					Color color = Color.FromArgb(ptr[RGB.R], ptr[RGB.G], ptr[RGB.B]);
					HsvColor hsv = HsvColor.FromColor(color);

					// Posterize value
					hsv.Value -= (hsv.Value % factor);

					// Convert back to RGB
					color = hsv.ToColor();

					ptr[RGB.R] = (byte)color.R;
					ptr[RGB.G] = (byte)color.G;
					ptr[RGB.B] = (byte)color.B;
				}

				ptr += offset;
			}
		}
	}
}