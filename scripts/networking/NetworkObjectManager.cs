using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using Godot;
using MessagePack;
using powdered_networking.messages;

namespace PowderRoom.scripts.networking_wrapper;

public record QueuedInstantiation(string ownerId, string objectType, string id, int xPos, int yPos, int zPos);

public class ServerObjectManager
{
    public ConcurrentQueue<QueuedInstantiation> instantiateQueue = new ConcurrentQueue<QueuedInstantiation>();
    private Dictionary<string, List<string>> playerObjects = new Dictionary<string, List<string>>();
    
    public void Instantiate(string objectType, string ownerId, NetworkStream stream)
    {
        var objectId = GenerateObjectId();
        RegisterNodesByPlayer(objectId, ownerId);

        var instantiate = new NetworkInstantiate(objectType, ownerId, objectId);
        var msg = MessagePackSerializer.Serialize<INetworkMessage>(instantiate);
        stream.Write(msg);
        
        instantiateLocally(objectType, objectId, ownerId);
    }
    
    private static string GenerateObjectId()
    {
        return Guid.NewGuid().ToString();
    }

    private void instantiateLocally(string objectType, string objectId, string ownerId)
    {
        
        instantiateQueue.Enqueue(new QueuedInstantiation(ownerId, objectType, objectId, 0, 0, 0));
    }

    private void RegisterNodesByPlayer(string objectId, string ownerId)
    {
        var ids = playerObjects.GetValueOrDefault(ownerId, new List<string>());
        ids.Add(objectId);
        
        playerObjects.Add(ownerId, ids);
    }

    public ServerObjectManager(ConcurrentQueue<QueuedInstantiation> instantiateQueue)
    {
        this.instantiateQueue = instantiateQueue;
    }
}

public class ClientObjectManager
{
    public ConcurrentQueue<QueuedInstantiation> instantiateQueue = new ConcurrentQueue<QueuedInstantiation>();
    public void Instantiate(string objectType, string objectId, string ownerId)
    {
        instantiateLocally(objectType, objectId, ownerId);
    }
    
    private static string GenerateObjectId()
    {
        return Guid.NewGuid().ToString();
    }

    private void instantiateLocally(string objectType, string objectId, string ownerId)
    {
        
        instantiateQueue.Enqueue(new QueuedInstantiation(ownerId, objectType, objectId, 0, 0, 0));
    }
    
    public ClientObjectManager(ConcurrentQueue<QueuedInstantiation> instantiateQueue)
    {
        this.instantiateQueue = instantiateQueue;
    }
}