using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using powdered_networking;

public abstract partial class GodotSocketClient : Node
{
    private ConcurrentQueue<NetworkInput> _inputQueue = new ConcurrentQueue<NetworkInput>();

    public override void _Ready()
    {
        StartServerAsync();
        StartSocketClient();
    }

    public void _Process(float delta)
    {
        QueueInput();
    }

    private void QueueInput()
    {
        NetworkInput input = PollInput();
        _inputQueue.Enqueue(input);
    }

    public abstract NetworkInput PollInput();
    
    private async void StartSocketClient()
    {
        // Run the client connection and message sending in the background
        await Task.Run(() => SocketClient.ConnectAndSendMessageAsync(_inputQueue));
    }
    private async void StartServerAsync()
    {
        // Run the server in the background without blocking the main thread
        await Task.Run(() => Server.StartServerAsync());
    }
}