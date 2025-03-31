using Godot;
using System.Linq;

public partial class Player : Node3D
{
	private PlayerCamera cam;
	public Phone phone;
	public AnimationPlayer animationPlayer;

	private float rotationSpeed = 2.5f;
	private Vector2 targetRotation;
	private Vector2 direction;

	public enum States
	{
		NORMAL,
		TOCAMERA,
		CAMERA,
		HIDDEN,
		RECORD,
	}

	private States state = States.NORMAL;
	public States State
	{
		get => state;
		set
		{
			state = value;
			stateTimer = 0;
		}
	}

	private float stateTimer = 0.0f;
	private int currentCamera = 0;
	private Godot.Collections.Array<RoomCamera> cameras;

	private Interactable focusedInteractable;
	public Interactable FocusedInteractable {
		get => focusedInteractable;
	}
	private bool isMousePressed = false;

	public override void _Ready()
	{
		cam = GetNode<PlayerCamera>("Root/Camera");
		phone = GetNode<Phone>("Root/Phone");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		var camerasGroup = GetTree().GetNodesInGroup("cameras").Select(x => (RoomCamera)x).ToArray();
		cameras = new Godot.Collections.Array<RoomCamera>(camerasGroup);
	}

	public override void _Process(double delta)
	{
		if (focusedInteractable != null)
		{
			bool shouldInteract = isMousePressed && focusedInteractable.IsInteractable;

			if (shouldInteract)
				focusedInteractable.StartInteract();
			else
				focusedInteractable.StopInteract();
		}

		switch (state)
		{
			case States.NORMAL:
				if (animationPlayer.CurrentAnimation == "")
				{
					targetRotation.X = Mathf.Lerp(targetRotation.X, (-direction.X * 1.5f) + Mathf.DegToRad(180), (float)GetProcessDeltaTime() * rotationSpeed);
					targetRotation.Y = Mathf.Lerp(targetRotation.Y, (-direction.Y * 1.5f), (float)GetProcessDeltaTime() * rotationSpeed);
					Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);
				}

				// Flashlight
				if (Input.IsActionJustPressed("toggle_flash"))
				{
					phone.Flash = !phone.Flash;
				}

				// Hide
				if (Input.IsActionJustPressed("hide"))
				{
					if (animationPlayer.CurrentAnimation != "UnhideUnderTable")
					{
						animationPlayer.Play("HideUnderTable");
						state = States.HIDDEN;
					}
				}

				if (Input.IsActionJustPressed("enter_camera"))
				{
					State = States.TOCAMERA;
					phone.Animation = Phone.Animations.OPEN_CAMERA;
				}

				break;


			case States.HIDDEN:
				targetRotation.X = Mathf.Lerp(targetRotation.X, Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
				targetRotation.Y = Mathf.Lerp(targetRotation.Y, Mathf.DegToRad(0), (float)GetProcessDeltaTime() * 2.5f);
				Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);
				// Unhide
				if (Input.IsActionJustPressed("hide") && animationPlayer.CurrentAnimation != "HideUnderTable")
				{
					animationPlayer.Play("UnhideUnderTable");
					Rotation = new Vector3(Mathf.DegToRad(0), Mathf.DegToRad(180), Rotation.Z);
					state = States.NORMAL;
				}
				break;

			case States.TOCAMERA:
				if (stateTimer > 0.4f)
				{
					SwitchCamera(1);
					State = States.CAMERA;
					ToggleCameraUI(true);
				}
				break;

			case States.CAMERA:
				// Change to previous camera
				if (Input.IsActionJustPressed("previous_camera") && focusedInteractable == null)
				{
					if (currentCamera > 1)
					{
						SwitchCamera(currentCamera - 1);
					}
					else
					{
						SwitchCamera(cameras.Count - 1);
					}
				}

				// Change to next camera
				if (Input.IsActionJustPressed("next_camera"))
				{
					if (currentCamera < cameras.Count - 1)
					{
						SwitchCamera(currentCamera + 1);
					}
					else
					{
						SwitchCamera(0);

						State = States.NORMAL;
						phone.Animation = Phone.Animations.CLOSE_CAMERA;
						ToggleCameraUI(false);
					}
				}

				if (Input.IsActionJustPressed("goto_camera_1"))
				{
					SwitchCamera(1);
				}

				if (Input.IsActionJustPressed("goto_camera_2"))
				{
					SwitchCamera(2);
				}

				if (Input.IsActionJustPressed("goto_camera_3"))
				{
					SwitchCamera(3);
				}

				if (Input.IsActionJustPressed("goto_camera_4"))
				{
					SwitchCamera(4);
				}

				if (Input.IsActionJustPressed("goto_camera_5"))
				{
					SwitchCamera(5);
				}

				if (Input.IsActionJustPressed("goto_camera_6"))
				{
					SwitchCamera(6);
				}

				if (Input.IsActionJustPressed("goto_camera_7"))
				{
					SwitchCamera(7);
				}

				if (Input.IsActionJustPressed("goto_camera_8"))
				{
					SwitchCamera(8);
				}

				if (Input.IsActionJustPressed("leave_cameras"))
				{
					SwitchCamera(0);

					State = States.NORMAL;
					phone.Animation = Phone.Animations.CLOSE_CAMERA;
					ToggleCameraUI(false);
				}

				if (cameras[currentCamera].Environment != null)
				{
					if (cameras[currentCamera].Environment.TonemapExposure < 1.2f)
					{
						cameras[currentCamera].Environment.TonemapExposure += 0.6f * (float)delta;
					}
				}
				break;
			case States.RECORD:
				targetRotation.X = Mathf.Lerp(targetRotation.X, Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
				targetRotation.Y = Mathf.Lerp(targetRotation.Y, Mathf.DegToRad(0), (float)GetProcessDeltaTime() * 2.5f);
				Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);
				break;

		}
		stateTimer += 1.0f * (float)delta;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			Vector2 screenCenter = GetViewport().GetVisibleRect().Size / 2;
			direction.X = (mouseMotion.Position.X - screenCenter.X) / screenCenter.X;
			direction.Y = (mouseMotion.Position.Y - screenCenter.Y) / screenCenter.Y;

