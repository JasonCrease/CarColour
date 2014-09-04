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

        internal void Process(int hueAfter)
        {
            byte afterHue = (byte)hueAfter;
            byte afterSat = 0;

            BeforeImage = new Image<Bgr, byte>(BeforeImagePath).
                Resize(440, 320, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC, false);
            DebugImage = BeforeImage.Convert<Hsv,byte>();

            for (int i = 0; i < DebugImage.Width; i++)
                for (int j = 0; j < DebugImage.Height; j++)
                {
                    int hue = DebugImage.Data[j, i, 0];
                    int sat = DebugImage.Data[j, i, 1];
                    int val = DebugImage.Data[j, i, 2];

                    if ((hue < 10 || hue > 170))
                    {
                        //DebugImage.Data[j, i, 0] = afterHue;
                        //DebugImage.Data[j, i, 1] = afterSat;

                        if (val > 140)
                        {
                            DebugImage.Data[j, i, 1] = 0;
                        }
                        else
                        {
                            //DebugImage.Data[j, i, 1] = 0;
                            DebugImage.Data[j, i, 1] = 0;
                        }
                    }
                }

          AfterImage = DebugImage.Convert<Bgr, byte>();
        }
    }
}
