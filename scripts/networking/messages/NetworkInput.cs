using MessagePack;
using powdered_networking.messages;

[MessagePackObject]
public struct NetworkInput : INetworkMessage
{
    // TODO: Make these easily modifiable by extending NetworkInput
    [Key(0)]
    public bool Sprint {get; set;}
    [Key(1)]
    public bool Jump {get; set;}
    [Key(2)]
    public NetworkVector2 Direction {get; set;}
    [Key(3)]
    public bool Fire {get; set;}


    public static NetworkInput Neutral()
    {
        return new NetworkInput {Sprint = false, Jump = false, Direction = new NetworkVector2(), Fire = false};
    }
}