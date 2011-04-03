using System;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public sealed class ColorDodge : Blend
	{
		public ColorDodge(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public ColorDodge(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte a, byte b)
		{
			return (b == 255) ? (byte)255 : (byte)Math.Max(Math.Min((a << 8) / (255 - b), 255), 0);
		}
	}
}