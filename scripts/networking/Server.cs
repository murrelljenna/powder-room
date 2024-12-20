using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using powdered_networking.messages;

namespace powdered_networking
{
	public class Server
	{
		private const int Port = 5000;

		public static async Task StartServerAsync()
		{
			Console.WriteLine("Hi there");
			TcpListener server = new TcpListener(IPAddress.Any, Port);

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
                    Console.WriteLine(buffer.Length);
					// Read data asynchronously
					
					int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

					INetworkMessage netObj = MessagePackSerializer.Deserialize<INetworkMessage>(buffer);
					Console.WriteLine("Deserializing message");
					switch (netObj)
					{
						case NetworkInput input: 
							Console.WriteLine("Input received!");
							break;
						
						case NetworkEvent networkEvent: 
							Console.WriteLine("Event received!");
							break;
					}

					// Close the client connection
					client.Close();
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
