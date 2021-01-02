using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace TowersOfHanoi
{
    struct Move
    {
        public byte from;
        public byte to;

        public Move(byte from, byte to)
        {
            this.from = from;
            this.to = to;
        }
    }

    static class Essentials
    {
        public static double Lerp(double from, double to, double amount)
        {
            return from + (to - from) * amount;
        }

        public static double Hypot(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static double SqrHypot(double x, double y)
        {
            return x * x + y * y;
        }

        public static Color HSVtoRGB(double h, double s, double v)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (v <= 0)
            {
                R = G = B = 0;
            }
            else if (s <= 0)
            {
                R = G = B = v;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = v * (1 - s);
                double qv = v * (1 - s * f);
                double tv = v * (1 - s * (1 - f));
                switch (i)
                {
                    // Red is the dominant color
                    case 0:
                        R = v;
                        G = tv;
                        B = pv;
                        break;
                    // Green is the dominant color
                    case 1:
                        R = qv;
                        G = v;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = v;
                        B = tv;
                        break;
                    // Blue is the dominant color
                    case 3:
                        R = pv;
                        G = qv;
                        B = v;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = v;
                        break;
                    // Red is the dominant color
                    case 5:
                        R = v;
                        G = pv;
                        B = qv;
                        break;
                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                        R = v;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = v;
                        G = pv;
                        B = qv;
                        break;
                    // The color is not defined, we should throw an error.
                    default:
                        R = G = B = v; // Just pretend its black/white
                        break;
                }
            }
            return Color.FromRgb(Clamp((byte)(R * 255), 0, 255), Clamp((byte)(G * 255), 0, 255), Clamp((byte)(B * 255), 0, 255));
        }

        public static byte Clamp(byte x, byte min, byte max)
        {
            return (x < min) ? min : (x > max) ? max : x;
        }

        public static void PrintList<T>(List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            list.ForEach(a => sb.Append($"{a}\n"));
            Console.Write(sb.ToString());
        }
    }
}
