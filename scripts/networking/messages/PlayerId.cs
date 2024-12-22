using System;
using System.Collections.Generic;

namespace powdered_networking.messages;

public record NetworkPlayer(string PlayerId, string PlayerName);

public class PlayerManager
{
    private List<NetworkPlayer> players = new List<NetworkPlayer>();

    public NetworkPlayer NewPlayer(string playerName)
    {
        var player = new NetworkPlayer(GeneratePlayerId(), playerName);
        players.Add(player);
        return player;
    }
    
    private static string GeneratePlayerId()
    {
        return Guid.NewGuid().ToString();
    }
}