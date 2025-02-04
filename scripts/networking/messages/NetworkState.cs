using MessagePack;

namespace powdered_networking.messages;

[MessagePackObject]
public class NetworkState : INetworkMessage
{
    [Key(0)]
    public NetworkObject[] objects { get; set; }
}