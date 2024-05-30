using System;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Collections.Immutable;
using System.Numerics;
using System.Collections;
using System.Text;

namespace MMS_Slika
{
    internal static class Filters
    {
        private static bool IsPixelExist(int xCoord, int yCoord, int bmpWidth, int bmpHeight)
        {
            return xCoord >= 0 && xCoord < bmpHeight && yCoord >= 0 && yCoord < bmpWidth;
        }

        public static int XY2Counter(int x, int y, int stride)
        {
            int counter = x * stride + y * 3;
            // counter *= 3;
            return counter;
        }

        #region Invert

        public static Bitmap Invert(Bitmap bmp) {
            Bitmap filteredBmp = (Bitmap)bmp.Clone();

            var bmpData = filteredBmp.LockBits(
                new Rectangle(0, 0, filteredBmp.Width, filteredBmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                filteredBmp.PixelFormat
            );

            IntPtr ptr = bmpData.Scan0;
            int bytes  = Math.Abs(bmpData.Stride) * filteredBmp.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);

            for(int i = 0; i < rgbValues.Length; i++) {
                rgbValues[i] = (byte)((255) - rgbValues[i]);
            }

            Marshal.Copy(rgbValues, 0, ptr, bytes);

            filteredBmp.UnlockBits(bmpData);
            return filteredBmp;
        }

        #endregion

        #region Kuwahara

        public static Bitmap Kuwahara(Bitmap bmp, int a)
        {
            Bitmap filteredBmp = (Bitmap)bmp.Clone();

            var bmpData = filteredBmp.LockBits(
                new Rectangle(0, 0, filteredBmp.Width, filteredBmp.Height),
                ImageLockMode.ReadOnly,
                filteredBmp.PixelFormat
            );

            IntPtr ptr = bmpData.Scan0;
            int stride = Math.Abs(bmpData.Stride);
            int bytes  = stride * filteredBmp.Height;
            byte[] rgbValuesOld = new byte[bytes];
            byte[] rgbValuesNew = new byte[bytes];

            Marshal.Copy(ptr, rgbValuesOld, 0, bytes);

            Parallel.For(0, bmpData.Width * bmpData.Height, index => {
                int i = index / bmpData.Width;
                int j = index % bmpData.Width;
                if(i == 0 && j == 960) {
                    ;
                }
                (int, int) regionWithMinVariance = GetRegionWithMinVarinace(rgbValuesOld, bmpData.Width, bmpData.Height, i, j, stride, a);
                Color color = GetMeanColor4Region(rgbValuesOld, bmpData.Width, bmpData.Height, regionWithMinVariance.Item1, regionWithMinVariance.Item2, stride, a + 1);
                SetColor(rgbValuesNew, i, j, stride, color);
            });

            Marshal.Copy(rgbValuesNew, 0, ptr, bytes);

            filteredBmp.UnlockBits(bmpData);
            return filteredBmp;
        }

        private static float GetValue4Pixel(byte[] rgbValues, int x, int y, int stride)
        {
            int counter = XY2Counter(x, y, stride);
            float B = rgbValues[counter] / 255.0f;
            float G = rgbValues[counter + 1] / 255.0f;
            float R = rgbValues[counter + 2] / 255.0f;
            float V = Math.Max(Math.Max(R, G), B);
            return V;
        }

        private static float GetMeanValue4Region(byte[] rgbValues, int bmpWidth, int bmpHeight, int x, int y, int stride, int a)
        {
            float sum = 0.0f;
            float numOfPixels = 0.0f;
            for(int i = 0; i < a; i++) {
                for(int j = 0; j < a; j++) {
                    int xCoord = x + i;
                    int yCoord = y + j;
                    if(IsPixelExist(xCoord, yCoord, bmpWidth, bmpHeight)) {
                        sum += GetValue4Pixel(rgbValues, x + i, y + j, stride);
                        numOfPixels++;
                    }
                }
            }
            sum /= numOfPixels;
            return sum;
        }

