using LibraryDb;
using LibraryMessage;
using ServerUserConnection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace MyMessangerExamServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       


        ServerConnection serverConnection;

        ObservableCollection<User> AllUsers;

        public MainWindow()
        {
            InitializeComponent();
            AllUsers = new ObservableCollection<User>();
            lbAllUser.ItemsSource = AllUsers;
        }

        private void RemoveOnlineClient(UserConnection obj)
        {
            void c()
            {
                serverConnection.clients.Remove(obj);
                if (serverConnection.clients.Count == 0)
                    brdrTaskbarIcon.Visibility = Visibility.Hidden;
            }
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();
        }

        private void UpdateAllClients(User u)
        {
            void c() => AllUsers.Add(u);
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();
        }

        private void AddOnlineClient(UserConnection obj)
        {
            void c()
            {
                serverConnection.clients.Add(obj);
                brdrTaskbarIcon.Visibility = Visibility.Visible;
            }
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(c);
            else c();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            serverConnection = null;
            try
            {
                if ( string.IsNullOrEmpty(tbPort.Text))
                {
                    serverConnection = new ServerConnection();
                }
                else serverConnection = new ServerConnection(int.Parse(tbPort.Text));
            }
            catch
            { }
            if (serverConnection == null)
            {
                MessageBox.Show("Ошибка. Сервер создан по умолчанию.");
                serverConnection = new ServerConnection();
            }
            Task.Run(() => serverConnection.StartServer());
            btnStart.IsEnabled = false;
            serverConnection.ClientConnected += AddOnlineClient;
            serverConnection.newClientConnected += UpdateAllClients;
            serverConnection.ClientDisconnected += RemoveOnlineClient;
            lbOnlineUser.ItemsSource = serverConnection.clients;
            LoadllClients();
            tasklbOnlain.ItemsSource = serverConnection.clients;
            TbIInfo.ToolTipText = "Online";
        }

        private void LoadllClients()
        {
            foreach (var item in serverConnection.dbServer.Users)
            {
                AllUsers.Add(item);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (serverConnection == null) return;
            serverConnection.ShutDownServer();
            serverConnection.IsWorker = false;
            btnStart.IsEnabled = true;
            serverConnection = null;
            AllUsers.Clear();
            TbIInfo.ToolTipText = "Ofline";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            serverConnection.dbServer.SaveChanges();
        }

        private void mItOpen_Click(object sender, RoutedEventArgs e)
        {
            if(mItOpen.Header.ToString()== "Згорнути")
            {
                WindowServer.Hide();
                mItOpen.Header = "Розгорнути";
            }
            else
            {
                WindowServer.Show();
                mItOpen.Header = "Згорнути";
            }
        }
        private void mItClose_Click(object sender, RoutedEventArgs e) => Close();

        private void TbIInfo_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            WindowServer.Show();
            mItOpen.Header = "Згорнути";
            WindowServer.WindowState = WindowState.Normal;
            MyFunction.SetFocusWindow(WindowServer.Title);
        }
    }
}
