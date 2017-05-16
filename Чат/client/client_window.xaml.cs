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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace client
{
    public partial class MainWindow : Window
    {
        string userName;
        string address;
        const int port = 8888;
        static TcpClient user = new TcpClient();
        static NetworkStream netStream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void connect()
        {
            try
            {
                user.Connect(address, port);
                netStream = user.GetStream();

                string message = userName;
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                netStream.Write(bytes, 0, bytes.Length);

                Thread thread = new Thread(new ThreadStart(Message));
                thread.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void disconnect()
        {
            if (netStream != null)
                netStream.Close();
            if (user != null)
                user.Close();
            Environment.Exit(0);
        }
        private void Message()
        {
            while (true)
            {
                try
                {
                    byte[] value = new byte[64];
                    string message;
                    StringBuilder stringBuilder = new StringBuilder();
                    int date = 0;
                    do
                    {
                        date = netStream.Read(value, 0, value.Length);
                        if (value != null)
                        {
                            stringBuilder.Append(Encoding.UTF8.GetString(value, 0, date));
                        }
                    }
                    while (netStream.DataAvailable);

                    message = stringBuilder.ToString();
                    outputText(message);
                }
                catch (Exception)
                {
                    disconnect();
                }
            }
        }
        public void outputText(string obj)
        {
            Dispatcher.Invoke(new Action<string>((a) => chat_txt.AppendText(a)), '[' + DateTime.Now.ToString() + "] - " + obj + '\n');
            scroll();
        }
        delegate void scroll_d();
        public void scroll()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new scroll_d(scroll));
                return;
            }
            chat_txt.ScrollToEnd();
        }
        static void sendMessage(string obj)
        {
            byte[] value = Encoding.UTF8.GetBytes(obj);
            netStream.Write(value, 0, value.Length);           
        }
        private void connectButtonClick(object sender, RoutedEventArgs e)
        {
            bool process = true;
            if (userName_txt.Text.Trim() == "")
            {
                MessageBox.Show("Введите имя и повторите еще раз");
                process = false;
            }
            else
            {
                userName = userName_txt.Text;
            }
            try
            {
                IPAddress.Parse(ip_txt.Text.Trim());
                address = ip_txt.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Введите другой адрес");
                process = false;
            }
            if (process)
            {
                connect();
                controls.IsExpanded = false;
            }
        }
        private void sendButtonClick(object sender, RoutedEventArgs e)
        {
            if (input_txt.Text.Trim() != "")
            {
                sendMessage(input_txt.Text);
                input_txt.Clear();
            }
            chat_txt.ScrollToEnd();
        }
        private void disconnectButtonClick(object sender, RoutedEventArgs e)
        {
            disconnect();
        }

        private void input_txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendMessage(input_txt.Text);
                input_txt.Clear();
            }
            chat_txt.ScrollToEnd();
        }
    }
}
