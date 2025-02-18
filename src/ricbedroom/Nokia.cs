using Godot;
using System;

public partial class Nokia : StaticBody3D
{
	public Sprite3D progress;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		progress = GetNode<Sprite3D>("Model/Progress");
	}
}
