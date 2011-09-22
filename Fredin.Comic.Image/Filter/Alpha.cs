using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging.Filters;
using AForge.Imaging;


namespace Fredin.Comic.Image.Filter
{
	public class Alpha : BaseInPlaceFilter
	{
		private Dictionary<PixelFormat, PixelFormat> _formatTransalations;
		public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
		{
			get { return this._formatTransalations; }
		}

		public byte Value { get; set; }

		public Alpha(byte value)
		{
			this.Value = value;

			this._formatTransalations = new Dictionary<PixelFormat, PixelFormat>();
			this.FormatTransalations.Add(PixelFormat.Format32bppArgb, PixelFormat.Format32bppArgb);
		}

		protected override unsafe void ProcessFilter(UnmanagedImage image)
		{
			int offset = (image.Stride - image.Width * 4);
			byte* ptr = (byte*)image.ImageData.ToPointer();
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++, ptr += 4)
				{
					ptr[3] = this.Value;
				}

				ptr += offset;
			}
		}
	}
}
