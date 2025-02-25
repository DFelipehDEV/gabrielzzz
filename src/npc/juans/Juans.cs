using Godot;

public partial class Juans : EnemyNPC
{
	public Light3D flashLight;
	public bool flashed = false;

	public override void _Ready()
	{
		base._Ready();
		flashLight = GetTree().Root.GetNode<Light3D>("Node3D/Player/Root/Phone/Flash");
	}
	public override void MovedToNewPosition(Node3D position) {
		if (position.IsInGroup("kitchen")) {
			position.GetNode<AudioStreamPlayer3D>("Faucet").Play();
		}

		if (position.IsInGroup("office")) {
			if (flashLight.Visible) {
				GetNode<AudioStreamPlayer3D>("FlashedSound").Play();
			} else {
				GetNode<AudioStreamPlayer3D>("OfficeSound").Play();
			}
		}
	}
}
