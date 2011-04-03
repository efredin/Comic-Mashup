using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;

namespace Fredin.Comic.Image.Filter
{
//    public sealed class Bilateral : IFilter, IInPlaceFilter, IInPlacePartialFilter
//    {
//        private Convolution Filter { get; set; }

//        public double Sigma { get; set; }

//        public Bilateral(double sigma)
//        {
//            this.Sigma = sigma;
//            this.CreateFilter();
//        }

//        public Bitmap Apply(Bitmap image)
//        {
//            return this.Filter.Apply(image);
//        }

//        public Bitmap Apply(BitmapData imageData)
//        {
//            return this.Filter.Apply(imageData);
//        }

//        public void ApplyInPlace(Bitmap image)
//        {
//            this.Filter.ApplyInPlace(image);
//        }

//        public void ApplyInPlace(BitmapData imageData)
//        {
//            this.Filter.ApplyInPlace(imageData);
//        }

//        public void ApplyInPlace(Bitmap image, Rectangle rect)
//        {
//            this.Filter.ApplyInPlace(image, rect);
//        }

//        public void ApplyInPlace(BitmapData imageData, Rectangle rect)
//        {
//            this.Filter.ApplyInPlace(imageData, rect);
//        }

//        private void CreateFilter()
//        {
//            int size = (int)Math.Ceiling(4 * this.Sigma) + 1;

//            Gaussian gaus = new Gaussian(this.Sigma);
//            int[,] kernel = gaus.KernelDiscret2D(size);
//            this.Filter = new Convolution(kernel);
//        }
//    }
//}

	public class Bilateral : FilterAnyToAnyUsingCopyPartial
	{
		private int Divisor { get; set; }
		private int Size { get; set; }
		private double SigmaD { get; set; }
		private double SigmaR { get; set; }
		private double[,] KernelD { get; set; }
		private double[,] KernelR { get; set; }
		private double[] GaussSimilarity { get; set; }
		private double TwoSigmaRSquared { get; set; }

		public Bilateral(double sigmaD, double sigmaR)
		{
			// compute the necessary kernel radius from the maximum of both sigma values
			double sigmaMax = Math.Max(sigmaD, sigmaR);
			int kernelRadius = (int)Math.Ceiling(2 * sigmaMax);

			this.SigmaD = sigmaD;
			this.SigmaR = sigmaR;

			this.TwoSigmaRSquared = 2 * this.SigmaR * this.SigmaR;

			this.Size = kernelRadius * 2 + 1;
			int center = (this.Size - 1) / 2;

			Gaussian gauss = new Gaussian(sigmaD);
			this.KernelD = gauss.Kernel2D(this.Size);

			for (int x = -center; x < -center + this.Size; x++) 
			{
				for (int y = -center; y < -center + this.Size; y++)
				{
					this.KernelD[x + center, y + center] = gauss.Function2D(x, y);
				}
			}

			// precomute all possible similarity values for performance resaons
			this.GaussSimilarity = new double[256];
			for (int i = 0; i < 256; i++)
			{
				this.GaussSimilarity[i] = Math.Exp(-((i) / this.TwoSigmaRSquared));
			}
		}

