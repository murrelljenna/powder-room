using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using powdered_networking.messages;
using PowderRoom.scripts.networking_wrapper;

namespace powdered_networking
{
    public abstract class SocketClient
    {
        private const bool DEBUG = false;
        private const string ServerIp = "127.0.0.1"; // Localhost
        private const int Port = 5000;
        private const string PlayerName = "Player";

        public static async Task ConnectAndSendMessageAsync(ConcurrentQueue<NetworkInput> messageQueue, ConcurrentQueue<QueuedInstantiation> instantiationQueue, ConcurrentQueue<NetworkState> networkStateQueue, CancellationToken cancel)
        {
            ClientObjectManager objectManager = new ClientObjectManager(instantiationQueue);
            bool errorThrown = false;
            try
            {
                // Connect to the server asynchronously
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ServerIp, Port);
                    if (DEBUG)
                    {
                        Console.WriteLine("Connected to server.");
                        Console.WriteLine($"Current messageQueue size: {messageQueue.Count}");
                    }

                    // Get the stream object for writing data
                    NetworkStream stream = client.GetStream();
                    var handshake = new PlayerConnected(PlayerName);
                    var msg = MessagePackSerializer.Serialize<INetworkMessage>(handshake);
                    await stream.WriteAsync(msg);
                    var confirmation = await ReceiveConfirmationAsync(stream, cancel);
                    var playerId = confirmation.playerId;
                    if (DEBUG) Console.WriteLine($"Player {playerId} connected.");
                    while (!cancel.IsCancellationRequested && !errorThrown)
                    {
                        //Console.WriteLine($"{!cancel.IsCancellationRequested} && ${!errorThrown}");
                        if (messageQueue.TryDequeue(out NetworkInput messageInput))
                        {
                            if (Server.DEBUG) Console.WriteLine("Writing from messageQueue.");
                            await SendNetworkInput(stream, messageInput);
                            await ReceiveMessage(stream, cancel, networkStateQueue, objectManager);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                errorThrown = true;
            }
        }
        private static async Task SendNetworkInput(NetworkStream stream, NetworkInput networkInput)
        {
            byte[] data = MessagePackSerializer.Serialize<INetworkMessage>(networkInput);
            await stream.WriteAsync(data, 0, data.Length);
        }
        
        private static async Task<PlayerConnectedConfirmation> ReceiveConfirmationAsync(NetworkStream stream, CancellationToken cancel)
        {
            byte[] buffer = new byte[1024];

            try
            {
                // Wait to receive data from the server (with cancellation token support)
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancel);
                if (bytesRead > 0)
                {
                    // Deserialize the received data as a PlayerConnectionConfirmation
                    var confirmation = MessagePackSerializer.Deserialize<PlayerConnectedConfirmation>(buffer);
                    return confirmation;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving confirmation: {ex.Message}");
            }

            return null;
        }
        
                private static async Task ReceiveMessage(NetworkStream stream, CancellationToken cancel, ConcurrentQueue<NetworkState> networkStateQueue, ClientObjectManager objectManager)
                {
                    byte[] buffer = new byte[1024];
        
                    try
                    {
                        // Wait to receive data from the server (with cancellation token support)
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancel);
                        if (bytesRead > 0)
                        {
                            // Deserialize the received data as a PlayerConnectionConfirmation
                            var confirmation = MessagePackSerializer.Deserialize<INetworkMessage>(buffer);
                            ProcessMessageFromServer(confirmation, objectManager, networkStateQueue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error receiving confirmation: {ex.Message}");
                    }
                }

        private static void ProcessMessageFromServer(INetworkMessage message, ClientObjectManager objectManager, ConcurrentQueue<NetworkState> networkStateQueue) 
        {
            switch (message)
            {
                case NetworkInstantiate networkInstantiate:
                    Console.WriteLine("Received an instantiate message from server");
                    objectManager.Instantiate(networkInstantiate.objectType, networkInstantiate.objectId, networkInstantiate.owner);
                    break;
                case NetworkState networkState:
                    networkStateQueue.Enqueue(networkState);
                    //Console.WriteLine("Received network state message from server");
                    break;
            }
        }
    }
}