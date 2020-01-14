using AForge.Video.DirectShow;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;




namespace WebCamStream
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        public byte[] _bufImage;

        public event PropertyChangedEventHandler PropertyChanged;
        VideoCapture capture;
        Mat frame;
        Bitmap image;
        Thread videoTread;

        public Image Image { get; set; }
        public BitmapImage BitImage { get; set; } 
        private SynchronizationContext context = SynchronizationContext.Current;
        private VideoCaptureDevice videoCaptureDevice;
        public MainWindow()
        {

            InitializeComponent();
            this.DataContext = this;
            BitImage = new BitmapImage();

          

            videoTread = new Thread(GetVideo);

            videoTread.Start();
            //videoCaptureDevice = new AForge.Video.DirectShow.VideoCaptureDevice();
            //videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            //var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //videoCaptureDevice.Source = videoDevices[0].MonikerString;
            //videoCaptureDevice.Start();
        }

        private void GetVideo()
        {
            frame = new Mat();
            VideoCapture capture = new VideoCapture(0);
            capture.Open(0);
            while (true)
            {
                if (capture != null)
                {
                    capture.Read(frame);
                    image = BitmapConverter.ToBitmap(frame);
                    context.Post(ApplyImage, image);
                }
            }

           

        }

       

        //private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        //{
        //    context.Post(ApplyImage, (Image)eventArgs.Frame.Clone());
        //}




        private void ApplyImage(object o)
        {
            BitmapImage btm = new BitmapImage();
            using (MemoryStream memStream2 = new MemoryStream())
            {
                (o as Bitmap).Save(memStream2, System.Drawing.Imaging.ImageFormat.Png);
                memStream2.Position = 0;
                btm.BeginInit();
                btm.CacheOption = BitmapCacheOption.OnLoad;
                btm.UriSource = null;
                btm.StreamSource = memStream2;
                btm.EndInit();
            }
            BitImage = btm;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            videoTread.Abort();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
