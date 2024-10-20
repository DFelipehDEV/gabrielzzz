using Godot;

public partial class Player : Node3D
{
	private Camera3D cam;
	private float rotationSpeed = 0.05f;
	private float targetRotation = 0f;
	private float direction;

	public override void _Ready()
	{
		cam = GetNode<Camera3D>("Camera");
	}

	public override void _Process(double delta)
	{
		targetRotation = Mathf.Lerp(targetRotation, (-direction*1.5f) + Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
		Rotation = new Vector3(Rotation.X, targetRotation, Rotation.Z);
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion) {
			Vector2 mousePos = eventMouseMotion.Position;
		
			Vector2 screenCenter = GetViewport().GetVisibleRect().Size  / 2;
			direction = (mousePos.X - screenCenter.X) / screenCenter.X;
		}
	}	
}
