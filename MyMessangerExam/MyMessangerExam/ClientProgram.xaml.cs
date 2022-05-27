using LibraryDb;
using LibraryMessage;
using Microsoft.Win32;
using MyMessangerExam.Registration;
using MyMessangerExam.ViewElement;
using MyMessangerExam.ViewMessage;
using ServerUserConnection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyMessangerExam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool? IsVideoCall = false;
        private UserConnection client;
        MySystemMessageRespon messageRespon;
        private IPEndPoint endPoint;
        //VideoCall GetVideoCall;
        event Action TheEndVideoCall;
        public MainWindow()
        {
            InitializeComponent();
            //lbMessanger.ItemsSource = messages;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (messageRespon != null)
                client?.SendMessage(new MyMessage() { TypeMessage = -1, UserFrom_Id = messageRespon.user.Id });
            client?.CloseConnection();
            Close();
        }

        private void btnSmile_Click(object sender, RoutedEventArgs e)
        {
            if (popSmile.IsOpen)
                popSmile.IsOpen = false;
            else popSmile.IsOpen = true;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            UserConnectionReg tempWindow = new UserConnectionReg();
            if (tempWindow.ShowDialog() == true)
            {

                try
                {
                    if (string.IsNullOrEmpty(tbIP.Text) && string.IsNullOrEmpty(tbPort.Text))
                    {
                        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1000); //  для зручного працювання на одній машині
                    }
                    else endPoint = new IPEndPoint(IPAddress.Parse(tbIP.Text), int.Parse(tbPort.Text));
                }
                catch
                {
                    client = null;
                    MessageBox.Show("Error");
                }
                client = new UserConnection(endPoint);
                client.IncomingMessage += AddMessage;
                client.StartConnection();
                if (client.ClientConnection.Connected)
                {
                    Task.Factory.StartNew(() =>
                    {
                        while (client.ClientConnection?.Connected == true)
                        {
                            client?.ReceiveMessage();
                        }
                    });
                    spConnect.IsEnabled = false;
                    btnDisconnect.IsEnabled = true;
                    Thread thread = new Thread(() =>
                    {
                        var b = MyFunction.ConvertToBytes(tempWindow.message);
                        MyMessage message = new MyMessage() { Content = b, IsMyMessage = true };
                        client?.SendMessage(message);
                    });
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
            Visibility = Visibility.Visible;
            tiMessanger.IsEnabled = true;
        }
        void ViewMessanger()
        {
            UserContact user = null;
            void c()
            {
                lbMessanger.Children.Clear();
                user = (UserContact)lbContacts.SelectedItem;
            }
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();
            if (user == null) return;
            user.IsNotRead = false;
            if (messageRespon.messages.Count(mess => mess.UserFrom_Id == user.Id || mess.UserTo_Id == user.Id) > 0)
            {
                foreach (var item in messageRespon.messages.Where(mess => mess.UserFrom_Id == user.Id || mess.UserTo_Id == user.Id).ToList())
                {
                    ViewMessageContact(item);
                }
            }
            scrollViewer.ScrollToEnd();
        }

        private void ViewMessageContact(MyMessage item)
        {
            UserControlViewMessage userControlViewMessage = new UserControlViewMessage();
            if (item.UserFrom_Id == messageRespon.user.Id)
            {
                userControlViewMessage.stpMessage.Background = new SolidColorBrush(Color.FromRgb(105, 99, 239));
                userControlViewMessage.AvatarUser.Source = UserImage.Source;
                userControlViewMessage.tblNameUser.Text = Username.Text;
                userControlViewMessage.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                UserContact temp = (UserContact)lbContacts.SelectedItem;
                userControlViewMessage.stpMessage.Background = new SolidColorBrush(Color.FromRgb(41, 27, 82));
                var s = messageRespon.users.FirstOrDefault(u => u.Id == item.UserFrom_Id).AvatarContact;
                userControlViewMessage.AvatarUser.Source = MyFunction.ConvertBytesToImage(s);
                userControlViewMessage.tblNameUser.Text = temp.Name;
                userControlViewMessage.HorizontalAlignment = HorizontalAlignment.Left;
            }
            userControlViewMessage.AddContent(item);
            void c() => lbMessanger.Children.Add(userControlViewMessage);
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();

        }


        void ChangeColorChat(int n)
        {
            foreach (var item in lbContacts.Items)
            {
                if (item is UserContact)
                {
                    UserContact user = (UserContact)item;

                    if (user.Id == n)
                    {
                        user.IsNotRead = true;
                    }
                }
            }
        }

        void AddMessage(MyMessage m)
        {

            BinaryFormatter bf = new BinaryFormatter();
            //
            void c()
            {
                switch (m.TypeMessage)
                {
                    case 0:
                        using (MemoryStream ms = new MemoryStream(m.Content))
                        {
                            messageRespon = bf.Deserialize(ms) as MySystemMessageRespon;
                        }
                        if (messageRespon != null)
                        {

                            if (messageRespon.users != null)
                                lbContacts.ItemsSource = messageRespon.users.ToList();
                            else messageRespon.users = new List<UserContact>();
                            if (messageRespon.messages == null)
                                messageRespon.messages = new List<MyMessage>();
                            Username.Text = messageRespon.user.Name;
                            NumberUser.Text = messageRespon.user.Id.ToString();
                            client.GetUser = messageRespon.user;
                            MemoryStream byteStream = new MemoryStream(messageRespon.user.Avatar);
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.StreamSource = byteStream;
                            image.EndInit();
                            UserImage.Source = image;
                        }
                        else client.CloseConnection();
                        break;
                    case 1:
                    case 3:
                        messageRespon.messages.Add(m);
                        ChangeColorChat(m.UserFrom_Id);
                        ViewMessanger();
                        ShowInfoNewMessage(m.UserFrom_Id);
                        break;
                    case 2:
                        List<UserContact> res;
                        using (MemoryStream ms = new MemoryStream(m.Content))
                        {
                            res = bf.Deserialize(ms) as List<UserContact>;
                        }
                        messageRespon.users.AddRange(res.Where(usc => usc.Id != messageRespon.user.Id));
                        lbContacts.ItemsSource = messageRespon.users.ToList();
                        break;
                    case -4:
                        TheEndVideoCall?.Invoke();
                        TheEndVideoCall = null;
                        break;
                    case 4:                      
                        if (IsVideoCall == false)
                        {
                            VideoCallCustomBalloonUserControl videoCall = new VideoCallCustomBalloonUserControl();
                            videoCall.AcceptVideoCall+=()=> Task.Run(() =>
                            {
                                VideoCall GetVideoCall = null;
                                void c1()
                                {
                                    GetVideoCall = new VideoCall(client, m);
                                    GetVideoCall.TheEnd += () => IsVideoCall = false;
                                    GetVideoCall.Show();
                                    TheEndVideoCall += GetVideoCall.Close;
                                }
                                if (!Dispatcher.CheckAccess())
                                    Dispatcher.Invoke(c1);
                                else c1();
                            });
                            IsVideoCall = true;
                            videoCall.AcceptVideoCall += () => Task.Run(() =>
                            {
                                
                            });
                        }
                        else
                        {

                        }

                        break;
                    case 404:
                        DisconnectToServer();
                        MessageBox.Show("Disconnect to server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        break;
                }
            }
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();

        }



        private void ShowInfoNewMessage(int IdUser)
        {
            if (IdUser == messageRespon.user.Id) return;
            var test = new UserControlCustomBalloon();
            var user = messageRespon.users.First(u => u.Id == IdUser);
            test.Username.Text = user.Name;
            test.UserImage.Source = MyFunction.ConvertBytesToImage(user.AvatarContact);
            TbIInfo.ShowCustomBalloon(test, System.Windows.Controls.Primitives.PopupAnimation.Slide, 10000);
        }
        private void btnDisconnect_Click(object sender, RoutedEventArgs e) => DisconnectToServer();


        void DisconnectToServer()
        {
            if (messageRespon != null)
                client?.SendMessage(new MyMessage() { TypeMessage = -1, UserFrom_Id = messageRespon.user.Id });
            client?.CloseConnection();
            lbContacts.ItemsSource = null;
            messageRespon = null;
            spConnect.IsEnabled = true;
            btnDisconnect.IsEnabled = false;
            Username.Text = null;
            UserImage.Source = null;
            tiMessanger.IsEnabled = false;
        }

        private void btnToSend_Click(object sender, RoutedEventArgs e)
        {
            if (lbContacts.SelectedItem is UserContact)
            {
                UserContact user = (UserContact)lbContacts.SelectedItem;
                if (!string.IsNullOrEmpty(tbMessage.Text))
                {
                    var text = MyFunction.ConvertToBytes(tbMessage.Text);
                    MyMessage temp = new MyMessage() { TypeMessage = 1, UserFrom_Id = messageRespon.user.Id, UserTo_Id = user.Id, Content = text };
                    client?.SendMessage(temp);
                    tbMessage.Text = "";
                }
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tbMessage.Text += (sender as Button).Content.ToString();
        }

        private void lbContacts_SelectionChanged(object sender, SelectionChangedEventArgs e) => ViewMessanger();

        private void btnNewContact_Click(object sender, RoutedEventArgs e)
        {
            int idNewCount;

            if (int.TryParse(tbNewContact.Text, out idNewCount))
            {
                if (messageRespon.users.Count(u => u.Id == idNewCount) > 0)
                    return;
                client?.SendMessage(new MyMessage() { TypeMessage = 2, UserFrom_Id = messageRespon.user.Id, UserTo_Id = idNewCount, IsMyMessage = true });
            }
            tbNewContact.Text = "";
        }

        private void btnSendFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == true)
            {
                var path = openFile.FileNames;
                Task.Factory.StartNew(() => SendToFiles(path));
            }

        }
        private void SendToFiles(string[] path)
        {
            List<MessageFile> messageFiles = new List<MessageFile>();
            foreach (var item in path)
            {
                byte[] bytes;
                string fileName = Path.GetFileName(item);
                var file = new FileInfo(item);
                long numBytes = file.Length;
                string extension = file.Extension;
                BinaryReader reader = new BinaryReader(File.Open(item, FileMode.Open));
                bytes = reader.ReadBytes((int)numBytes);
                messageFiles.Add(new MessageFile() { NameFile = fileName, ContentFile = bytes, ExtensionFile = extension });
            }
            void c()
            {
                if (lbContacts.SelectedItem is UserContact)
                {
                    UserContact user = (UserContact)lbContacts.SelectedItem;
                    MyMessage temp = new MyMessage() { TypeMessage = 3, UserFrom_Id = messageRespon.user.Id, UserTo_Id = user.Id, Content = MyFunction.ConvertToBytes(messageFiles) };
                    client?.SendMessage(temp);
                }
            }
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void TbIInfo_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            MyFunction.SetFocusWindow(Title);
        }

        private void btnHide_Click(object sender, RoutedEventArgs e) => Hide();

        private void btnCallVideo_Click(object sender, RoutedEventArgs e)
        {
            if (lbContacts.SelectedItem is UserContact)
            {
                UserContact friend = (UserContact)lbContacts.SelectedItem;
                client.SendMessage(new MyMessage() { TypeMessage = 4, UserFrom_Id = messageRespon.user.Id, UserTo_Id = friend.Id });
            }
        }
    }
}
