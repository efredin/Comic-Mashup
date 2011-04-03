using System;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public sealed class ColorBurn : Blend
	{
		public ColorBurn(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public ColorBurn(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte ptr, byte ovr)
		{
			return (ptr == 0) ? (byte)0 : (byte)Math.Max(Math.Min(255 - (((255 - ovr) * 255) / ptr), 255), 0);
		}
	}
}