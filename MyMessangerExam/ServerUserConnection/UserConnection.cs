using LibraryDb;
using LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServerUserConnection
{
    public class UserConnection
    {
        private IPEndPoint endPoint;
        private TcpClient client;
        private BinaryFormatter formatter;
        public User GetUser { get; set; }
        public TcpClient ClientConnection { get => client; }
        public event Action<MyMessage> IncomingMessage;
        public event Action<string, object> SystemMessage;

        public UserConnection(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            client = null;
        }
        public UserConnection(IPAddress ip, int port)
        {
            endPoint = new IPEndPoint(ip, port);
            client = null;
        }
        public UserConnection(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentException("Подключение не может быть пустым");

            client = tcpClient;
            formatter = new BinaryFormatter();
        }
        public bool StartConnection()
        {
            if (client != null)
                return client.Connected;

            client = new TcpClient();
            try
            {
                client.Connect(endPoint);
                if (!client.Connected)
                {
                    SystemMessage?.Invoke("Не подключен!!!", null);
                }
                else
                {
                    formatter = new BinaryFormatter();
                    SystemMessage?.Invoke("Подключение установлено!", null);
                }
                return client.Connected;
            }
            catch (SocketException e)
            {
                SystemMessage?.Invoke("SocketException: " + e.Message, e);
                return false;
            }
            catch (Exception e)
            {
                SystemMessage?.Invoke("System exception: " + e.Message, e);
                return false;
            }
        }
        public void CloseConnection()
        {
            client?.Close();
            client = null;
            SystemMessage?.Invoke("Подключение закрыто!", null);
        }
        public void SendMessage(MyMessage message)
        {
            if (client != null && client.Connected)
            {
                formatter.Serialize(client?.GetStream(), message);
                SystemMessage?.Invoke("Отправлено сообщение", message);
            }
        }
        public Task SendMessageTask(MyMessage message)
        {
            return Task.Run(() =>
            {
                SendMessage(message);
            });
        }

        public MyMessage ReceiveMessage()
        {
            if (client != null & client?.Connected==true)
            {
                try
                {
                    var message = formatter.Deserialize(client.GetStream()) as MyMessage;
                    if (message != null)
                        Task.Run(()=>IncomingMessage?.Invoke(message));
                
                    return message;
                }
                catch
                { }

            }
            return null;
        }

        public Task ReceiveMessageTask()
        {
            return Task.Run(ReceiveMessage);
        }



        ///////////
        ///

        public void SendMessage(MySystemMessageRespon message)
        {
            if (client != null && client.Connected)
            {
                formatter.Serialize(client.GetStream(), message);
                SystemMessage?.Invoke("Отправлено сообщение", message);
            }
        }

        public void SendMessage(MySystemMessageQuery message)
        {
            if (client != null && client.Connected)
            {
                formatter.Serialize(client.GetStream(), message);
                SystemMessage?.Invoke("Отправлено сообщение", message);
            }
        }



        public MySystemMessageRespon ReceiveSystemMessage()
        {
            if (client != null & client.Connected)
            {
                try
                {
                    var message = formatter.Deserialize(client.GetStream()) as MySystemMessageRespon;
                    return message;
                }
                catch
                { }

            }
            return null;
        }
        public MySystemMessageQuery ReceiveSystemMessageQuery()
        {
            if (client != null & client.Connected)
            {
                try
                {
                    var message = formatter.Deserialize(client.GetStream()) as MySystemMessageQuery;
                    return message;
                }
                catch
                { }
            }
            return null;
        }
    }
}
