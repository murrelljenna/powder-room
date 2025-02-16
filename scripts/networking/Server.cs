using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;
using powdered_networking.messages;
using PowderRoom.scripts.networking_wrapper;

namespace powdered_networking
{
	public class Server
	{
		public const bool DEBUG = false;
		private const int Port = 5000;
		public static async Task StartServerAsync(ConcurrentQueue<NetworkInput> inputQueue, ConcurrentQueue<QueuedInstantiation> spawnQueue, ConcurrentQueue<List<NetworkObject>> stateQueue)
		{
			PlayerManager playerManager = new PlayerManager();
			TcpListener server = new TcpListener(IPAddress.Any, Port);
			ServerObjectManager objectManager = new ServerObjectManager(spawnQueue);
			NetworkObjectPosTracker posTracker = new NetworkObjectPosTracker(stateQueue);
			bool confirmationReceived = false;
			try
			{
				server.Start();
				Console.WriteLine($"Server started on port {Port}. Waiting for a connection...");

				while (true)
				{
					// Await the acceptance of a client without blocking the main thread
					TcpClient client = await server.AcceptTcpClientAsync();
					Console.WriteLine("Client connected!");

					// Get a stream object for reading data
					NetworkStream stream = client.GetStream();
					byte[] buffer = new byte[1024];
                    if (DEBUG) Console.WriteLine(buffer.Length);
					// Read data asynchronously

					while (true)
					{
						if (confirmationReceived)
						{
							await posTracker.TickPosTracking(stream);
						}

						if (stream.DataAvailable)
						{
							int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

							INetworkMessage netObj = MessagePackSerializer.Deserialize<INetworkMessage>(buffer);
							if (Server.DEBUG) Console.WriteLine("Deserializing message");
							switch (netObj)
							{
								case NetworkInput input:
									Console.WriteLine("Input received");
									inputQueue.Enqueue(input);
									break;

								case NetworkEvent networkEvent:
									Console.WriteLine("Event received!");
									break;
								case PlayerConnected playerConnected:
									NetworkPlayer player = playerManager.NewPlayer(playerConnected.playerName);
									var confirmation = new PlayerConnectedConfirmation(player.PlayerId);
									var msg = MessagePackSerializer
										.Serialize<PlayerConnectedConfirmation>(confirmation);
									await stream.WriteAsync(msg);
									Console.WriteLine("Player connected. Instantiating player");
									confirmationReceived = true;
									objectManager.Instantiate("player", player.PlayerId, stream);
									break;
							}
						}
					}

					// Close the client connection
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
			finally
			{
				server.Stop();
			}
		}
	}
}
