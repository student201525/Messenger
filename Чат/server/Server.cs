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
    public class Server
    {
        const int port = 8888;
        static TcpListener admin;
        public List<Client> storage = new List<Client>();

        protected internal void AddUser(Client client)
        {
            if (storage.Contains(client))
                return;
            storage.Add(client);
        }
        protected internal void DeleteUser(string id)
        {

            Client client = storage.FirstOrDefault(obj => obj.idSender == id);
            if (client != null)
                return;
                storage.Remove(client);
        }
        protected internal void Audition()
        {
            try
            {
                admin = new TcpListener(IPAddress.Any, port);
                admin.Start();
                Console.WriteLine("Ожидайте подключения: ");

                while (true)
                {
                    TcpClient trcpUser = admin.AcceptTcpClient();
                    Client client = new Client(trcpUser, this);
                    Thread newClient = new Thread(new ThreadStart(client.Process));
                    newClient.Start();
                }
            }
            catch (Exception exeption)
            {
                Console.WriteLine(exeption.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        protected internal void StreamMessage(string message, string idRecipient)
        {
            byte[] value = Encoding.UTF8.GetBytes(message);
            if (value != null)
            {
                for (int i = 0; i < storage.Count; i++)
                {
                    storage[i].netStream.Write(value, 0, value.Length);
                }
            }

        }
        protected internal void Disconnect()
        {
            admin.Stop();
            for (int i = 0; i < storage.Count; i++)
            {
                storage[i].CloseConnection();
            }
            Environment.Exit(0);
        }
    }
}