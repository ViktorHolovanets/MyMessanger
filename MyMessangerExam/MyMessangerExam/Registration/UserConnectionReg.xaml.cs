using LibraryMessage;
using ServerUserConnection;
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
using System.Windows.Shapes;

namespace MyMessangerExam.Registration
{
    /// <summary>
    /// Логика взаимодействия для UserConnection.xaml
    /// </summary>
    public partial class UserConnectionReg : Window
    {
        public MySystemMessageQuery message;
        
        public UserConnectionReg()
        {
            InitializeComponent();
            //tempDbUsers = db;
        }

        private void Button_Click_newEnter(object sender, RoutedEventArgs e)
        {
            Info.Children.Clear();
            NewUser newUser = new NewUser(this);
            Info.Children.Add(newUser);
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.DialogResult=false;
            Close();
        }

        private void Button_Click_Enter(object sender, RoutedEventArgs e)
        {
            Info.Children.Clear();
            UserEnter newUser = new UserEnter(this);
            Info.Children.Add(newUser);
        }
    }
}
