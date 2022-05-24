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

namespace MyMessangerExam.Registration
{
    /// <summary>
    /// Логика взаимодействия для UserEnter.xaml
    /// </summary>
    public partial class UserEnter : UserControl
    {
        UserConnectionReg user;
        public UserEnter(UserConnectionReg win)
        {
            InitializeComponent();
            user = win;
        }

        private void Button_ClickOk(object sender, RoutedEventArgs e)
        {
            user.message = new LibraryMessage.MySystemMessageQuery(false, tbLogin.Text, tbPassword.Text);
            user.DialogResult = true;
            user.Info.Children.Clear();
            user.Close();
        }

        private void Button_ClickCancel(object sender, RoutedEventArgs e) => user.Info.Children.Clear();
    }
}
