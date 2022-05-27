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

namespace MyMessangerExam.ViewElement
{
    /// <summary>
    /// Логика взаимодействия для VideoCallCustomBalloonUserControl.xaml
    /// </summary>
    public partial class VideoCallCustomBalloonUserControl : UserControl
    {
        public event Action AcceptVideoCall;
        public event Action RejectVideoCall;
        public VideoCallCustomBalloonUserControl()
        {
            InitializeComponent();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e) => AcceptVideoCall?.Invoke();

        private void btnReject_Click(object sender, RoutedEventArgs e) => RejectVideoCall?.Invoke();
    }
}