			// Update mouse over state
			UpdateMouseOver(mouseMotion.Position);
		}
		else if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
		{
			isMousePressed = mouseButton.Pressed;

			if (mouseButton.Pressed && focusedInteractable != null)
			{
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private void UpdateMouseOver(Vector2 mousePosition)
	{
		var previousFocused = focusedInteractable;
		focusedInteractable = null;

		var camera = GetViewport().GetCamera3D();
		var from = camera.ProjectRayOrigin(mousePosition);
		var to = from + camera.ProjectRayNormal(mousePosition) * 1000;

		var spaceState = GetWorld3D().DirectSpaceState;
		var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollisionMask = 1
		});

		if (result.Count > 0 && result["collider"].AsGodotObject() is Interactable interactable)
		{
			focusedInteractable = interactable;
		}

		if (previousFocused != null) previousFocused.Unhighlight();
		if (focusedInteractable != null && focusedInteractable.IsInteractable) focusedInteractable.Highlight();
	}

	private void SwitchCamera(int newCameraIndex)
	{
		cameras[currentCamera].Current = false;
		cameras[currentCamera].Visible = false;
		cameras[currentCamera].listener.ClearCurrent();

		currentCamera = newCameraIndex;

		cameras[currentCamera].Current = true;
		cameras[currentCamera].Visible = true;
		cameras[currentCamera].listener.MakeCurrent();

		if (cameras[currentCamera].Environment != null)
		{
			cameras[currentCamera].Environment.TonemapExposure = 0.0f;
		}
	}

	private void ToggleCameraUI(bool visible)
	{
		phone.cameraUI.Visible = visible;
		cam.posterize.Visible = visible;
		cam.grain.Visible = visible;
	}

}
