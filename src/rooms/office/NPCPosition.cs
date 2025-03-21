using Godot;

[GlobalClass]
public partial class NPCPosition : Node3D
{
	[Export]
	public Godot.Collections.Array<string> nearRoomsGroup;
}
