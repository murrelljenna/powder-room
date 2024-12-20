using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using powdered_networking.messages;

namespace powdered_networking
{
    public abstract class SocketClient
    {
        private const string ServerIp = "127.0.0.1"; // Localhost
        private const int Port = 5000;

        public static async Task ConnectAndSendMessageAsync(ConcurrentQueue<NetworkInput> messageQueue)
        {
            try
            {
                // Connect to the server asynchronously
                using (TcpClient client = new TcpClient())
                {

                    await client.ConnectAsync(ServerIp, Port);
                    Console.WriteLine("Connected to server.");
                    Console.WriteLine($"Current messageQueue size: {messageQueue.Count}");
                    // Get the stream object for writing data
                    NetworkStream stream = client.GetStream();
                    if (messageQueue.TryDequeue(out NetworkInput messageInput))
                    {
                        Console.WriteLine("Writing from messageQueue.");
                        await SendNetworkInput(stream, messageInput);
                    }
                    else
                    {
                        Console.WriteLine("Writing generic network input.");
                        await SendNetworkInput(stream, new NetworkInput());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private static async Task SendNetworkInput(NetworkStream stream, NetworkInput networkInput)
        {
            byte[] data = MessagePackSerializer.Serialize<INetworkMessage>(networkInput);
            await stream.WriteAsync(data, 0, data.Length);
            Console.WriteLine($"Sent: {data}");
        }
    }
}