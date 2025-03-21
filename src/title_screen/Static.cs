using Godot;
using System;

public partial class Static : Sprite2D
{
	private double frame;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		frame += 12.0 * delta;
		frame = frame % 4; 
		
		Frame = (int)(frame);
	}
}
