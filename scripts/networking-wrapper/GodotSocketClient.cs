using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using powdered_networking;
using PowderRoom.scripts.networking_wrapper;

public abstract partial class GodotSocketClient : Node
{
    private ConcurrentQueue<NetworkInput> _inputQueue = new ConcurrentQueue<NetworkInput>();
    private ConcurrentQueue<QueuedInstantiation> _instantiationQueue = new ConcurrentQueue<QueuedInstantiation>();
    public Node sceneReference;
    
    public override void _Ready()
    {
        sceneReference = GetTree().Root;
        StartServerAsync();
        StartSocketClient();
    }

    public override void _Process(double delta)
    {
        QueueInput();
        InstantiateNetworkObjects();
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
        await Task.Run(() => Server.StartServerAsync(InstantiateNode, sceneReference, _instantiationQueue));
    }

    private void InstantiateNetworkObjects()
    {
        if (_instantiationQueue.TryDequeue(out QueuedInstantiation instantiation))
        {
            Console.WriteLine($"Instantiating {instantiation.objectType}");
            var scene = InstantiateNode(instantiation.objectType);
            var node = scene.Instantiate<Node3D>();
            var position = new Vector3(instantiation.xPos, instantiation.yPos, instantiation.zPos);
            node.SetGlobalPosition(position);
            AddChild(node);
        }
    }
}