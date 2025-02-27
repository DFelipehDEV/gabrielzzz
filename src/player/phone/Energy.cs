using Godot;
using System;

public partial class Energy : Label3D
{
	public double energy = 100.0;
	public double wasteMultiplier = 1.0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		energy -= (0.2 * wasteMultiplier) * delta;
		Text = ((int)energy).ToString();
	}
}
