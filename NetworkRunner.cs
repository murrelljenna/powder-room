using Godot;
using System;
using System.Threading.Tasks;
using MessagePack;
using powdered_networking;

public partial class NetworkRunner : GodotSocketClient
{
	[Export]
	public PackedScene playerPrefab;
	public override NetworkInput PollInput()
	{
		NetworkInput input = new NetworkInput();
		
		if (Input.IsActionPressed("ui_up"))
		{
			
		}

		if (Input.IsActionPressed("sprint"))
		{
			input.Sprint = true;
		}

		return input;
	}

	public override PackedScene InstantiateNode(string nodeName)
	{
		switch (nodeName)
		{
			case "player":
				return playerPrefab;
			
			default:
				throw new ArgumentException($"Invalid node name: {nodeName}", nameof(nodeName));
				
		}
	}
}