        private static float GetVariance4Region(byte[] rgbValues, int bmpWidth, int bmpHeight, int x, int y, int stride, int a)
        {
            float mean = GetMeanValue4Region(rgbValues, bmpWidth, bmpHeight, x, y, stride, a);
            float variance = 0.0f;
            float numOfPixels = 0.0f;
            for(int i = 0; i < a; i++) {
                for(int j = 0; j < a; j++) {
                    int xCoord = x + i;
                    int yCoord = y + j;
                    if(IsPixelExist(xCoord, yCoord, bmpWidth, bmpHeight)) {
                        float v = GetValue4Pixel(rgbValues, i, j, stride);
                        variance += (v - mean) * (v - mean);
                        numOfPixels++;
                    }
                }
            }
            variance /= numOfPixels;
            variance = (float)Math.Sqrt(variance);
            return variance;
        }

        private static (int, int) GetRegionWithMinVarinace(byte[] rgbValues, int bmpWidth, int bmpHeight, int i, int j, int stride, int a)
        {
            return (new (int, int)[]{
                (i - a, j - a),
                (i - a, j),
                (i, j - a),
                (i, j)
            })
            .Select(tlPX => new {
                Region = tlPX,
                Variance = GetVariance4Region(rgbValues, bmpWidth, bmpHeight, tlPX.Item1, tlPX.Item2, stride, a + 1)
            })
            .MinBy(o => o.Variance)!
            .Region;
        }

        private static Color GetMeanColor4Region(byte[] rgbValues, int bmpWidth, int bmpHeight, int x, int y, int stride, int a)
        {
            float B = 0;
            float G = 0;
            float R = 0;
            float numOfPixels = 0.0f;
            for(int i = 0; i < a; i++) {
                for(int j = 0; j < a; j++) {
                    int xCoord = x + i;
                    int yCoord = y + j;
                    if (IsPixelExist(xCoord, yCoord, bmpWidth, bmpHeight)) {
                        int counter = XY2Counter(xCoord, yCoord, stride);
                        B += rgbValues[counter];
                        G += rgbValues[counter + 1];
                        R += rgbValues[counter + 2];
                        numOfPixels++;

                    }
                }
            }
            B /= numOfPixels;
            G /= numOfPixels;
            R /= numOfPixels;
            return Color.FromArgb((int)R, (int)G, (int)B);
        }

        private static void SetColor(byte[] rgbValues, int x, int y, int stride, Color color)
        {
            int counter = XY2Counter(x, y, stride);
            rgbValues[counter] = color.B;
            rgbValues[counter + 1] = color.G;
            rgbValues[counter + 2] = color.R;
        }

        #endregion

        #region Burkes Dithering

        public static Bitmap BurkesDithering(Form1 form1, Bitmap bmp)
        {
            Bitmap filteredBmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format8bppIndexed);

            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite,
                bmp.PixelFormat
            );
            
            IntPtr ptrBmp = bmpData.Scan0;
            int bytesBmp  = Math.Abs(bmpData.Stride) * bmpData.Height;
            byte[] rgbValuesBmp = new byte[bytesBmp];
            Marshal.Copy(ptrBmp, rgbValuesBmp, 0, bytesBmp);
            int strideBmp = Math.Abs(bmpData.Stride);

            var bmpFilteredData = filteredBmp.LockBits(
                new Rectangle(0, 0, filteredBmp.Width, filteredBmp.Height),
                ImageLockMode.ReadWrite,
                filteredBmp.PixelFormat
            );

            IntPtr ptrBmpFiltered = bmpFilteredData.Scan0;
            int bytesBmpFiltered  = Math.Abs(bmpFilteredData.Stride) * filteredBmp.Height;
            byte[] rgbValuesBmpFiltered = new byte[bytesBmpFiltered];
            int strideBmpFiltered = Math.Abs(bmpFilteredData.Stride);

