using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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

    public override void _Process(double delta)
    {
        QueueInput();
    }

    private void QueueInput()
    {
        NetworkInput input = PollInput();
        _inputQueue.Enqueue(input);
    }

    public abstract NetworkInput PollInput();
    public abstract PackedScene InstantiateNode(string nodeName);
    
    private async void StartSocketClient()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        // Run the client connection and message sending in the background
        await Task.Run(() => SocketClient.ConnectAndSendMessageAsync(_inputQueue, cts.Token));
    }
    private async void StartServerAsync()
    {
        // Run the server in the background without blocking the main thread
        await Task.Run(() => Server.StartServerAsync(InstantiateNode));
    }
}