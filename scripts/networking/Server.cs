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
			IPEndPoint? remoteEP = new IPEndPoint(IPAddress.Any, Port);
			PlayerManager playerManager = new PlayerManager();
			UdpClient server = new UdpClient(Port);
			ServerObjectManager objectManager = new ServerObjectManager(spawnQueue, playerManager);
			NetworkObjectPosTracker posTracker = new NetworkObjectPosTracker(stateQueue, playerManager);
			bool confirmationReceived = false;
			Console.WriteLine($"Server started on port {Port}. Waiting for a connection...");
				
			while (true)
			{
				if (confirmationReceived)
				{ 
					posTracker.TickPosTracking(server);
				}

				if (server.Available > 0)
				{

					byte[] message = server.Receive(ref remoteEP);

					INetworkMessage netObj = MessagePackSerializer.Deserialize<INetworkMessage>(message);
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
							NetworkPlayer player = playerManager.NewPlayer(playerConnected.playerName, remoteEP);
							var confirmation = new PlayerConnectedConfirmation(player.PlayerId);
							var msg = MessagePackSerializer
								.Serialize<PlayerConnectedConfirmation>(confirmation);
							server.Send(msg, remoteEP);
							Console.WriteLine("Player connected. Instantiating player");
							confirmationReceived = true;
							objectManager.Instantiate("player", player.PlayerId, server);
							break;
					}
				}
			}
		}
	}
}
