using System;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public sealed class Screen : Blend
	{
		public Screen(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public Screen(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte ptr, byte ovr)
		{
			return (byte)Math.Max(Math.Min(255 - ((255 - ptr) / 255.0f * (255 - ovr) / 255.0f) * 255.0f, 255), 0);
		}
	}
}
