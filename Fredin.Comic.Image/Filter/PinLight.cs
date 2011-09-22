﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Fredin.Comic.Image.Filter
{
	public class PinLight : Blend
	{
		public PinLight(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public PinLight(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte a, byte b)
		{
			return (b < 128 ) ? (byte)((a * b) >> 7) : (byte)(255 - ((255 - b) * (255 - a) >> 7));
		}
	}
}
