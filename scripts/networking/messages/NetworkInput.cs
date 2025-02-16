using MessagePack;
using powdered_networking.messages;

Net

[MessagePackObject]
public struct NetworkInput : INetworkMessage
{
    // TODO: Make these easily modifiable by extending NetworkInput
    [Key(0)]
    public bool Sprint {get; set;}
    [Key(1)]
    public bool Jump {get; set;}
    public NetworkVector2
    [Key(6)]
    public bool Fire {get; set;}


    public static NetworkInput Neutral()
    {
        return new NetworkInput {Forward = false, Sprint = false, Jump = false, Right = false, Left = false, Back = false, Fire = false};
    }
}