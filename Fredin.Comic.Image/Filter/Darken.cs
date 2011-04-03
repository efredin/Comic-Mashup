using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Fredin.Comic.Image.Filter
{
	public class Darken : Blend
	{
		public Darken(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public Darken(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte a, byte b)
		{
			return ((a < b) ? a : b);
		}
	}
}
