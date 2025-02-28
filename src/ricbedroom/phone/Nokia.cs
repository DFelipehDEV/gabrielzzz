using Godot;
using System;

public partial class Nokia : Node3D
{
	private TextureProgressBar progress;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		progress = GetNode<TextureProgressBar>("Progress/SubViewport/TextureProgressBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		progress.Value += 1;
		progress.Value %= 100;
	}
}
