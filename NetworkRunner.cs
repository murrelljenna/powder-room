using Godot;
using System;
using System.Threading.Tasks;
using powdered_networking;

public partial class NetworkRunner : Node
{
	public override void _Ready()
	{
		// Start the server in the background
		StartServerAsync();
		ConnectClientAsync();
	}

	private async void StartServerAsync()
	{
		// Run the server in the background without blocking the main thread
		await Task.Run(() => Server.StartServerAsync());
	}
	
	private async void ConnectClientAsync()
	{
		// Run the client connection and message sending in the background
		await Task.Run(() => SocketClient.ConnectAndSendMessageAsync());
	}
}
