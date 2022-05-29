using LibraryMessage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using Path = System.IO.Path;
using UserControl = System.Windows.Controls.UserControl;

namespace MyMessangerExam.ViewMessage
{
    /// <summary>
    /// Логика взаимодействия для UserControlViewMessage.xaml
    /// </summary>
    public partial class UserControlViewMessage : UserControl
    {
        MyMessage message;
        public event Action<MyMessage> DelMessage;
        public UserControlViewMessage()
        {
            InitializeComponent();
        }
        public void AddContent(MyMessage message)
        {            
            this.message = message;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = null;
            if (message.Content != null)
                ms = new MemoryStream(message.Content);
            switch (message.TypeMessage)
            {
                case 1:
                    wrpContentMessage.Visibility = Visibility.Visible;
                    var res = bf.Deserialize(ms) as string;
                    tblTextMessage.Visibility = Visibility.Visible;
                    tblTextMessage.Text = res;
                    break;
                case 3:
                    wrpContentMessage.Visibility = Visibility.Visible;
                    var files = bf.Deserialize(ms) as List<MessageFile>;
                    foreach (var item in files)
                    {
                        if (item.ExtensionFile == ".bmp" || item.ExtensionFile == ".gif" || item.ExtensionFile == ".jpg" || item.ExtensionFile == ".png")
                        {
                            Image image = new Image() { Source = MyFunction.ConvertBytesToImage(item.ContentFile), Tag = item };
                            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                            image.MaxHeight = 100;
                            image.MaxWidth = 100;
                            wrpContentMessage.Children.Add(image);
                        }
                        else
                        {
                            Button btn = new Button();
                            btn.Template = (ControlTemplate)FindResource("LoadFile");
                            btn.Content = $"{item.NameFile}";
                            btn.FontSize = 15;
                            btn.Tag = item;
                            btn.MouseDown += Button_MouseDown;
                            btn.MaxHeight = 100;
                            btn.MaxWidth = 100;
                            wrpContentMessage.Children.Add(btn);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Image image = (Image)sender;
                MessageFile file = (MessageFile)image.Tag;
                File.WriteAllBytes(Path.Combine(folderBrowserDialog.SelectedPath, file.NameFile), file.ContentFile);
            }

        }
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var content = (sender as System.Windows.Controls.Control).Tag;
                MessageFile file = (MessageFile)content;
                File.WriteAllBytes(Path.Combine(folderBrowserDialog.SelectedPath, file.NameFile), file.ContentFile);
            }
        }

        private void contextMenuDeleteMessage_Click(object sender, RoutedEventArgs e)
        {
            if (message != null) 
                DelMessage?.Invoke(message);
        }
    }
}
