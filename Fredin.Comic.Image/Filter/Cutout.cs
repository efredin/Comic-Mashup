using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public class Cutout : BaseInPlaceFilter
	{
		#region [Property]

		public int Interval { get; set; }

		private Dictionary<PixelFormat, PixelFormat> _formatTransalations;
		public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
		{
			get { return this._formatTransalations; }
		}

		#endregion

		public Cutout(int interval)
		{
			this.Interval = interval;

			this._formatTransalations = new Dictionary<PixelFormat, PixelFormat>();
			this.FormatTransalations.Add(PixelFormat.Format24bppRgb, PixelFormat.Format24bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppRgb, PixelFormat.Format32bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppArgb, PixelFormat.Format32bppArgb);
		}

		protected override unsafe void ProcessFilter(UnmanagedImage image)
		{
			int offset = (image.Stride - image.Width * 3);
			byte* src = (byte*)image.ImageData.ToPointer();

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++, src += 3)
				{
					src[RGB.R] = (byte)(src[RGB.R] - src[RGB.R] % this.Interval);
					src[RGB.G] = (byte)(src[RGB.G] - src[RGB.G] % this.Interval);
					src[RGB.B] = (byte)(src[RGB.B] - src[RGB.B] % this.Interval);
				}

				src += offset;
			}
		}
	}
}