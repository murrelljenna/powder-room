using System.Net.Http.Json;
using MessagePack;
using powdered_networking.messages;

/*[MessagePackObject]
public class NetworkState : NetworkMessage
{
    public int Tick;
    public NetworkObject[] objects;
}*/


[MessagePackObject]
public class NetworkObject
{
    [Key(0)]
    public string Id { get; set; }
    [Key(1)]
    public string OwnerId { get; set; }
    [Key(2)]
    public NetworkVector3 pos { get; set; }

    public NetworkObject(string _id, string _ownerId, NetworkVector3 _pos)
    {
        Id = _id;
        OwnerId = _ownerId;
        pos = _pos;
    }
    
    public NetworkObject(string _id, string _ownerId)
    {
        Id = _id;
        OwnerId = _ownerId;
        pos = new NetworkVector3(0, 0, 0);
    }
}
