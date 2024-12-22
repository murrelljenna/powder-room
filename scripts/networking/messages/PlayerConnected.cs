using MessagePack;

namespace powdered_networking.messages;

[MessagePackObject]
public class PlayerConnected : INetworkMessage
{
    [Key(0)]
    public string playerName { get; set; }
    public PlayerConnected(string playerName)
    {
        this.playerName = playerName;
    }
}

[MessagePackObject]
public class PlayerConnectedConfirmation : INetworkMessage
{
    [Key(0)]
    public string playerId { get; }

    public PlayerConnectedConfirmation(string playerId)
    {
        this.playerId = playerId;
    }
}