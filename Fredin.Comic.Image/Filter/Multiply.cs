using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Fredin.Comic.Image.Filter
{
	public class Multiply : Blend
	{
		public Multiply(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public Multiply(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte a, byte b)
		{
			return (byte)((a * b) >> 8);
		}
	}
}
