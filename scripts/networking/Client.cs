using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace powdered_networking
{
    public class SocketClient
    {
        private const string ServerIp = "127.0.0.1"; // Localhost
        private const int Port = 5000;

        public static async Task ConnectAndSendMessageAsync()
        {
            try
            {
                // Connect to the server asynchronously
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ServerIp, Port);
                    Console.WriteLine("Connected to server.");

                    // Get the stream object for writing data
                    NetworkStream stream = client.GetStream();
                    string message = "hello world";

                    // Convert string to bytes and send asynchronously
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(data, 0, data.Length);
                    Console.WriteLine($"Sent: {message}");

                    // The client connection will be closed when the using block ends
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}