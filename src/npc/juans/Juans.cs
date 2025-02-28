using Godot;

public partial class Juans : EnemyNPC
{
	public AudioStreamPlayer3D faucetSound, flashedSound, officeSound;
	public Light3D flashLight;
	public bool flashed = false;

	public override void _Ready()
	{
		base._Ready();
		faucetSound = GetNode<AudioStreamPlayer3D>("KitchenFaucet");
		flashedSound = GetNode<AudioStreamPlayer3D>("FlashedSound");
		officeSound = GetNode<AudioStreamPlayer3D>("OfficeSound");
		flashLight = GetTree().Root.GetNode<Light3D>("Node3D/Player/Root/Phone/Flash");
	}
	public override void MovedToNewPosition(Node3D position) {
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
