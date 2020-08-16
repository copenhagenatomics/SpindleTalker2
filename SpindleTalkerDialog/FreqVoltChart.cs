
using System.Drawing;
using VfdControl;

namespace SpindleTalker2
{
    public class FreqVoltChart
    {
        public static int width = 300;
        public static int height = 300;
        public static int edgeWidth = 5;
        public static int dotSize = 6;
        public static int plotWidth = width - 2*edgeWidth;
        public static int plotHeight = height - 2*edgeWidth;
        public static int farEdge = width - edgeWidth;
        public static Image Draw(VFDdata data)
        {
            float x1 = (float)(edgeWidth + (plotWidth * data.MinimumFreq / data.MaxFreq));
            float x2 = (float)(edgeWidth + (plotWidth * data.IntermediateFreq / data.MaxFreq));
            float x3 = (float)(edgeWidth + (plotWidth * data.MaxFreq / data.MaxFreq));
            float x4 = (float)(edgeWidth + (plotWidth * data.OutFrequency / data.MaxFreq)) - dotSize/2;
            float y0 = farEdge;
            float y1 = (float)(farEdge - (plotHeight * data.MinVoltage / data.MaxVoltage));
            float y2 = (float)(farEdge - (plotHeight * data.IntermediateVoltage / data.MaxVoltage));
            float y3 = (float)(farEdge - (plotHeight * data.MaxVoltage / data.MaxVoltage));
            float y4 = (float)(farEdge - (plotHeight * data.OutVoltAC / data.MaxVoltage)) - dotSize/2;


            var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, (float)width, (float)height);
                g.DrawLine(Pens.Black, edgeWidth, edgeWidth, edgeWidth, farEdge);
                g.DrawLine(Pens.Black, edgeWidth, farEdge, farEdge, farEdge);
                g.DrawString($"{data.MaxFreq} Hz", new Font("Arial", 8), Brushes.Blue, 160, 180);
                if (data.VFDVoltageRating > 0)
                {
                    g.DrawString($"{data.MaxVoltage} V", new Font("Arial", 8), Brushes.Blue, edgeWidth, edgeWidth);
                    g.DrawLine(Pens.Red, x1, y0, x1, y1);
                    g.DrawLine(Pens.Red, x1, y1, x2, y2);
                    g.DrawLine(Pens.Red, x2, y2, x3, y3);
                    g.FillEllipse(Brushes.DarkRed, x4, y4, dotSize, dotSize);
                }
            }

            return bmp;
        }
    }
}
