namespace powdered_networking.messages;

[MessagePack.Union(0, typeof(NetworkInput))]
[MessagePack.Union(1, typeof(NetworkEvent))]
[MessagePack.Union(2, typeof(PlayerConnected))]
[MessagePack.Union(3, typeof(PlayerConnectedConfirmation))]
[MessagePack.Union(4, typeof(NetworkInstantiate))]
[MessagePack.Union(5, typeof(NetworkState))]
public interface INetworkMessage
{
}