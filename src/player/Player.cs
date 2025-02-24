using Godot;
using System.Linq;

public partial class Player : Node3D
{
	private PlayerCamera cam;
	private Node3D eye;
	private Phone phone;
	private AnimationPlayer animationPlayer;
	private Vector3 normalPhonePosition;
	private float rotationSpeed = 0.05f;
	private Vector2 targetRotation;
	private Vector2 direction;
	// TODO: Add flashlight state
	public enum States {
		NORMAL,
		TOCAMERA,
		CAMERA,
		TOHIDDEN,
		HIDDEN,
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
	private float stateTimer = 0;
	private int currentCamera = 0;
	private Godot.Collections.Array<RoomCamera> cameras;

	private Node3D focused;

	public override void _Ready()
	{
		cam = GetNode<PlayerCamera>("Root/Camera");
		phone = GetNode<Phone>("Root/Phone");
		eye = GetNode<Node3D>("Root/Eye");
		var camerasGroup = GetTree().GetNodesInGroup("cameras").Select(x => (RoomCamera)x).ToArray();
		cameras = new Godot.Collections.Array<RoomCamera>(camerasGroup);

		focused = null;
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		normalPhonePosition = phone.Position;
	}

	public override void _Process(double delta)
	{
		if (state == States.NORMAL && animationPlayer.CurrentAnimation == "" ) {
			targetRotation.X = Mathf.Lerp(targetRotation.X, (-direction.X * 1.5f) + Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
			targetRotation.Y = Mathf.Lerp(targetRotation.Y, (-direction.Y * 1.5f), (float)GetProcessDeltaTime() * 2.5f);
			Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);
		}

		if (Input.IsActionJustPressed("toggle_flash")) {
			phone.Flash = !phone.Flash;
			
			if(state == States.CAMERA){
				if (focused == null) {
					if (currentCamera > 1)  {
							cameras[currentCamera].Current = false;
							cameras[currentCamera].listener.ClearCurrent();
							currentCamera--;
							cameras[currentCamera].Current = true;
							cameras[currentCamera].listener.MakeCurrent();
							cameras[currentCamera].Environment.TonemapExposure = 0.0f;
					} else {
							// Reset
							cameras[currentCamera].Current = false;
							cameras[currentCamera].listener.ClearCurrent();
							currentCamera = cameras.Count - 1;
							cameras[currentCamera].Current = true;
							cameras[currentCamera].listener.MakeCurrent();
					}
				} else {
					if (focused.Name == "Nokia") {
						Sprite2D progressBar = focused.GetNode<Sprite2D>("Model/Progress");

						progressBar.Visible = true;
					}
				}
			}
		}
		
		if (Input.IsActionJustPressed("hide")) {
			if (state == States.NORMAL && animationPlayer.CurrentAnimation != "UnhideUnderTable") {
				
				animationPlayer.Play("HideUnderTable");
				state = States.TOHIDDEN;
			} else {
				animationPlayer.Play("UnhideUnderTable");
				Rotation = new Vector3(Mathf.DegToRad(0), Mathf.DegToRad(180), Rotation.Z);
				state = States.NORMAL;
			}
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
						cameras[currentCamera].listener.ClearCurrent();
						currentCamera++;
						cameras[currentCamera].Current = true;
						cameras[currentCamera].listener.MakeCurrent();
						cameras[currentCamera].Environment.TonemapExposure = 0.0f;
					} else {
						// Reset
						cameras[currentCamera].Current = false;
						cameras[currentCamera].listener.ClearCurrent();
						currentCamera = 0;
						cameras[currentCamera].Current = true;
						cameras[currentCamera].listener.MakeCurrent();
						State = States.NORMAL;
						Tween toNormalPosition = CreateTween().SetTrans(Tween.TransitionType.Quad);
						toNormalPosition.TweenProperty(phone, "rotation_degrees", new Vector3(0.0f, 0.0f, 0.0f), 0.5);	
						Tween toNormalRotation = CreateTween().SetTrans(Tween.TransitionType.Quad);	
						toNormalRotation.TweenProperty(phone, "position", normalPhonePosition, 0.3);			
						phone.cameraUI.Visible = false;
						cam.posterize.Visible = false;
						cam.grain.Visible = false;
					}
					break;
			}
		}

		switch (State) {
				case States.TOCAMERA:
					if (stateTimer > 0.4f) {
						cameras[0].Current = false;
						cameras[1].Current = true;
						cameras[1].Environment.TonemapExposure = 0.0f;
						currentCamera = 1;
						State = States.CAMERA;
						phone.cameraUI.Visible = true;
						cam.posterize.Visible = true;
						cam.grain.Visible = true;
					} 
					break;

				case States.CAMERA:
					if (cameras[currentCamera].Environment.TonemapExposure < 1.2f) {
						cameras[currentCamera].Environment.TonemapExposure += 0.6f * (float)delta;
					}
					break;
					
				case States.TOHIDDEN:
					targetRotation.X = Mathf.Lerp(targetRotation.X, Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
					targetRotation.Y = Mathf.Lerp(targetRotation.Y, Mathf.DegToRad(0 ), (float)GetProcessDeltaTime() * 2.5f);
					Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);
					break;
		}
		stateTimer += 1.0f * (float)delta;
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
