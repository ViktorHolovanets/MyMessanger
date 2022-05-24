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
        UdpClient udpClientListener;
        UdpClient udpClient;
        IPEndPoint endPoint;
        int portRecive;
        int portRemote;
        string IPAdressRemote;
        public event Action<byte[]> IcomingMessanger;
        public ClientServerUdp(int portRecive, int portRemote, string iPAdressRemote)
        {
            this.portRecive = portRecive;
            this.portRemote = portRemote;
            IPAdressRemote = iPAdressRemote;
            endPoint = new IPEndPoint(IPAddress.Parse(iPAdressRemote), portRemote);
            udpClient=new UdpClient();
        }

        public async void StartListening()
        {
            if (udpClientListener != null) return;
            udpClientListener = new UdpClient(portRecive);
            while (true)
            {
                var data = await udpClientListener.ReceiveAsync();
                IcomingMessanger?.Invoke(data.Buffer);
            }
        }
        public void SendMessage(byte[] b)=> udpClient?.Send(b, b.Length, endPoint);
       
    }

}
