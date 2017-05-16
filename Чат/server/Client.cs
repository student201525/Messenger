using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace server
{
    public class Client
    {
        protected internal string idSender { get; private set; }
        protected internal NetworkStream netStream { get; private set; }
        string userName;
        string message;
        TcpClient user;
        Server chatServer;

        public Client(TcpClient tcpUser, Server serverObject)
        {
            idSender = Guid.NewGuid().ToString();
            user = tcpUser;
            chatServer = serverObject;
            serverObject.AddUser(this);
        }

        public void Process()
        {
            try
            {
                netStream = user.GetStream();
                message = ReceiveMessage();
                userName = message;
                message = userName + " присоединился";
                chatServer.StreamMessage(message,idSender);

                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                            message = ReceiveMessage();
                            message = String.Format("{0}: {1}", userName, message);
                            Console.WriteLine(message);
                            chatServer.StreamMessage(message,idSender);                    
                    }
                    catch
                    {
                        message = String.Format(userName+" вышел из чата");
                        Console.WriteLine(message);
                        chatServer.StreamMessage(message,idSender);
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                chatServer.DeleteUser(idSender);
                CloseConnection();
            }
        }
        private string ReceiveMessage()
        {
            byte[] value = new byte[64];
            StringBuilder stringBuilder = new StringBuilder();
            int date = 0;
            do
            {
                date = netStream.Read(value, 0, value.Length);
                stringBuilder.Append(Encoding.UTF8.GetString(value, 0, date));
            }
            while (netStream.DataAvailable);
            return stringBuilder.ToString();
        }
        protected internal void CloseConnection()
        {
            if (netStream != null)
                netStream.Close();
            if (user != null)
                user.Close();
        }
    }
}