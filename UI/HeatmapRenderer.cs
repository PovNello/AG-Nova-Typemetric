using System.Windows.Media;

namespace Typemetric.UI
{
    public static class HeatmapRenderer
    {
        private static readonly Color[] ThermalColors = new Color[]
        {
            Color.FromRgb(0x14, 0x24, 0x37), // 0.0 - NovaPanel (Dark Blue)
            Color.FromRgb(0x31, 0x4B, 0x63), // Transition (NovaBorder)
            Color.FromRgb(0x8F, 0xD8, 0xE0), // NovaCyan
            Color.FromRgb(0xDC, 0xCF, 0xAA), // NovaIvory (Yellowish)
            Color.FromRgb(0xE6, 0x82, 0x2F), // NovaOrange
            Color.FromRgb(0xD5, 0x67, 0x56)  // 1.0 - NovaDanger (Red)
        };

        public static SolidColorBrush GetColorForFrequency(int count, int maxCount)
        {
            if (count == 0 || maxCount == 0)
                return new SolidColorBrush(ThermalColors[0]);

            // Logarithmic scale to prevent extreme outliers (like Spacebar) from squashing the scale
            double logCount = System.Math.Log(count + 1);
            double logMax = System.Math.Log(maxCount + 1);
            double ratio = logCount / logMax;

            if (ratio < 0) ratio = 0;
            if (ratio > 1) ratio = 1;

            int segments = ThermalColors.Length - 1;
            double scaledRatio = ratio * segments;
            int index = (int)scaledRatio;

            if (index >= segments) 
                return new SolidColorBrush(ThermalColors[segments]);

            double fraction = scaledRatio - index;

            Color c1 = ThermalColors[index];
            Color c2 = ThermalColors[index + 1];

            byte r = (byte)(c1.R + (c2.R - c1.R) * fraction);
            byte g = (byte)(c1.G + (c2.G - c1.G) * fraction);
            byte b = (byte)(c1.B + (c2.B - c1.B) * fraction);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
    }
}
