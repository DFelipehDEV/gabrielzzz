using Godot;

[GlobalClass]
public partial class RoomArea : Area3D
{
	[Export]
	private RoomCamera roomCamera;

	public override void _Ready() {
		base._Ready();
		if (roomCamera == null) {
			GD.PrintErr("Missing field 'roomCamera'");
		}
		AreaEntered += DeactivateCamera;
		AreaExited += DeactivateCamera;
	}

	private void DeactivateCamera(Node3D body) {
		roomCamera.Deactivate(1.0);
	}
}
