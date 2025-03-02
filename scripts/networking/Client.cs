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
        private const bool DEBUG = true;
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
                using (UdpClient client = new UdpClient())
                {
                    client.Connect(ServerIp, Port);
                    if (DEBUG)
                    {
                        Console.WriteLine("Connected to server.");
                        Console.WriteLine($"Current messageQueue size: {messageQueue.Count}");
                    }

                    // Get the stream object for writing data
                    var handshake = new PlayerConnected(PlayerName);
                    var msg = MessagePackSerializer.Serialize<INetworkMessage>(handshake);
                    client.Send(msg);
                    var confirmation = await ReceiveConfirmationAsync(client, cancel);
                    var playerId = confirmation.playerId;
                    if (DEBUG) Console.WriteLine($"Player {playerId} connected.");
                    while (!cancel.IsCancellationRequested && !errorThrown)
                    {
                        //Console.WriteLine("Checking for new input");
                        //Console.WriteLine($"{!cancel.IsCancellationRequested} && ${!errorThrown}");
                        if (messageQueue.TryDequeue(out NetworkInput messageInput))
                        {
                            Console.WriteLine("Sending new message.");
                            if (Server.DEBUG) Console.WriteLine("Writing from messageQueue.");
                            SendNetworkInput(client, messageInput);
                            if (client.Available > 0)
                            {
                                await ReceiveMessage(client, cancel, networkStateQueue, objectManager);
                            }
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
        private static void SendNetworkInput(UdpClient client, NetworkInput networkInput)
        {
            byte[] data = MessagePackSerializer.Serialize<INetworkMessage>(networkInput);
            client.Send(data);
        }
        
        private static async Task<PlayerConnectedConfirmation> ReceiveConfirmationAsync(UdpClient client, CancellationToken cancel)
        {
            byte[] buffer = new byte[1024];

            try
            {
                UdpReceiveResult result = await client.ReceiveAsync();
                var confirmation = MessagePackSerializer.Deserialize<PlayerConnectedConfirmation>(result.Buffer);
                return confirmation;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving confirmation: {ex.Message}");
            }

            return null;
        }
        
                private static async Task ReceiveMessage(UdpClient client, CancellationToken cancel, ConcurrentQueue<NetworkState> networkStateQueue, ClientObjectManager objectManager)
                {
                    byte[] buffer = new byte[1024];
        
                    try
                    {
                        if (client.Available > 0)
                        {
                        // Wait to receive data from the server (with cancellation token support)
                        UdpReceiveResult result = await client.ReceiveAsync();

                            // Deserialize the received data as a PlayerConnectionConfirmation
                            var confirmation = MessagePackSerializer.Deserialize<INetworkMessage>(result.Buffer);
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