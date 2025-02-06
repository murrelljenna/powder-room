using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Godot;
using MessagePack;
using powdered_networking.messages;

namespace PowderRoom.scripts.networking_wrapper;

// Needs to provide an accurate list of network objects to server code to send to clients

// Whatever instantiates registers that with this
// This then sends that data every godot frame to the net code via a queue


public class GodotNetworkObjectPosTracker 
{
    public ConcurrentQueue<List<NetworkObject>> networkObjectsQueue = new ConcurrentQueue<List<NetworkObject>>();

    public List<(NetworkObject, Node3D)> networkObjects = new List<(NetworkObject, Node3D)>();
    private Dictionary<String, Node3D> networkobjectsById = new Dictionary<String, Node3D>();
    public GodotNetworkObjectPosTracker(ConcurrentQueue<List<NetworkObject>> queue)
    {
        networkObjectsQueue = queue;
    }
    
    public void RegisterNetworkObject(string objectId, string ownerId, Node3D node)
    {
        NetworkVector3 pos = new NetworkVector3(node.GlobalPosition.X, node.GlobalPosition.Y, node.GlobalPosition.Z);
        NetworkObject networkObject = new NetworkObject(objectId, ownerId, pos);
        networkObjects.Add((networkObject, node));
        networkobjectsById.Add(objectId, node);
        Console.WriteLine("Registering new network object of id " + objectId);
    }

    public void TickNetworkTransformTracking()
    {
            List<NetworkObject> updatedObjects = new List<NetworkObject>();

            foreach (var (networkObject, node) in networkObjects)
            {
                networkObject.pos = new NetworkVector3(node.GlobalPosition.X, node.GlobalPosition.Y, node.GlobalPosition.Z);
                updatedObjects.Add(networkObject);
            }

            if (updatedObjects.Count > 0)
            {
                networkObjectsQueue.Enqueue(updatedObjects);
            }
    }

    public void SyncPos(NetworkObject[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            Console.WriteLine("Syncing object: " + objects[i].Id.ToString());
            var netObj = objects[i];
            if (networkobjectsById.TryGetValue(netObj.Id, out Node3D node))
            {
                Vector3 pos = new Vector3(node.GlobalPosition.X, node.GlobalPosition.Y, node.GlobalPosition.Z);
                node.GlobalPosition = pos;
            }
        }
    }
}

public class NetworkObjectPosTracker
{
    public ConcurrentQueue<List<NetworkObject>> networkObjectsQueue = new ConcurrentQueue<List<NetworkObject>>();
    public NetworkObjectPosTracker(ConcurrentQueue<List<NetworkObject>> queue)
    {
        networkObjectsQueue = queue;
    }

    public async Task TickPosTracking(NetworkStream stream)
    {
        if (networkObjectsQueue.TryDequeue(out List<NetworkObject> networkObjects))
        {
            if (networkObjects.Count > 0)
            {
                NetworkState networkState = new NetworkState();
                networkState.objects = networkObjects.ToArray();
                var msg = MessagePackSerializer.Serialize<INetworkMessage>(networkState);
                await stream.WriteAsync(msg);
            }

        }

    }
}