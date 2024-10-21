using Godot;
using System;

public partial class Radio : StaticBody3D
{
	private bool playing = false;
	private AudioStreamPlayer3D player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<AudioStreamPlayer3D>("MusicPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _InputEvent(Camera3D camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeIdx)
	{
		if (Input.IsActionJustPressed("toggle_flash")) 
		{
			if (!player.Playing) {
				player.Play();
				GD.Print("playing");
			} else {
				player.Stop();
				GD.Print("stopped");
			}
		}
	}
}