            for(int i = 0; i < bmp.Height; i++) {
                for(int j = 0; j < bmp.Width; j++) {
                    int counter = XY2Counter(i, j, strideBmp);
                    byte oldB = rgbValuesBmp[counter];
                    byte oldG = rgbValuesBmp[counter + 1];
                    byte oldR = rgbValuesBmp[counter + 2];
                    Color oldColor = Color.FromArgb(oldR, oldG, oldB);
                    byte colorIndex = FindNearestColorIndex(filteredBmp, oldColor);
                    int counterFiltered = i * strideBmpFiltered + j;
                    rgbValuesBmpFiltered[counterFiltered] = colorIndex;
                    SetColors2Positions(rgbValuesBmp, i, j, bmp.Height, strideBmp, oldColor, filteredBmp.Palette.Entries[colorIndex]);
                }
            }

            bmp.UnlockBits(bmpData);
            Marshal.Copy(rgbValuesBmpFiltered, 0, ptrBmpFiltered, bytesBmpFiltered);
            filteredBmp.UnlockBits(bmpFilteredData);

            return filteredBmp;
        }

        private static byte FindNearestColorIndex(Bitmap bmp, Color oldColor)
        {
            byte indexR = ColorIndex(oldColor.R);
            byte indexG = ColorIndex(oldColor.G);
            byte indexB = ColorIndex(oldColor.B);

            byte colorIndex = (byte)(indexR * 36 + indexG * 6 + indexB);
            //Color newColor = bmp.Palette.Entries[colorIndex];
            //int oldColorDiff = 
            //    Math.Abs(oldColor.R - newColor.R) + 
            //    Math.Abs(oldColor.G - newColor.G) + 
            //    Math.Abs(oldColor.B - newColor.B);

            //for(int i = 6 * 6 * 6; i < bmp.Palette.Entries.Length; i++) {
            //   Color currColor = bmp.Palette.Entries[i];
            //    int currColorDiff = Math.Abs(currColor.R - oldColor.R) + Math.Abs(currColor.G - oldColor.G) + Math.Abs(currColor.B - oldColor.B);
            //    if(currColorDiff < oldColorDiff) {
            //        newColor = currColor;
            //        oldColorDiff =
            //            Math.Abs(oldColor.R - newColor.R) +
            //            Math.Abs(oldColor.G - newColor.G) +
            //            Math.Abs(oldColor.B - newColor.B);
            //        colorIndex = (byte)i;
            //    }
            //}

            return colorIndex;
        }

        private static byte ColorIndex(int oldColor)
        {
            int index = oldColor / 51;
            int down = oldColor - index * 51;
            int up = Math.Min((index + 1) * 51, 255) - oldColor;

            index = down <= up ? index : index + 1;

            return (byte)index;
        }

        private static void SetColors2Positions(byte[] rgbValuesBmp, int x, int y, int bmpHeight, int stride, Color oldColor, Color newColor)
        {
            int diffR = oldColor.R - newColor.R;
            int diffG = oldColor.G - newColor.G;
            int diffB = oldColor.B - newColor.B;

            var positionsWithFactor = (new (int, int, double)[] {
                (x + 0, y + 1, 8.0),
                (x + 0, y + 2, 4.0),
                (x + 1, y - 2, 2.0),
                (x + 1, y - 1, 4.0),
                (x + 1, y - 0, 8.0),
                (x + 1, y + 1, 4.0),
                (x + 1, y + 2, 2.0),
            });

            for(int i = 0; i < positionsWithFactor.Length; i++) {
                (int, int, double) posFact = positionsWithFactor[i];
                posFact.Item3 /= 32.0;
                if (IsPixelExist(posFact.Item1, posFact.Item2, stride / 3, bmpHeight)) { 
                    int counter = XY2Counter(posFact.Item1, posFact.Item2, stride);
                    byte B = rgbValuesBmp[counter];
                    byte G = rgbValuesBmp[counter + 1];
                    byte R = rgbValuesBmp[counter + 2];

                    B += (byte)(diffB * posFact.Item3);
                    G += (byte)(diffG * posFact.Item3);
                    R += (byte)(diffR * posFact.Item3);

                    rgbValuesBmp[counter] = B;
                    rgbValuesBmp[counter + 1] = G;
                    rgbValuesBmp[counter + 2] = R;
                }
            }
        }

        #endregion
    }

    public struct YCbCr
    {
        public byte Y = 0;
        public byte Cb = 0;
        public byte Cr = 0;

        public YCbCr()
        {
        }
    }
}