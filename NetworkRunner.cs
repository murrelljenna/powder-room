using Godot;
using System;
using System.Threading.Tasks;
using MessagePack;
using powdered_networking;

public partial class NetworkRunner : GodotSocketClient
{
	public override NetworkInput PollInput()
	{
		NetworkInput input = new NetworkInput();
		
		if (Input.IsActionPressed("ui_up"))
		{
			
		}

		if (Input.IsActionPressed("sprint"))
		{
			//input.Sprint = true;
		}

		return input;
	}
}
