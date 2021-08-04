using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFCommon
{
    public static class ColorUtil
    {
        public static System.Windows.Media.Color ConvertStringToColor(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }

        public static System.Windows.Media.Color WpfColorFromDrawingColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb((byte)((color.A) * .75), color.R, color.G, color.B);
        }

        public static int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
                c.R * c.R * .299 +
                c.G * c.G * .587 +
                c.B * c.B * .114);
        }
    }
}
