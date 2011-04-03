using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Fredin.Comic.Image.Filter
{
	public class SmartBlur : BaseInPlaceFilter
	{
		#region [Property]

		private Dictionary<PixelFormat, PixelFormat> _formatTransalations;
		public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
		{
			get { return this._formatTransalations; }
		}

		public int Radius { get; set; }

		public double Strength { get; set; }

		#endregion

		public SmartBlur(int radius, double strength)
		{
			this.Radius = radius;
			this.Strength = strength;

			this._formatTransalations = new Dictionary<PixelFormat, PixelFormat>();
			this.FormatTransalations.Add(PixelFormat.Format16bppArgb1555, PixelFormat.Format16bppArgb1555);
			this.FormatTransalations.Add(PixelFormat.Format24bppRgb, PixelFormat.Format24bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppRgb, PixelFormat.Format32bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppArgb, PixelFormat.Format32bppArgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppPArgb, PixelFormat.Format32bppPArgb);
			this.FormatTransalations.Add(PixelFormat.Format48bppRgb, PixelFormat.Format48bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format4bppIndexed, PixelFormat.Format4bppIndexed);
			this.FormatTransalations.Add(PixelFormat.Format64bppArgb, PixelFormat.Format64bppArgb);
			this.FormatTransalations.Add(PixelFormat.Format64bppPArgb, PixelFormat.Format64bppPArgb);
		}

		private unsafe int DistSquare(byte* center, byte* k)
		{
			int distSquare;
			int distComp;
			distComp = k[RGB.B] - center[RGB.B];
			distSquare = distComp * distComp;
			distComp = k[RGB.G] - center[RGB.G];
			distSquare += distComp * distComp;
			distComp = k[RGB.R] - center[RGB.R];
			distSquare += distComp * distComp;
			return distSquare;
		}

		protected override unsafe void ProcessFilter(UnmanagedImage image)
		{
			int strengthi = (int)(this.Strength * this.Strength * (255 * 255 * 3));
			int radiusSqr = (this.Radius * this.Radius);

			//IntPtr scan0 = image.ImageData;
			//int stride = image.Stride;
			int offset = (image.Stride - image.Width * 3);
			byte* src = (byte*)image.ImageData.ToPointer();

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++, src += 3)
				{
					int count = 0;
					int sumb = 0;
					int sumg = 0;
					int sumr = 0;
					for (int i = -this.Radius; i <= this.Radius; i++)
					{
						int ypi = y + i;
						if (ypi >= 0 && ypi < image.Height)
						{
							// y - 1
							byte* k = src + (image.Stride * i);
							if (this.DistSquare(src, k) <= strengthi)
							{
								sumb += k[RGB.B];
								sumg += k[RGB.G];
								sumr += k[RGB.R];
								count++;
							}

							int dist = i * i;
							for (int j = 1; (dist = dist + (2 * j) - 1) <= radiusSqr; j++)
							{
								if (x - j >= 0)
								{
									byte* cmj = k - (j * 3);
									if (this.DistSquare(src, cmj) <= strengthi)
									{
										sumb += cmj[RGB.B];
										sumg += cmj[RGB.G];
										sumr += cmj[RGB.R];
										count++;
									}
								}

								if (x + j < image.Width)
								{
									byte* cpj = k + (j * 3);
									if (this.DistSquare(src, cpj) <= strengthi)
									{
										sumb += cpj[RGB.B];
										sumg += cpj[RGB.G];
										sumr += cpj[RGB.R];
										count++;
									}
								}
							}
						}
					}

					int countdiv2 = (int)((uint)count / 2);

					src[RGB.R] = (byte)((sumr + countdiv2) / count);
					src[RGB.G] = (byte)((sumg + countdiv2) / count);
					src[RGB.B] = (byte)((sumb + countdiv2) / count);
				}

				src += offset;
			}
		}
	}
}

/*
			return (x, y) =>
            {
                int count = 0;
                int sumb = 0;
                int sumg = 0;
                int sumr = 0;
                int suma = 0;
                ColorBgra center = source(x, y);
                for (int i = -radiusFloor; i <= radiusFloor; i++)
                {
                    int ypi = y + i;
                    ColorBgra c = source(x, ypi);
                    if (DistSquare(center, c) <= strengthi)
                    {
                        sumb += c.B;
                        sumg += c.G;
                        sumr += c.R;
                        suma += c.A;
                        count++;
                    }

                    int dist = i * i;
                    for (int j = 1; (dist = dist + (2 * j) - 1) <= radiusSqr; j++)
                    {
                        c = source(x - j, ypi);
                        if (DistSquare(center, c) <= strengthi)
                        {
                            sumb += c.B;
                            sumg += c.G;
                            sumr += c.R;
                            suma += c.A;
                            count++;
                        }

                        c = source(x + j, ypi);
                        if (DistSquare(center, c) <= strengthi)
                        {
                            sumb += c.B;
                            sumg += c.G;
                            sumr += c.R;
                            suma += c.A;
                            count++;
                        }
                    }
                }

                Debug.Assert(count >= 0);

                // cast count to a uint for shr instruction generation
                int countdiv2 = (int)((uint)count / 2);

                return new ColorBgra
                {
                    B = (byte)((sumb + countdiv2) / count),
                    G = (byte)((sumg + countdiv2) / count),
                    R = (byte)((sumr + countdiv2) / count),
                    A = (byte)((suma + countdiv2) / count)
                };
            };
*/