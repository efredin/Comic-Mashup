using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Fredin.Comic.Image.Filter
{
	public class SoftLight : Blend
	{
		public SoftLight(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public SoftLight(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte a, byte b)
		{
			int c = a * b >> 8;
			return (byte)(c + a * (255 - ((255 - a) * (255 - b) >> 8) - c) >> 8);
		}
	}
}
