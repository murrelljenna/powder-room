using System;
using System.Collections.Generic;
using System.Net;

namespace powdered_networking.messages;

public record NetworkPlayer(string PlayerId, string PlayerName, IPEndPoint remote);

public class PlayerManager
{
    public List<NetworkPlayer> players = new List<NetworkPlayer>();

    public NetworkPlayer NewPlayer(string playerName, IPEndPoint remote)
    {
        var player = new NetworkPlayer(GeneratePlayerId(), playerName, remote);
        players.Add(player);
        return player;
    }
    
    private static string GeneratePlayerId()
    {
        return Guid.NewGuid().ToString();
    }
}