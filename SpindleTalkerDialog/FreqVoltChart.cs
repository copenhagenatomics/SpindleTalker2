
using System.Drawing;
using VfdControl;

namespace SpindleTalker2
{
    public class FreqVoltChart
    {
        public static int width = 200;
        public static int height = 200;
        public static Image Draw(VFDdata data)
        {
            float x1 = (float)(5 + (190 * data.MinFreq / data.MaxFreq));
            float x2 = (float)(5 + (190 * data.IntermediateFreq / data.MaxFreq));
            float x3 = (float)(5 + (190 * data.MaxFreq / data.MaxFreq));
            float x4 = (float)(5 + (190 * data.OutFrequency * 4 / data.MaxFreq)) - 3;
            float y1 = (float)(195 - (190 * data.MinVoltage / data.MaxVoltage));
            float y2 = (float)(195 - (190 * data.IntermediateVoltage / data.MaxVoltage));
            float y3 = (float)(195 - (190 * data.MaxVoltage / data.MaxVoltage));
            float y4 = (float)(195 - (190 * data.OutVoltAC / data.MaxVoltage)) - 3;


            var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, (float)width, (float)height);
                g.DrawLine(Pens.Black, 5, 5, 5, 195);
                g.DrawLine(Pens.Black, 5, 195, 195, 195);
                g.DrawString($"{data.MaxFreq} Hz", new Font("Arial", 8), Brushes.Blue, 160, 180);
                if (data.VFDVoltageRating > 0)
                {
                    g.DrawString($"{data.MaxVoltage} V", new Font("Arial", 8), Brushes.Blue, 5, 5);
                    g.DrawLine(Pens.Red, x1, y1, x2, y2);
                    g.DrawLine(Pens.Red, x2, y2, x3, y3);
                    g.FillEllipse(Brushes.DarkRed, x4, y4, 6, 6);
                }
            }

            return bmp;
        }
    }
}
