using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace QRCodeDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isBusy = false;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            setQR();
        }

        //http://chart.apis.google.com/chart?chs=300x300&cht=qr&chl=www.google.com
        public BitmapImage GenerateQR(string info)
        {
            BitmapImage image = new BitmapImage();

            try
            {
                Uri url = new Uri("http://chart.apis.google.com/chart?chs=300x300&cht=qr&chl=" +System.Web.HttpUtility.UrlEncode(info));

                //方法一，指向网络流
                //image.BeginInit();
                //image.UriSource = url;
                //image.EndInit();

                //方法二，使用http请求下载到内存流
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                //HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                //Stream stream = res.GetResponseStream();

                //MemoryStream mStream = new MemoryStream();
                //byte[] buffer = new byte[100000];
                //int numBytesRead = 0;
                //int n = 0;
                //do
                //{
                //    n = stream.Read(buffer, numBytesRead, 100);
                //    numBytesRead += n;
                //}
                //while (n > 0);
                //mStream.Write(buffer, 0, numBytesRead);

                //image.BeginInit();
                //image.StreamSource = mStream;
                //image.EndInit();

                //////释放资源
                //stream.Close();
                //stream.Dispose();
                //res.Close();

                //方法三，使用WebClient下载
                if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "temp//"))
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "temp//");
                WebClient client = new WebClient();
                string randomName = Guid.NewGuid().ToString() + ".png";//使用随机名称，解决文件被占用问题
                Uri imageUri = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "temp//" + randomName);
                client.DownloadFile(url, imageUri.AbsolutePath);
                client.Dispose();
                image.BeginInit();
                image.UriSource = imageUri;
                image.EndInit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Oooooooooops,failed."+e.Message);
            }

            return image;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            setQR();
        }

        private void setQR()
        {
            if (isBusy)
                return;

            isBusy = true;
            //string qr = System.Web.HttpUtility.UrlEncode(textQR.Text);
            QRImage.Source = GenerateQR(textQR.Text);
            isBusy = false;
        }

        /// <summary>
        /// 将图片保存到本地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource bsrc = (BitmapSource)QRImage.Source;

            Microsoft.Win32.SaveFileDialog sf = new Microsoft.Win32.SaveFileDialog();
            sf.DefaultExt = ".png";
            sf.Filter = "png (.png)|*.png";

            if (sf.ShowDialog()==true)
            {
                PngBitmapEncoder pngE = new PngBitmapEncoder();
                pngE.Frames.Add(BitmapFrame.Create(bsrc));
                using (Stream stream = File.Create(sf.FileName))
                {
                    pngE.Save(stream);
                }
            }
        }
    }
}
