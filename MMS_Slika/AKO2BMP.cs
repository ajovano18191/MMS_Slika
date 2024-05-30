using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMS_Slika
{
    internal class AKO2BMP
    {
        public static Bitmap Load(string filePath)
        {
            byte[] decodedBytes = HuffmanDecoder.Decode(filePath);
            YCbCr[,] ycbcrValues = Upsampling(decodedBytes);
            byte[] rgbValues = BMP_YUV2RGB(ycbcrValues);

            Bitmap bmp = new Bitmap(ycbcrValues.GetLength(0), ycbcrValues.GetLength(1), PixelFormat.Format24bppRgb);
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                bmp.PixelFormat
            );

            IntPtr ptr = bmpData.Scan0;
            int bytes  = Math.Abs(bmpData.Stride) * bmp.Height;

            Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private static YCbCr[,] Upsampling(byte[] downsampled)
        {
            int offset = 0;
            int bmpWidth = BitConverter.ToInt32(downsampled, 0);
            offset += sizeof(int);
            int bmpHeight = BitConverter.ToInt32(downsampled, 4);
            offset += sizeof(int);

            YCbCr[,] YCbCrValues = new YCbCr[bmpWidth, bmpHeight];

            for(int i = 0; i < YCbCrValues.Length; i++) {
                YCbCrValues[i / bmpWidth, i % bmpHeight].Y = downsampled[offset++];
            }

            for(int i = 0; i < YCbCrValues.Length; i += 4) {
                int x = i / bmpWidth;
                int y = i % bmpHeight;
                if((x / 2) % 2 == 0) {
                    YCbCrValues[x, y].Cb = downsampled[offset];
                    YCbCrValues[x, y].Cr = downsampled[offset + 1];
                    YCbCrValues[x, y + 2].Cb = downsampled[offset];
                    YCbCrValues[x, y + 2].Cr = downsampled[offset + 1];

                    YCbCrValues[x, y + 1].Cb = downsampled[offset + 2];
                    YCbCrValues[x, y + 1].Cr = downsampled[offset + 3];
                    YCbCrValues[x, y + 3].Cb = downsampled[offset + 2];
                    YCbCrValues[x, y + 3].Cr = downsampled[offset + 3];
                }
                else {
                    y += 2;
                    YCbCrValues[x, y].Cb = downsampled[offset];
                    YCbCrValues[x, y].Cr = downsampled[offset + 1];
                    YCbCrValues[x, y - 2].Cb = downsampled[offset];
                    YCbCrValues[x, y - 2].Cr = downsampled[offset + 1];

                    YCbCrValues[x, y + 1].Cb = downsampled[offset + 2];
                    YCbCrValues[x, y + 1].Cr = downsampled[offset + 3];
                    YCbCrValues[x, y - 1].Cb = downsampled[offset + 2];
                    YCbCrValues[x, y - 1].Cr = downsampled[offset + 3];
                }
                offset += 2;
            }

            return YCbCrValues;
        }

        private static Color YUV2RGB(YCbCr yCbCr)
        {
            int R = (int)(yCbCr.Y + 1.402 * (yCbCr.Cr - 128));
            int G = (int)(yCbCr.Y - 0.344136 * (yCbCr.Cb - 128) - 0.714136 * (yCbCr.Cr - 128));
            int B = (int)(yCbCr.Y + 1.772 * (yCbCr.Cb - 128));

            R = Math.Max(Math.Min(R, 255), 0);
            G = Math.Max(Math.Min(G, 255), 0);
            B = Math.Max(Math.Min(B, 255), 0);

            return Color.FromArgb(R, G, B);
        }

        private static byte[] BMP_YUV2RGB(YCbCr[,] ycbcrValues)
        {
            int bmpWidth = ycbcrValues.GetLength(1);
            int bmpHeight = ycbcrValues.GetLength(0);
            byte[] rgbValues = new byte[bmpWidth * bmpHeight * 3];

            for(int i = 0; i < bmpHeight; i++) {
                for(int j = 0; j < bmpWidth; j++) {
                    int counter = Filters.XY2Counter(i, j, bmpWidth * 3);
                    Color rgbColor = YUV2RGB(ycbcrValues[i, j]);
                    rgbValues[counter] = rgbColor.B;
                    rgbValues[counter + 1] = rgbColor.G;
                    rgbValues[counter + 2] = rgbColor.R;
                }
            }

            return rgbValues;
        }
    }
}
