using MessagePack;

namespace powdered_networking.messages;

[MessagePackObject]
public class NetworkInstantiate : INetworkMessage
{
    [Key(0)]
    public string objectType {get; set;}
    [Key(1)]
    public string owner {get; set;}

    public NetworkInstantiate(string objectType, string playerId)
    {
        this.objectType = objectType;
        this.owner = playerId;
    }
}