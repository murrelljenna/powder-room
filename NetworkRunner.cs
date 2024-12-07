using Godot;
using System;
//using powdered_networking;

public partial class NetworkRunner : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Before");
		Server.StartServer();
		GD.Print("After");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
