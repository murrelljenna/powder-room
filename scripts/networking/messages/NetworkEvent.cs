using MessagePack;
using powdered_networking.messages;

[MessagePackObject]
public class NetworkEvent : INetworkMessage
{
    [Key(1)] public string eventType {get; set;}
}