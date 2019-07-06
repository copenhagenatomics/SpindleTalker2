using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VFDcontrol
{
    public class FreqVoltChart
    {
        public static int width = 200;
        public static int height = 200;
        public static Image Draw(VFDdata data)
        {
            float x1 = (float)(5 + (190 * data.MinFreq / 400.0));
            float x2 = (float)(5 + (190 * data.IntermediateFreq / 400.0));
            float x3 = (float)(5 + (190 * data.MaxFreq / 400.0));
            float y1 = (float)(5 + (190 * data.MinVoltage/data.VFDVoltageRating));
            float y2 = (float)(5 + (190 * data.IntermediateVoltage / data.VFDVoltageRating));
            float y3 = (float)(5 + (190 * data.MaxVoltage / data.VFDVoltageRating));

            var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, (float)width, (float)height);
                g.DrawLine(Pens.Black, 5, 5, 5, 195);
                g.DrawLine(Pens.Black, 5, 195, 195, 195);
                g.DrawLine(Pens.Red, x1, y1, x2, y2);
                g.DrawLine(Pens.Red, x2, y2, x3, y3);
            }

            return bmp;
        }
    }
}
