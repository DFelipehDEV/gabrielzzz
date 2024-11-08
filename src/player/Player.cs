using Godot;

public partial class Player : Node3D
{
	private Camera3D cam;
	private Phone phone;
	private float rotationSpeed = 0.05f;
	private float targetRotation = 0f;
	private float direction;
	// TODO: Add flashlight state
	enum States {
		NORMAL,
		CAMERA,
	}
	private States state = States.NORMAL;
	private uint stateTimer = 0;
	private int currentCamera = 0;
	[Export]
	private Godot.Collections.Array<Camera3D> cameras;

	public override void _Ready()
	{
		cam = GetNode<Camera3D>("Camera");
		phone = GetNode<Phone>("Phone");
	}

	public override void _Process(double delta)
	{
		targetRotation = Mathf.Lerp(targetRotation, (-direction*1.5f) + Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
		Rotation = new Vector3(Rotation.X, targetRotation, Rotation.Z);

		if (Input.IsActionJustPressed("toggle_flash")) {
			phone.Flash = !phone.Flash;
		}
		
		if (Input.IsActionJustPressed("toggle_camera")) {
			switch (state) {
				case States.NORMAL:
					cameras[0].Current = false;
					cameras[1].Current = true;
					currentCamera = 1;
					state = States.CAMERA;
					break;
				
				case States.CAMERA:
					if (currentCamera < cameras.Count - 1)  {
						cameras[currentCamera].Current = false;
						currentCamera++;
						cameras[currentCamera].Current = true;
					} else {
						cameras[currentCamera].Current = false;
						currentCamera = 0;
						cameras[currentCamera].Current = true;
						state = States.NORMAL;
					}
					break;
			}
			GD.Print(state.ToString());
			GD.Print(currentCamera);
		}
		stateTimer++;
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
