using LibraryDb;
using LibraryMessage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ServerUserConnection
{
    public class ServerConnection
    {
        private TcpListener listener;
        private IPEndPoint endPoint;
        public ObservableCollection<UserConnection> clients = new ObservableCollection<UserConnection>();
        public bool IsWorker = true;
        public event Action<UserConnection> ClientConnected;
        public event Action<UserConnection> ClientDisconnected;
        public event Action<User> newClientConnected;
        DbMessanger dbMessanger;
        public DbMessanger dbServer { get { return dbMessanger; } set { } }
        private object db = new object();
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms;
        public ServerConnection(int port, string dbConnect)
        {
            endPoint = new IPEndPoint(IPAddress.Any, port);
            listener = null;
            IsWorker = true;
            dbMessanger = new DbMessanger(dbConnect);
        }
        public ServerConnection(int port)
        {
            endPoint = new IPEndPoint(IPAddress.Any, port);
            listener = null;
            IsWorker = true;

            dbMessanger = new DbMessanger(MyFunction.StringConnection(@"Db\DatabaseUsers.mdf"));
        }
        public ServerConnection():this (1000){}
        public void StartServer()
        {
            if (listener != null)
                return;
            if (clients == null)
                clients = new ObservableCollection<UserConnection>();
            listener = new TcpListener(endPoint);
            listener.Start(12);
            while (IsWorker)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    UserConnection user = new UserConnection(client);
                    if (!IsWorker) break;

                    Task.Factory.StartNew((o) =>
                    {
                        RunMessanger(o as UserConnection);
                    }, user);
                }
                catch { }
            }
            ShutDownServer();
        }
        public void ShutDownServer()
        {

            clients?.Clear();
            clients = null;
            listener?.Stop();
        }

        public void SendToMessage(MyMessage myMessage)
        {
            if (dbMessanger.Users.FirstOrDefault(u => u.Id == myMessage.UserFrom_Id).IsBlackList) return;
            Task.Run(() =>
            {
                lock (db)
                {
                    if (myMessage.TypeMessage == 4|| myMessage.TypeMessage == -4) return;
                    dbMessanger.Messages.Add(myMessage);
                    dbMessanger.SaveChanges();
                }
            });
            UserConnection FromCont = clients.FirstOrDefault(c => c.GetUser.Id == myMessage.UserFrom_Id);
            UserConnection ToCont = clients.FirstOrDefault(c => c.GetUser.Id == myMessage.UserTo_Id);
            if (myMessage.TypeMessage != 4)
                FromCont?.SendMessage(myMessage);
            ToCont?.SendMessage(myMessage);
        }
        private void NewMessageClient(MyMessage obj, UserConnection user)
        {
            switch (obj.TypeMessage)
            {
                case -1:
                    user?.CloseConnection();
                    ClientDisconnected?.Invoke(user);
                    break;
                case 0:
                    ms = new MemoryStream(obj?.Content);
                    MySystemMessageQuery res = bf.Deserialize(ms) as MySystemMessageQuery;
                    EntryUser(res, user);
                    break;
                case 1:
                case 3:
                case -4:
                    SendToMessage(obj);
                    break;
                case 2:
                    ms = new MemoryStream();
                    var userFrom = dbMessanger.Users.FirstOrDefault(u => u.Id == obj.UserFrom_Id);
                    var userTo = dbMessanger.Users.FirstOrDefault(u => u.Id == obj.UserTo_Id);
                    List<UserContact> contacts = new List<UserContact>();
                    contacts.Add(new UserContact() { Id = userFrom.Id, Name = userFrom.Name, AvatarContact = userFrom.Avatar, IsNotRead = true });
                    contacts.Add(new UserContact() { Id = userTo.Id, Name = userTo.Name, AvatarContact = userTo.Avatar, IsNotRead = true });
                    bf.Serialize(ms, contacts);
                    ms.Position = 0;
                    obj.Content = ms.ToArray();
                    SendToMessage(obj); break;
                case 4:
                    UdpClientConnection(obj);
                    break;
                default:
                    break;
            }
        }

        private async void UdpClientConnection(MyMessage obj)
        {
            UserConnection FromCont = clients.FirstOrDefault(c => c.GetUser.Id == obj.UserFrom_Id);
            UserConnection ToCont = clients.FirstOrDefault(c => c.GetUser.Id == obj.UserTo_Id);
            if (FromCont != null && ToCont != null)
            {
                UserConnection[] userConnections = new UserConnection[] { FromCont, ToCont };
                await Task.Factory.StartNew((o) => UdpVideoCall(o as UserConnection[]), userConnections);
            }
        }

        private void UdpVideoCall(UserConnection[] userConnections)
        {
            string ipFrom = IPAddress.Parse(((IPEndPoint)userConnections[0].ClientConnection.Client.RemoteEndPoint).Address.ToString()).ToString();
            string ipFromLocal = IPAddress.Parse(((IPEndPoint)userConnections[0].ClientConnection.Client.LocalEndPoint).Address.ToString()).ToString();
            string ipTo = IPAddress.Parse(((IPEndPoint)userConnections[1].ClientConnection.Client.RemoteEndPoint).Address.ToString()).ToString();
            string ipToLocal = IPAddress.Parse(((IPEndPoint)userConnections[1].ClientConnection.Client.LocalEndPoint).Address.ToString()).ToString();
            if (ipFrom == "127.0.0.1")
                SendToMessage(new MyMessage() { TypeMessage = 4, Content = MyFunction.ConvertToBytes(ipToLocal), UserTo_Id = userConnections[1].GetUser.Id , UserFrom_Id= userConnections[0].GetUser.Id });
            else
                SendToMessage(new MyMessage() { TypeMessage = 4, Content = MyFunction.ConvertToBytes(ipFrom), UserTo_Id = userConnections[1].GetUser.Id, UserFrom_Id = userConnections[0].GetUser.Id });
            if (ipTo == "127.0.0.1")
                SendToMessage(new MyMessage() { TypeMessage = 4, Content = MyFunction.ConvertToBytes(ipFromLocal), UserTo_Id = userConnections[0].GetUser.Id , UserFrom_Id = userConnections[1].GetUser.Id });
            else
                SendToMessage(new MyMessage() { TypeMessage = 4, Content = MyFunction.ConvertToBytes(ipTo), UserTo_Id = userConnections[0].GetUser.Id, UserFrom_Id = userConnections[1].GetUser.Id });
        }

        private void EntryUser(MySystemMessageQuery message, UserConnection user)
        {
            User newConnect;
            bf = new BinaryFormatter();
            ms = new MemoryStream();
            lock (db)
            {
                if (message.IsNewUser)
                {
                    newConnect = new User() { Name = message.Name, Login = message.Login, Password = message.Password, Avatar = message.Avatar };
                    dbMessanger.Users.Add(newConnect);
                    dbMessanger.SaveChanges();
                    newClientConnected(newConnect);
                    user.GetUser = newConnect;                    
                    var result = new MySystemMessageRespon() { user = newConnect };
                    bf.Serialize(ms, result);
                    ms.Position = 0;
                }
                else
                {

                    newConnect = dbMessanger.Users?.FirstOrDefault(x => x.Login == message.Login && x.Password == message.Password);
                    if (newConnect == null)
                    {
                        user.CloseConnection();
                        return;
                    }
                    List<MyMessage> userMessage = dbMessanger.Messages.Where(x => x.UserTo_Id == newConnect.Id || x.UserFrom_Id == newConnect.Id)?.ToList();
                    List<UserContact> t = userMessage.Join(dbMessanger.Users,
                        m => m.UserFrom_Id,
                        u => u.Id,
                        (m, u) => new UserContact() { Id = u.Id, Name = u.Name, AvatarContact = u.Avatar }).ToList();
                    t.AddRange(userMessage.Join(dbMessanger.Users,
                        m => m.UserTo_Id,
                        u => u.Id,
                        (m, u) => new UserContact() { Id = u.Id, Name = u.Name, AvatarContact = u.Avatar }).Distinct().ToList());
                    t = t.Where(u => u.Id != newConnect.Id).Distinct().ToList();
                    var res = t;
                    for (int i = 0; i < t.Count; i++)
                    {
                        if (res.Count(r => r.Id == t[i].Id) > 1)
                        { res.Remove(t[i]); i--; }
                    }
                    var result = new MySystemMessageRespon() { user = newConnect, messages = userMessage, users = t };
                    bf.Serialize(ms, result);
                    ms.Position = 0;
                    user.GetUser = newConnect;
                }
            }
            ClientConnected?.Invoke(user);
            user.SendMessage(new MyMessage() { TypeMessage = 0, Content = ms.ToArray(), UserFrom_Id = newConnect.Id });
        }

        public void RunMessanger(UserConnection client)
        {

            while (IsWorker || client?.ClientConnection != null)
            {
                MyMessage message = client?.ReceiveMessage();
                if (message == null) return;
                NewMessageClient(message, client);
            }
            clients.Remove(client);
            ClientDisconnected?.Invoke(client);
        }

    }
}
