using System;
using System.Net.Sockets;
using System.Text;

namespace powdered_networking
{
    class SocketClient
    {
        private const string ServerIp = "127.0.0.1"; // Localhost
        private const int Port = 5000;

        public static void ConnectAndSendMessage()
        {
            try
            {
                // Connect to the server
                TcpClient client = new TcpClient(ServerIp, Port);
                Console.WriteLine("Connected to server.");

                // Get a stream object for writing data
                NetworkStream stream = client.GetStream();
                string message = "hello world";

                // Convert string to bytes and send
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Console.WriteLine($"Sent: {message}");

                // Close the connection
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void Main(string[] args)
        {
            ConnectAndSendMessage();
        }
    }
}