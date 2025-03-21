using Godot;

[GlobalClass]
public partial class RoomCamera : Camera3D
{
	public AudioListener3D listener;

	public override void _Ready()
	{
		listener = new AudioListener3D();
		AddChild(listener);
	}
}
