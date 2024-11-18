using Godot;

public partial class Player : Node3D
{
	private Camera3D cam;
	private Node3D eye;
	private Phone phone;
	private Vector3 normalPhonePosition;
	private float rotationSpeed = 0.05f;
	private Vector2 targetRotation;
	private Vector2 direction;
	// TODO: Add flashlight state
	public enum States {
		NORMAL,
		TOCAMERA,
		CAMERA,
	}
	private States state = States.NORMAL;
	public States State {
		get {
			return state;
		}
		set {
			state = value;
			stateTimer = 0;
		}
	}
	private uint stateTimer = 0;
	private int currentCamera = 0;
	[Export]
	private Godot.Collections.Array<Camera3D> cameras;

	public override void _Ready()
	{
		cam = GetNode<Camera3D>("Camera");
		phone = GetNode<Phone>("Phone");
		eye = GetNode<Node3D>("Eye");
		normalPhonePosition = phone.Position;
	}

	public override void _Process(double delta)
	{
		targetRotation.X = Mathf.Lerp(targetRotation.X, (-direction.X * 1.5f) + Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
		targetRotation.Y = Mathf.Lerp(targetRotation.Y, (-direction.Y * 1.5f), (float)GetProcessDeltaTime() * 2.5f);
		Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);

		if (Input.IsActionJustPressed("toggle_flash")) {
			phone.Flash = !phone.Flash;
		}
		
		if (Input.IsActionJustPressed("toggle_camera")) {
			switch (State) {
				case States.NORMAL:
					State = States.TOCAMERA;
					Tween toEyePosition = CreateTween().SetTrans(Tween.TransitionType.Quad);
					toEyePosition.TweenProperty(phone, "position", eye.Position, 0.5);
					Tween toEyeRotation = CreateTween().SetTrans(Tween.TransitionType.Quad);
					toEyeRotation.TweenProperty(phone, "rotation_degrees", new Vector3(0.0f, 0.0f, 90.0f), 0.4);
					break;
				
				case States.CAMERA:
					if (currentCamera < cameras.Count - 1)  {
						cameras[currentCamera].Current = false;
						currentCamera++;
						cameras[currentCamera].Current = true;
						cameras[currentCamera].Environment.TonemapExposure = 0.0f;
					} else {
						// Reset
						cameras[currentCamera].Current = false;
						currentCamera = 0;
						cameras[currentCamera].Current = true;
						State = States.NORMAL;
						Tween toNormalPosition = CreateTween().SetTrans(Tween.TransitionType.Quad);
						toNormalPosition.TweenProperty(phone, "rotation_degrees", new Vector3(0.0f, 0.0f, 0.0f), 0.5);	
						Tween toNormalRotation = CreateTween().SetTrans(Tween.TransitionType.Quad);	
						toNormalRotation.TweenProperty(phone, "position", normalPhonePosition, 0.3);			
						phone.cameraUI.Visible = false;
					}
					break;
			}
		}

		switch (State) {
				case States.TOCAMERA:
					if (stateTimer > 70) {
						cameras[0].Current = false;
						cameras[1].Current = true;
						cameras[1].Environment.TonemapExposure = 0.0f;
						currentCamera = 1;
						State = States.CAMERA;
						phone.cameraUI.Visible = true;
					} 
					break;

				case States.CAMERA:
					if (cameras[currentCamera].Environment.TonemapExposure < 1.2f) {
						cameras[currentCamera].Environment.TonemapExposure += 0.6f * (float)delta;
					}
					break;
			
		}
		stateTimer++;
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion) {
			Vector2 mousePos = eventMouseMotion.Position;
		
			Vector2 screenCenter = GetViewport().GetVisibleRect().Size  / 2;
			direction.X = (mousePos.X - screenCenter.X) / screenCenter.X;
			direction.Y = (mousePos.Y - screenCenter.Y) / screenCenter.Y;
		}
	}	
}
