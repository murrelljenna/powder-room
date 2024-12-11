using System.Net.Http.Json;
using MessagePack;
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

    public NetworkObject(string _id)
    {
        Id = _id;
    }
}
