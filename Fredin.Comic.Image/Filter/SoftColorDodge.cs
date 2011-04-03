using System;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public sealed class SoftColorDodge : Blend
	{
		public SoftColorDodge(Bitmap overlayImage)
			: base(overlayImage)
		{
		}

		public SoftColorDodge(Bitmap overlayImage, Point position)
			: base(overlayImage, position)
		{
		}

		protected override byte BlendFunction(byte a, byte b)
		{
			byte result;
			if (a + b < 256)
			{
				if (b == 255)
				{
					result = 255;
				}
				else
				{
					result = Math.Min((byte)((a << 7) / (255 - b)), (byte)255);
				}
			}
			else
			{
				if (a == 0)
				{
					result = 0;
				}
				else
				{
					result = Math.Max((byte)(((255 - b) << 7) / a), (byte)0);
				}
			}
			return result;
		}
	}
}