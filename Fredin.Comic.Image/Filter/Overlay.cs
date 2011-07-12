using System;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public sealed class Overlay : Blend
	{
		public Overlay(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public Overlay(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte ptr, byte ovr)
		{
			return ((ovr < 128) ? (byte)Math.Max(Math.Min((ptr / 255.0f * ovr / 255.0f) * 255.0f * 2, 255), 0) : (byte)Math.Max(Math.Min(255 - ((255 - ptr) / 255.0f * (255 - ovr) / 255.0f) * 255.0f * 2, 255), 0));
		}
	}
}