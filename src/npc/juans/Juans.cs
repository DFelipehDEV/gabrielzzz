using Godot;
using System;

public partial class Juans : EnemyNPC
{
	public AudioStreamPlayer3D faucetSound, flashedSound, officeSound;
	public Light3D flashLight;
	public bool flashed = false;

	private Generator generator;

	public override void _Ready()
	{
		base._Ready();
		generator = (Generator)GetTree().GetFirstNodeInGroup("generator");

		faucetSound = GetNode<AudioStreamPlayer3D>("KitchenFaucet");
		flashedSound = GetNode<AudioStreamPlayer3D>("FlashedSound");
		officeSound = GetNode<AudioStreamPlayer3D>("OfficeSound");
		flashLight = GetTree().Root.GetNode<Light3D>("Node3D/Player/Root/Phone/Flash");
	}
	public override void OnMovedToNewPosition(Node3D position) {
		if (position.IsInGroup("garage"))
		{
			// Break generator
			if (generator != null && !generator.Broken) 
			{
				// 1 in 3 chance to break the generator
				Random random = new Random();
				int chance = random.Next(1, 4);

				if (chance == 1) // 1 in 3 chance
				{
					generator.Broken = true;
					GD.Print("Generator has been broken");
				}
			}
		}

		if (position.IsInGroup("kitchen")) {
			faucetSound.Play();
		}

		if (position.IsInGroup("office")) {
			if (flashLight.Visible) {
				flashedSound.Play();
			} else {
				officeSound.Play();
			}
		}
	}
}
