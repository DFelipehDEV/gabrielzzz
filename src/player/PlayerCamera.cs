using Godot;

public partial class PlayerCamera : RoomCamera
{
    [Export]
    public CanvasLayer Posterize { get; private set; }

    [Export]
    public CanvasLayer Grain { get; private set; }
}
