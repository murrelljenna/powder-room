using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using powdered_networking;
using powdered_networking.messages;
using PowderRoom.scripts.networking_wrapper;

public abstract partial class GodotSocketClient : Node
{
    private ConcurrentQueue<NetworkInput> _inputQueue = new ConcurrentQueue<NetworkInput>();
    private ConcurrentQueue<QueuedInstantiation> _instantiationQueue = new ConcurrentQueue<QueuedInstantiation>();
    private ConcurrentQueue<List<NetworkObject>> _networkObjectQueue = new ConcurrentQueue<List<NetworkObject>>();
    public Node sceneReference;
    private GodotNetworkObjectPosTracker networkObjectTracker;
    private ConcurrentQueue<NetworkState> networkStateQueue = new ConcurrentQueue<NetworkState>();
    private ConcurrentQueue<NetworkInput> serverReceiveInputQueue = new ConcurrentQueue<NetworkInput>();
    private bool isServer()
    {
        string[] args = OS.GetCmdlineArgs();
        return (args[0] == "--server");
    }
    
    public override void _Ready()
    {
        networkObjectTracker = new GodotNetworkObjectPosTracker(_networkObjectQueue);
        
        
        if (isServer())
        {
            StartServerAsync();
        }
        else
        {
            StartSocketClient();
        }
    }

    public override void _Process(double delta)
    {
        if (isServer()) {
            networkObjectTracker.TickNetworkTransformTracking();
        }
        QueueInput();
        InstantiateNetworkObjects();
        TickNetworkState();
    }

    public void OnInput(Action<NetworkInput> action)
    {
        if (isServer())
        {
            if (serverReceiveInputQueue.TryDequeue(out NetworkInput input))
            {
                action(input);
            }
        }
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
        await Task.Run(() => SocketClient.ConnectAndSendMessageAsync(_inputQueue, _instantiationQueue, networkStateQueue, cts.Token));
        
    }
    private async void StartServerAsync()
    {
        
        // Run the server in the background without blocking the main thread
        await Task.Run(() => Server.StartServerAsync(serverReceiveInputQueue, _instantiationQueue, _networkObjectQueue));
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
            networkObjectTracker.RegisterNetworkObject(instantiation.id, instantiation.ownerId, node);
        }
    }

    private void TickNetworkState()
    {
        if (networkStateQueue.TryDequeue(out NetworkState networkState))
        {
            SyncNetworkState(networkState);
        }
    }

    private void SyncNetworkState(NetworkState state)
    {
        networkObjectTracker.SyncPos(state.objects);
    }
}