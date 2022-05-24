using LibraryDb;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для NewUser.xaml
    /// </summary>
    public partial class NewUser : UserControl
    {
        UserConnectionReg user;
        byte[] bytes;
        public NewUser(UserConnectionReg user)
        {
            InitializeComponent();
            this.user = user;
        }

        private void Button_ClickOk(object sender, RoutedEventArgs e)
        {
            if (Password.Password == Password1.Password && !string.IsNullOrWhiteSpace(Name.Text) && !string.IsNullOrWhiteSpace(Login.Text))
            {
                if(bytes==null)
                {
                    MemoryStream ms = new MemoryStream();
                    BmpBitmapEncoder bbe = new BmpBitmapEncoder();
                    bbe.Frames.Add(BitmapFrame.Create(new Uri("pack://application:,,,/MyMessangerExam;component/Resources/Images/homer-simpson.png", UriKind.RelativeOrAbsolute)));
                    bbe.Save(ms);
                    bytes = ms.ToArray();                   
                }
                user.message = new LibraryMessage.MySystemMessageQuery(true, Login.Text, Password.Password) { Name= Name.Text, Avatar=bytes };
                user.DialogResult = true;
                user.Info.Children.Clear();
                user.Close();
            }
            else
            {
                MessageBox.Show("Error");
            }
            
        }

        private void Button_ClickCancel(object sender, RoutedEventArgs e) => user.Info.Children.Clear();

        private void btnLoadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog p = new OpenFileDialog();
            p.Filter = "Graphics File|*.bmp;*.gif;*. jpg; *.png";
            p.FileName = "";
            if (p.ShowDialog() == true)
            {
                bytes = LoadImag(p.FileName);                
            }
        }
        private byte[] LoadImag(string path)
        {
            byte[] bytes;
            long numBytes = new FileInfo(path).Length;
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
            bytes = reader.ReadBytes((int)numBytes);
            return bytes;
        }

        
    }
}
