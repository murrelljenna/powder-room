using MessagePack;
using powdered_networking.messages;

[MessagePackObject]
public struct NetworkInput : INetworkMessage
{
    // TODO: Make these easily modifiable by extending NetworkInput
    [Key(0)]
    public bool Forward {get; set;}
    [Key(1)]
    public bool Sprint {get; set;}
}