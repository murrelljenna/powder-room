using Godot;
using System;
using powdered_networking;

public partial class StarterScene : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		powdered_networking.Server.StartServer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
