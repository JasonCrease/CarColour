using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Threading;

namespace WpfProto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Processor m_Processor;
        Timer m_Timer;
        int picNum = 1;

        public MainWindow()
        {
            InitializeComponent();
            m_Timer = new Timer(Update, null, 1000, 600);
        }

        private void Update(object state)
        {
            m_Processor = new Processor();
            m_Processor.BeforeImagePath = System.IO.Path.GetFullPath(".\\..\\..\\..\\Images\\car" + picNum + ".jpg");

            string x, y, z;

            x = Dispatcher.Invoke(() => TextBoxHMid.Text);
            y = Dispatcher.Invoke(() => TextBoxHWidth.Text);
            z = Dispatcher.Invoke(() => TextBoxHAfter.Text);

            m_Processor.Process(int.Parse(z), 255, int.Parse(x), int.Parse(y));

            if (m_Processor.BeforeImage != null && m_Processor.AfterImage != null)
            {
                Dispatcher.Invoke(new Action(() =>
                    ImageBefore.Source = BitmapSourceConvert.ToBitmapSource(m_Processor.BeforeImage)));
                Dispatcher.Invoke(new Action(() =>
                    ImageAfter.Source = BitmapSourceConvert.ToBitmapSource(m_Processor.AfterImage)));
            }
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            Update(null);
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            picNum++;
            if (picNum > 7) picNum = 1;
        }

        public static class BitmapSourceConvert
        {
            [DllImport("gdi32")]
            private static extern int DeleteObject(IntPtr o);

            public static BitmapSource ToBitmapSource(Emgu.CV.IImage image)
            {
                using (System.Drawing.Bitmap source = image.Bitmap)
                {
                    IntPtr ptr = source.GetHbitmap();

                    BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        ptr,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                    DeleteObject(ptr);
                    return bs;
                }
            }
        }
    }
}
