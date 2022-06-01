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
using System.Windows.Controls;

namespace MyMessangerExam.ViewElement
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class VideoCall : Window
    {
        UserConnection Client;
        private ClientServerUdp udpVideo;
        private VoiceMessage voiceMessage;
        private FilterInfoCollection _filterInfoCollection;
        private VideoCaptureDevice _videoCaptureDevice;
        public event Action TheEnd;
        private bool IsSendVideo = true;
        private bool IsBroadcasting = false;
        byte[] bytes;
        int Friend_Id;
        bool CameraIsNotConnected;
        Thread ThreadBroadcast;
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
            voiceMessage = new VoiceMessage(45100, 45100, res);
            udpVideo.IcomingMessanger += ViewFoto;
            bytes = Client.GetUser.Avatar;
            Friend_Id = message.UserFrom_Id;
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



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            voiceMessage.StartVoiceMessage();
            udpVideo.StartListening();
            _filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_filterInfoCollection.Count > 0)
            {
                CameraIsNotConnected = false;
                _videoCaptureDevice = new VideoCaptureDevice(_filterInfoCollection[0].MonikerString);
                _videoCaptureDevice.NewFrame += _videoCaptureDevice_NewFrame;
                _videoCaptureDevice.Start();
            }
            else
            {
                CameraIsNotConnected = true;
                ThreadBroadcast = new Thread(Broadcast);
                //ThreadBroadcast.IsBackground = true;
                MyImage.Source = MyFunction.ConvertBytesToImage(bytes);
                Task.Run(() =>
                {
                    int n = 0;
                    while (n < 5)
                    {
                        udpVideo.SendMessage(bytes);
                        Thread.Sleep(5000);
                        n++;
                    }
                });
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Client?.SendMessage(new MyMessage() { TypeMessage = -4, UserFrom_Id = Client.GetUser.Id, UserTo_Id = Friend_Id });
            udpVideo?.Shutdown();
            voiceMessage?.StopVoiceMessage();
            _videoCaptureDevice?.SignalToStop();
            ThreadBroadcast?.Resume();
            ThreadBroadcast?.Abort();
            TheEnd?.Invoke();
        }
        private void Broadcast()
        {
            while (true)
            {
                var ms = new MemoryStream();
                Bitmap bitmap;
                Bitmap tempbitmap = (Bitmap)MyFunction.GetScreenBitmap().Clone();
                bitmap = new Bitmap(tempbitmap, 800, 600);
                bitmap?.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                var b = ms.ToArray();
                udpVideo.SendMessage(b);
                void c()
                {
                    MyImage.Source = MyFunction.ConvertBytesToImage(b);
                }
                if (!Dispatcher.CheckAccess())
                    Dispatcher.Invoke(c);
                else c();
                Thread.Sleep(100);
            }   
        }
        private void _videoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (IsSendVideo)
            {
                var ms = new MemoryStream();
                Bitmap bitmap;
                if (IsBroadcasting)
                {
                    Bitmap tempbitmap = (Bitmap)MyFunction.GetScreenBitmap().Clone();
                    bitmap = new Bitmap(tempbitmap, 800, 600);
                }
                else
                    bitmap = (Bitmap)eventArgs.Frame.Clone();
                bitmap?.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                var b = ms.ToArray();
                udpVideo.SendMessage(b);
                void c()
                {
                    MyImage.Source = MyFunction.ConvertBytesToImage(b);
                }
                if (!Dispatcher.CheckAccess())
                    Dispatcher.Invoke(c);
                else c();

            }
            else
            {
                udpVideo.SendMessage(bytes);
                void c() => MyImage.Source = MyFunction.ConvertBytesToImage(bytes);
                if (!Dispatcher.CheckAccess())
                    Dispatcher.Invoke(c);
                else c();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            udpVideo.Shutdown();
            voiceMessage.StopVoiceMessage();
            _videoCaptureDevice?.SignalToStop();
            TheEnd?.Invoke();
            Close();
        }

        private void btnOnOffSound_Click(object sender, RoutedEventArgs e)
        {
            if (voiceMessage.IsSound)
            {

                btnOnOffSound.Content = "🔇";
                voiceMessage.IsSound = false;
            }
            else
            {
                btnOnOffSound.Content = "🔊";
                voiceMessage.IsSound = true;
            }
        }

        private void btnOnOffMicrofon_Click(object sender, RoutedEventArgs e)
        {
            if (voiceMessage.IsSendVoice)
            {
                btnOnOffMicrofon.Content = "❌🎤";
                voiceMessage.IsSendVoice = false;
            }
            else
            {
                btnOnOffMicrofon.Content = "✔🎤";
                voiceMessage.IsSendVoice = true;
            }
        }

        private void btnOnOffSendVideo_Click(object sender, RoutedEventArgs e)
        {
            if (IsSendVideo)
            {
                btnOnOffSendVideo.Content = "❌📹";
                IsSendVideo = false;
            }
            else
            {
                btnOnOffSendVideo.Content = "✔📹";
                IsSendVideo = true;
            }
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

        private void btnBroadcastScreen_Click(object sender, RoutedEventArgs e)
        {
            if (IsBroadcasting)
            {
                if (CameraIsNotConnected)
                {
                    ThreadBroadcast.Suspend();
                    udpVideo.SendMessage(bytes);
                    MyImage.Source = MyFunction.ConvertBytesToImage(bytes);
                }
                btnBroadcastScreen.Content = "✔🖥";
                IsBroadcasting = false;
            }
            else
            {
                if(CameraIsNotConnected)
                {
                    if (ThreadBroadcast.ThreadState == ThreadState.Unstarted)
                        ThreadBroadcast.Start();
                    else ThreadBroadcast.Resume();
                }
                btnBroadcastScreen.Content = "❌🖥";
                IsBroadcasting = true;
            }
        }

        private void btnStateWindow_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
    }
}
