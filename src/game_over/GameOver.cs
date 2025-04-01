using Godot;
using System;

public partial class GameOver : Control
{

	private float time = 0;

	public override void _Process (double delta)
	{
		time += (float)delta;
		if (time >= 5)
			GetTree().ChangeSceneToFile("res://title_screen/TitleScreen.tscn");
	}
}
