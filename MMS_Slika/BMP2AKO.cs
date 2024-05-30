using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMS_Slika
{
    internal class BMP2AKO
    {
        public static void Save(string filePath, Bitmap bmp, byte wordLength = 8)
        {
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                bmp.PixelFormat
            );

            IntPtr ptr = bmpData.Scan0;
            int stride = Math.Abs(bmpData.Stride);
            int bytes  = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);

            YCbCr[,] ycbcrValues = BMP_RGB2YUV(rgbValues, bmpData.Width, bmpData.Height, stride);

            bmp.UnlockBits(bmpData);

            var downsampledBytes = Downsampling(ycbcrValues);

            HuffmanCoder.Code(filePath, downsampledBytes, wordLength);
        }

        private static YCbCr[,] BMP_RGB2YUV(byte[] rgbValues, int bmpWidth, int bmpHeight, int stride)
        {
            YCbCr[,] YCbCrValues = new YCbCr[bmpHeight, bmpWidth];
            for(int i = 0; i < bmpHeight; i++) {
                for(int j = 0; j < bmpWidth; j++) {
                    int counter = Filters.XY2Counter(i, j, stride);
                    byte B = rgbValues[counter];
                    byte G = rgbValues[counter + 1];
                    byte R = rgbValues[counter + 2];
                    YCbCr ycbcr = RGB2YUV(Color.FromArgb(R, G, B));
                    YCbCrValues[i, j] = ycbcr;
                }
            }
            return YCbCrValues;
        }

        private static YCbCr RGB2YUV(Color rgb)
        {
            return new YCbCr {
                Y = (byte)(0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B),
                Cb = (byte)(128 - 0.168736 * rgb.R - 0.331264 * rgb.G + 0.5 * rgb.B),
                Cr = (byte)(128 + 0.5 * rgb.R - 0.418688 * rgb.G - 0.081312 * rgb.B)
            };
        }

        private static byte[] Downsampling(YCbCr[,] original)
        {
            byte[] downsampled = new byte[sizeof(int) * 2 + original.Length + original.Length / 2 + original.Length / 2];
            int bmpWidth = original.GetLength(1);
            int bmpHeight = original.GetLength(0);

            int offset = 0;
            byte[] bmpWidthBytes = BitConverter.GetBytes(bmpWidth);
            bmpWidthBytes.CopyTo(downsampled, offset);
            offset += bmpWidthBytes.Length;

            byte[] bmpHeightBytes = BitConverter.GetBytes(bmpHeight);
            bmpHeightBytes.CopyTo(downsampled, offset);
            offset += bmpHeightBytes.Length;

            for(int i = 0; i < bmpHeight; i++) {
                for(int j = 0; j < bmpWidth; j++) {
                    downsampled[offset] = original[i, j].Y;
                    offset++;
                }
            }

            for(int i = 0; i < bmpHeight; i++) {
                for(int j = 0; j < bmpWidth; j += 4) {
                    int y = j + ((i / 2) % 2) * 2;
                    if(y < original.GetLength(1) && offset + 3 < downsampled.Length) {
                        downsampled[offset] = original[i, y].Cb;
                        offset++;
                        downsampled[offset] = original[i, y].Cr;
                        offset++;
                        downsampled[offset] = original[i, y + 1].Cb;
                        offset++;
                        downsampled[offset] = original[i, y + 1].Cr;
                        offset++;
                    }
                }
            }

            return downsampled;
        }
    }
}
