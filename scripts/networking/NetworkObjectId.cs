using System.Net.Http.Json;
using MessagePack;
/*[MessagePackObject]
public class NetworkState : NetworkMessage
{
    public int Tick;
    public NetworkObject[] objects;
}*/
[MessagePackObject]
public struct NetworkInput : INetworkMessage
{
    [Key(0)]
    public bool Forward {get; set;}
}
[MessagePackObject]
public class NetworkEvent : INetworkMessage
{
    [Key(1)] public string eventType {get; set;}
}
/*
public enum NetworkMessageType
{
    NetworkEvent,
    NetworkInput,
    NetworkState
}
*/
[MessagePack.Union(0, typeof(NetworkInput))]
[MessagePack.Union(1, typeof(NetworkEvent))]
public interface INetworkMessage
{
    /*[Key(0)]
    public NetworkMessageType messageType { get; set; }
    
    [Key()]*/
    
}

[MessagePackObject]
public class NetworkObject
{
    [Key(0)]
    public string Id { get; set; }

    public NetworkObject(string _id)
    {
        Id = _id;
    }
}
