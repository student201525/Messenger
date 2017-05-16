using System;
using System.Threading;



namespace server
{
    class Program
    {
        static Server server;
        static void Main(string[] args)
        {            
            try
            {
                server = new Server();
                Thread thread = new Thread(new ThreadStart(server.Audition));
                thread.Start(); 
            }
            catch (Exception exception)
            {
                server.Disconnect();
                Console.WriteLine(exception.Message);
            }
        }
    }
}
