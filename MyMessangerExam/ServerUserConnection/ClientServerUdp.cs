using LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerUserConnection
{

    public class ClientServerUdp
    {
        public bool? Isworker=true;
        UdpClient udpClientListener;
        UdpClient udpClient;
        IPEndPoint endPoint;
        int portRecive;

        public event Action<byte[]> IcomingMessanger;
        public ClientServerUdp(int portRecive, int portRemote, string iPAdressRemote)
        {
            this.portRecive = portRecive;
            endPoint = new IPEndPoint(IPAddress.Parse(iPAdressRemote), portRemote);
            udpClient = new UdpClient();           
        }

        public async void StartListening()
        {
            if (udpClientListener != null) return;
            udpClientListener = new UdpClient(portRecive);
            while (Isworker==true)
            {
                try
                {
                    var data = await udpClientListener?.ReceiveAsync();
                    IcomingMessanger?.Invoke(data.Buffer);
                }
                catch { }
            }
        }
        public void SendMessage(byte[] b)
        {
            try
            {
                udpClient?.Send(b, b.Length, endPoint);
            }
            catch { }
        }

        public void Shutdown()
        {
            Isworker = false;
            udpClientListener?.Close();
            udpClientListener?.Dispose();
            udpClient?.Close();
            udpClient?.Dispose();
        }
    }

}
