using MessagePack;
using powdered_networking.messages;

[MessagePackObject]
public struct NetworkInput : INetworkMessage
{
    [Key(0)]
    public bool Forward {get; set;}
}