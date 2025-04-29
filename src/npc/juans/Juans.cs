using Godot;
using System;

public partial class Juans : EnemyNPC
{
	[Export]
	private AudioStreamPlayer3D faucetSound, flashedSound, officeSound;

	private Light3D flashLight;

	private Generator generator;

	private Player player;

	public override void _Ready()
	{
		base._Ready();
		generator = GetTree().GetFirstNodeInGroup("generator") as Generator;
		player = GetTree().GetFirstNodeInGroup("player") as Player;
		flashLight = player.Phone.Light;
	}
	public override void OnMovedToNewPosition(Node3D position)
	{
		if (position.IsInGroup("garage"))
		{
			// Break generator
			if (generator != null && !generator.Broken)
			{
				generator.Broken = true;
				GD.Print("Generator has been broken");
			}
		}

		if (position.IsInGroup("kitchen"))
		{
			faucetSound.Play();
		}

		if (position.IsInGroup("office"))
		{
			if (flashLight.Visible)
			{
				flashedSound.Play();
			}
			else
			{
				officeSound.Play();
			}
		}
	}
}
