using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace powdered_networking
{
	public class Server
	{
		private const int Port = 5000;

		public static void StartServer()
		{
			Console.WriteLine("Hi there");
			// Create a TCP/IP socket
			TcpListener server = new TcpListener(IPAddress.Any, Port);

			try
			{
				// Start listening for client requests
				server.Start();
				Console.WriteLine($"Server started on port {Port}. Waiting for a connection...");

				while (true)
				{
					// Accept a pending connection
					TcpClient client = server.AcceptTcpClient();
					Console.WriteLine("Client connected!");

					// Get a stream object for reading data
					NetworkStream stream = client.GetStream();
					byte[] buffer = new byte[1024];
					int bytesRead = stream.Read(buffer, 0, buffer.Length);

					// Convert bytes to string and print
					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					Console.WriteLine($"Received: {message}");

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
				// Stop the server
				server.Stop();
			}
		}
	}
}