		protected override unsafe void ProcessFilter(IntPtr sourceData, BitmapData destinationData, Rectangle rect)
		{
			int pixelSize = (destinationData.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;

			// processing start and stop X,Y positions
			int startX = rect.Left;
			int startY = rect.Top;
			int stopX = startX + rect.Width;
			int stopY = startY + rect.Height;

			int stride = destinationData.Stride;
			int offset = stride - rect.Width * pixelSize;

			// loop and array indexes
			int i, j, t, k, ir, jr;
			// kernel's radius
			int radius = this.Size >> 1;
			// color sums
			long r, g, b, div;

			// kernel size
			int kernelSize = this.Size * this.Size;
			// number of kernel elements taken into account
			int processedKernelSize;

			byte* src = (byte*)sourceData.ToPointer();
			byte* dst = (byte*)destinationData.Scan0.ToPointer();
			byte* p;

			// allign pointers to the first pixel to process
			src += (startY * stride + startX * pixelSize);
			dst += (startY * stride + startX * pixelSize);

			// do the processing job
			if (destinationData.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				// grayscale image

				// for each line
				for (int y = startY; y < stopY; y++)
				{
					// for each pixel
					for (int x = startX; x < stopX; x++, src++, dst++)
					{
						g = div = processedKernelSize = 0;

						// for each kernel row
						for (i = 0; i < this.Size; i++)
						{
							ir = i - radius;
							t = y + ir;

							// skip row
							if (t < startY)
								continue;
							// break
							if (t >= stopY)
								break;

							// for each kernel column
							for (j = 0; j < this.Size; j++)
							{
								jr = j - radius;
								t = x + jr;

								// skip column
								if (t < startX)
									continue;

								if (t < stopX)
								{
									k = kernel[i, j];

									div += k;
									g += k * src[ir * stride + jr];
									processedKernelSize++;
								}
							}
						}

						// check if all kernel elements were processed
						if (processedKernelSize == kernelSize)
						{
							// all kernel elements are processed - we are not on the edge
							div = divisor;
						}
						else
						{
							// we are on edge. do we need to use dynamic divisor or not?
							if (!dynamicDivisorForEdges)
							{
								// do
								div = divisor;
							}
						}

						// check divider
						if (div != 0)
						{
							g /= div;
						}
						*dst = (g > 255) ? (byte)255 : ((g < 0) ? (byte)0 : (byte)g);
					}
					src += offset;
					dst += offset;
				}
			}
			else
			{
				// RGB image

				// for each line
				for (int y = startY; y < stopY; y++)
				{
					// for each pixel
					for (int x = startX; x < stopX; x++, src += 3, dst += 3)
					{
						r = g = b = div = processedKernelSize = 0;

						// for each kernel row
						for (i = 0; i < size; i++)
						{
							ir = i - radius;
							t = y + ir;

							// skip row
							if (t < startY)
								continue;
							// break
							if (t >= stopY)
								break;

							// for each kernel column
							for (j = 0; j < size; j++)
							{
								jr = j - radius;
								t = x + jr;

								// skip column
								if (t < startX)
									continue;

								if (t < stopX)
								{
									k = kernel[i, j];
									p = &src[ir * stride + jr * 3];

									div += k;

									r += k * p[RGB.R];
									g += k * p[RGB.G];
									b += k * p[RGB.B];

									processedKernelSize++;
								}
							}
						}

						// check if all kernel elements were processed
						if (processedKernelSize == kernelSize)
						{
							// all kernel elements are processed - we are not on the edge
							div = divisor;
						}
						else
						{
							// we are on edge. do we need to use dynamic divisor or not?
							if (!dynamicDivisorForEdges)
							{
								// do
								div = divisor;
							}
						}

						// check divider
						if (div != 0)
						{
							r /= div;
							g /= div;
							b /= div;
						}
						dst[RGB.R] = (r > 255) ? (byte)255 : ((r < 0) ? (byte)0 : (byte)r);
						dst[RGB.G] = (g > 255) ? (byte)255 : ((g < 0) ? (byte)0 : (byte)g);
						dst[RGB.B] = (b > 255) ? (byte)255 : ((b < 0) ? (byte)0 : (byte)b);
					}
					src += offset;
					dst += offset;
				}
			}
		}
	}
}		
			
////public class Bilateral extends ConvolutionFunction {

////    double sigmaD;
////    double sigmaR;

////    double[][] kernelD;
////    double[][] kernelR;

////    double gaussSimilarity[];

////    double twoSigmaRSquared;

////    public Bilateral(double sigmaD, double sigmaR) {
////        super(0);

////        // compute the necessary kernel radius from the maximum
////        // of both sigma values
////        double sigmaMax = Math.max(sigmaD, sigmaR);
////        this.kernelRadius = (int)Math.ceil(2 * sigmaMax);

////        this.sigmaD = sigmaD;
////        this.sigmaR = sigmaR;

////        this.twoSigmaRSquared = 2 * this.sigmaR * this.sigmaR;

////        // this will always be an odd number, i.e. {1,3,5,7,9,...}
////        int kernelSize = kernelRadius * 2 + 1;
////        int center = (kernelSize - 1) / 2;

////        System.out.println("Applying Bilateral Filter with sigmaD = " + sigmaD + ", sigmaR = " + sigmaR + " and kernelRadius " + kernelRadius);

////        this.kernelD = new double[kernelSize][kernelSize];

////        for (int x = -center; x < -center + kernelSize; x++) {
////            for (int y = -center; y < -center + kernelSize; y++) {
////                kernelD[x + center][y + center] = this.gauss(sigmaD, x, y);
////            }
////        }

////        // precomute all possible similarity values for
////        // performance resaons
////        this.gaussSimilarity = new double[256];
////        for (int i = 0; i < 256; i++) {
////            this.gaussSimilarity[i] = Math.exp(-((i) / this.twoSigmaRSquared));
////        }
////    }

////    public int apply(int i, int j) {

////        double sum = 0;
////        double totalWeight = 0;
////        int intensityCenter = this.pixels[i][j];

////        int mMax = i + kernelRadius;
////        int nMax = j + kernelRadius;
////        double weight;

////        for (int m = i-kernelRadius; m < mMax; m++) {
////            for (int n = j-kernelRadius; n < nMax; n++) {

////                if (this.isInsideBoundaries(m, n)) {
////                    int intensityKernelPos = this.pixels[m][n];
////                    weight = kernelD[i-m + kernelRadius][j-n + kernelRadius] * this.similarity(intensityKernelPos,intensityCenter);
////                    totalWeight += weight;
////                    sum += (weight * intensityKernelPos);
////                }
////            }
////        }
////        return (int)Math.floor(sum / totalWeight);
////    }

////    private double similarity(int p, int s) {
////        // this equals: Math.exp(-(( Math.abs(p-s)) /  2 * this.sigmaR * this.sigmaR));
////        // but is precomputed to improve performance
////        return this.gaussSimilarity[Math.abs(p-s)];
////    }


////    private double gauss (double sigma, int x, int y) {
////        return Math.exp(-((x * x + y * y) / (2 * sigma * sigma)));
////    }
////}
