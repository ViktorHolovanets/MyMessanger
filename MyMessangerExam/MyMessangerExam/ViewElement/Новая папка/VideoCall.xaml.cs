using AForge.Video.DirectShow;
using LibraryMessage;
using ServerUserConnection;
using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using NAudio.Wave;
using System.Threading.Tasks;

namespace MyMessangerExam.ViewElement
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class VideoCall : Window
    {
        //Подключены ли мы
        private bool connected;
        //сокет отправитель
       
        //поток для нашей речи
        WaveIn input;
        //поток для речи собеседника
        WaveOut output;
        //буфферный поток для передачи через сеть
        BufferedWaveProvider bufferStream;
        //поток для прослушивания входящих сообщений
        Thread in_thread;
        //сокет для приема (протокол UDP)
        Socket listeningSocket;



        ClientServerUdp udpVideo;
        ClientServerUdp udpAudio;
        public VideoCall(UserConnection client, MyMessage message)
        {

            InitializeComponent();
            Client = client;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = null;
            if (message.Content != null)
                ms = new MemoryStream(message.Content);
            var res = bf.Deserialize(ms) as string;
            udpVideo = new ClientServerUdp(45454, 45454, res);
            udpAudio = new ClientServerUdp(45100, 45100, res);
            udpAudio.IcomingMessanger += Listening;
            udpVideo.IcomingMessanger += ViewFoto;

            
        }
        private void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {
                udpAudio.SendMessage(e.Buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Listening(byte[] b)
        {

            try
            {
                bufferStream.AddSamples(b, 0, b.Length);
            }
            catch (SocketException ex)
            { }

        }

        private void ViewFoto(byte[] obj)
        {
            void c()
            {
                var ms = new MemoryStream(obj);
                FriendImage.Source = MyFunction.ConvertBytesToImage(ms.ToArray());
            }
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();
        }
        UserConnection Client;

        private FilterInfoCollection _filterInfoCollection;
        private VideoCaptureDevice _videoCaptureDevice;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Voice
            //создаем поток для записи нашей речи
            input = new WaveIn
            {
                //определяем его формат - частота дискретизации 8000 Гц, ширина сэмпла - 16 бит, 1 канал - моно
                WaveFormat = new WaveFormat(8000, 16, 1)
            };
            //добавляем код обработки нашего голоса, поступающего на микрофон
            input.DataAvailable += Voice_Input;
            //создаем поток для прослушивания входящего звука
            output = new WaveOut();
            //создаем поток для буферного потока и определяем у него такой же формат как и потока с микрофона
            bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            //привязываем поток входящего звука к буферному потоку
            output.Init(bufferStream);
            //сокет для отправки звука
            //this.client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            connected = true;
            //listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //создаем поток для прослушивания

            #endregion
            udpVideo.StartListening();
            udpAudio.StartListening();
            _filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            
            input.StartRecording();
            in_thread = new Thread(new ThreadStart(output.Play));
            in_thread.IsBackground = true;
            //запускаем его
            in_thread.Start();
            if (_filterInfoCollection.Count > 0)
            {
                _videoCaptureDevice = new VideoCaptureDevice(_filterInfoCollection[0].MonikerString);
                _videoCaptureDevice.NewFrame += _videoCaptureDevice_NewFrame;
                _videoCaptureDevice.Start();
            }
            else
            {
                var bytes = Client.GetUser.Avatar;
                Task.Run(() => {                
                while(true)
                    {
                        udpVideo.SendMessage(bytes);
                        Thread.Sleep(5000);
                    }
                });
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            listeningSocket.Close();
            listeningSocket.Dispose();

            
            if (output != null)
            {
                output.Stop();
                output.Dispose();
                output = null;
            }
            if (input != null)
            {
                input.Dispose();
                input = null;
            }
            bufferStream = null;
            if (_videoCaptureDevice != null)
                if (_videoCaptureDevice.IsRunning)
                    _videoCaptureDevice.Stop();

        }


        private void _videoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var ms = new MemoryStream();
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bytes = ms.ToArray();
            udpVideo.SendMessage(bytes);
            void c()
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();
                MyImage.Source = image;
            }
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (_videoCaptureDevice != null)
                if (_videoCaptureDevice.IsRunning)
                    _videoCaptureDevice.Stop();
            connected = false;
            listeningSocket.Close();
            listeningSocket.Dispose();

            
            if (output != null)
            {
                output.Stop();
                output.Dispose();
                output = null;
            }
            if (input != null)
            {
                input.Dispose();
                input = null;
            }
            bufferStream = null;
        }
    }
}
