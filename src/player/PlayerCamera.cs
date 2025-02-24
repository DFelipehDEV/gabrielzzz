using Godot;
using System;

public partial class PlayerCamera : RoomCamera
{
	public CanvasLayer posterize, grain;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		
		posterize = GetNode<CanvasLayer>("Posterize");
		grain = GetNode<CanvasLayer>("Grain");
	}
}
