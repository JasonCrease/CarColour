using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace WpfProto
{
    class Processor
    {
        public Emgu.CV.Image<Bgr, byte> BeforeImage { get; private set; }
        public Emgu.CV.Image<Hsv, byte> DebugImage { get; private set; }
        public Emgu.CV.Image<Bgr, byte> AfterImage { get; private set; }
        public string BeforeImagePath { get; set; }

        private int FindHuePeak()
        {
            int[] hues = new int[180];
            int[] hSmooth = new int[180];

            int width = DebugImage.Width;
            int height = DebugImage.Height;

            // Build histogram
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    int dist = ((width / 2) - Math.Abs((width / 2) - i)) * 
                                ((height / 2) - Math.Abs((height / 2) - j));
                    int hue = DebugImage.Data[j, i, 0];

                    if (DebugImage.Data[j, i, 1] > 50)
                        hues[hue] += dist;
                }

            // Smooth histogram
            for (int i = 0; i < 180; i++)
                for (int j = -10; j < 10; j++)
                {
                    int x = i + j;
                    if (x >= 180) x -= 180;
                    if (x < 0) x += 180;
                    hSmooth[i] += hues[x] / Math.Abs(j + 30);
                }

            int maxI = 0;

            for (int i = 0; i < 180; i++)
            {
                if (hSmooth[maxI] < hSmooth[i])
                    maxI = i;
            }

            return maxI;
        }

        internal void Process(int hueAfter, byte satAfter, int hueMid, int hueWidth)
        {
            byte afterSat = 0;

            BeforeImage = new Image<Bgr, byte>(BeforeImagePath).Resize(440, 320, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC, false);
            DebugImage = BeforeImage.Convert<Hsv, byte>();

            hueMid = FindHuePeak();
            int hueStart = (180 + hueMid - (hueWidth / 2)) % 180;
            int hueEnd = (180 + hueMid + (hueWidth / 2)) % 180;

            for (int i = 0; i < DebugImage.Width; i++)
                for (int j = 0; j < DebugImage.Height; j++)
                {
                    int hue = DebugImage.Data[j, i, 0];
                    int sat = DebugImage.Data[j, i, 1];
                    int val = DebugImage.Data[j, i, 2];

                    if ((hueStart < hueEnd) && (hue < hueEnd && hue > hueStart)
                        || (hueStart > hueEnd) && (hue < hueEnd || hue > hueStart))
                    {
                        if (sat > 30)
                        {
                            DebugImage.Data[j, i, 0] =(byte) hueAfter;
                            //DebugImage.Data[j, i, 1] = satAfter;
                        }
                    }
                }

            AfterImage = DebugImage.Convert<Bgr, byte>();
        }
    }
}