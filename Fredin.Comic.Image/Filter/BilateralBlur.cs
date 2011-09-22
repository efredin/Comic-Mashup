using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public class BilateralBlur : BaseInPlaceFilter
	{
		#region [Property]

		private Dictionary<PixelFormat, PixelFormat> _formatTransalations;
		public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
		{
			get { return this._formatTransalations; }
		}

		public int Radius { get; set; }

		public double Sigma { get; set; }

		#endregion

		public BilateralBlur(int radius, double sigma)
		{
			this.Radius = radius;
			this.Sigma = sigma;

			this._formatTransalations = new Dictionary<PixelFormat, PixelFormat>();
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

		protected override unsafe void ProcessFilter(UnmanagedImage image)
		{
			double[,] gauss = new double[this.Radius * 2 + 1, this.Radius * 2 + 1];
			for (int y = -this.Radius; y <= this.Radius; y++)
			{
				for (int x = -this.Radius; x <= this.Radius; x++)
				{
					gauss[x + this.Radius, y + this.Radius] = Math.Exp(-(Math.Pow(x, 2) + Math.Pow(y, 2)) / Math.Pow(Convert.ToDouble(2 * this.Radius), 2));
				}
			}

			double sigma = this.Sigma * 100;
		
			int offset = (image.Stride - (image.Width * 3));
			byte* src = (byte*)image.ImageData.ToPointer();
			byte* pixel = src;

			for (int y = 0; y < image.Height; y++, pixel += offset)
			{
				for (int x = 0; x < image.Width; x++, pixel += 3)
				{
					double r = 0.0, g = 0.0, b = 0.0;
					double totalWeight = 0.0;
					for (int v = -this.Radius; v < this.Radius; v++)
					{
						for (int u = -this.Radius; u < this.Radius; u++)
						{
							int ry = v + y;
							int rx = u + x;

							// image boundary check
							if (rx >= 0 && rx < image.Width && ry >= 0 && ry < image.Height)
							{
								byte* pixelR = src + (ry * image.Stride) + (rx * 3);
								double diff = Math.Exp( - (Math.Pow(pixel[RGB.R] - pixelR[RGB.R], 2) + Math.Pow(pixel[RGB.G] - pixelR[RGB.G], 2) + Math.Pow(pixel[RGB.B] - pixelR[RGB.B], 2)) / Math.Pow(2 * sigma, 2));
								double weight = diff * gauss[u + this.Radius, v + this.Radius];

								r += pixelR[RGB.R] * weight;
								g += pixelR[RGB.G] * weight;
								b += pixelR[RGB.B] * weight;
								totalWeight += weight;
							}
						}
					}

					if (totalWeight != 0)
					{
						pixel[RGB.R] = Convert.ToByte(r / totalWeight);
						pixel[RGB.G] = Convert.ToByte(g / totalWeight);
						pixel[RGB.B] = Convert.ToByte(b / totalWeight);
					}
				}
			}
		}
	}
}