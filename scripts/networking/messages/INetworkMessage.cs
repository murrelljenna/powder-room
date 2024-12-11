namespace powdered_networking.messages;

[MessagePack.Union(0, typeof(NetworkInput))]
[MessagePack.Union(1, typeof(NetworkEvent))]
public interface INetworkMessage
{
    /*[Key(0)]
    public NetworkMessageType messageType { get; set; }

    [Key()]*/
    
}