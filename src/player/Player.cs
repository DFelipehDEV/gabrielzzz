using Godot;
using System;
using System.Linq;

public partial class Player : Node3D
{
	[Export]
	private PlayerCamera cam;

	[Export]
	private Phone phone;
	public Phone Phone
	{
		get => phone;
	}

	[Export]
	private float phoneRotationSwayAmount = 20.0f;

	[Export]
	private float phoneRotationSwaySpeed = 2.5f;

	[Export]
	private float phonePositionSwayAmount = 0.01f;

	[Export]
	private float phonePositionSwaySpeed = 1.25f;

	[Export]
	private AnimationPlayer animationPlayer;
	public AnimationPlayer AnimationPlayer
	{
		get => animationPlayer;
	}

	[Export]
	private AudioStreamPlayer3D flashlightClickSound;

	[Export]
	private AudioStreamPlayer3D openCameraSound;

	[Export]
	private double timeToExitCameras = 0.75;

	[Export]
	private TextureProgressBar exitCamerasProgressBar;

	[Export]
	private ControlOverlay controlOverlay;

	private float rotationSpeed = 2.5f;
	private Vector2 targetRotation;
	private Vector2 direction;

	private Vector2 mouseMovement;

	private double exitClickHoldTime = 0.0f;

	public enum States
	{
		Default,
		ToCamera,
		Camera,
		Hidden,
		Record,
	}

	private States state = States.Default;
	public States State
	{
		get => state;
		set
		{
			state = value;
			stateTimer = 0;

			controlOverlay.Clear();
			switch (state) {
				case States.Default:
				case States.ToCamera:
					controlOverlay.Add(new ControlOverlay.Control("Toggle Flashlight", controlOverlay.LeftClick));
					controlOverlay.Add(new ControlOverlay.Control("Open Cameras", controlOverlay.RightClick));
					break;
				
				case States.Camera:
					controlOverlay.Add(new ControlOverlay.Control("Exit (Hold)", controlOverlay.LeftClick));
					controlOverlay.Add(new ControlOverlay.Control("Previous Camera", controlOverlay.LeftClick));
					controlOverlay.Add(new ControlOverlay.Control("Next Camera", controlOverlay.RightClick));
					controlOverlay.Add(new ControlOverlay.Control("Select Camera 1–8", controlOverlay.Layout1to8));
					break;
				
				case States.Hidden:
					controlOverlay.Add(new ControlOverlay.Control("Exit", controlOverlay.LeftClick));
					break;

				case States.Record:
					controlOverlay.Add(new ControlOverlay.Control("Exit", controlOverlay.LeftClick));
					controlOverlay.Add(new ControlOverlay.Control("Record (When Hovering)", controlOverlay.LeftClick));
					break;
			}
		}
	}

	private float stateTimer = 0.0f;
	private int currentCamera = 0;
	private Godot.Collections.Array<RoomCamera> cameras;

	private Interactable focusedInteractable;
	public Interactable FocusedInteractable
	{
		get => focusedInteractable;
	}
	private bool isMousePressed = false;

	public override void _Ready()
	{
		// This is to force the control overlay to show the controls
		State = States.Default;
		var camerasGroup = GetTree().GetNodesInGroup("cameras")
				.OfType<RoomCamera>()
				.OrderBy(camera => camera.GlobalTransform.Origin.DistanceTo(GlobalTransform.Origin))
				.ToArray();
		cameras = new Godot.Collections.Array<RoomCamera>(camerasGroup);
	}

	public override void _Process(double delta)
	{
		if (focusedInteractable != null)
		{
			bool shouldInteract = Input.IsActionPressed("toggle_flash") && focusedInteractable.IsInteractable;

			if (shouldInteract)
				focusedInteractable.StartInteract();
			else
				focusedInteractable.StopInteract();
		}

		switch (state)
		{
			case States.Default:
				if (animationPlayer.CurrentAnimation == "")
				{
					targetRotation.X = Mathf.Lerp(targetRotation.X, (-direction.X * 1.5f) + Mathf.DegToRad(180), (float)GetProcessDeltaTime() * rotationSpeed);
					targetRotation.Y = Mathf.Lerp(targetRotation.Y, (-direction.Y * 1.5f), (float)GetProcessDeltaTime() * rotationSpeed);
					Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);

					Vector2 swayInput = mouseMovement * 0.05f;
					Vector3 phoneSwayRotation = new Vector3(
						Mathf.Clamp(-swayInput.Y, -0.5f, 0.5f) * Mathf.DegToRad(phoneRotationSwayAmount),
						0.0f,
						Mathf.Clamp(-swayInput.X, -0.5f, 0.5f) * Mathf.DegToRad(phoneRotationSwayAmount)
					);
					phone.Holder.Rotation = phone.Holder.Rotation.Lerp(phoneSwayRotation, phoneRotationSwaySpeed * (float)delta);

					Vector3 phoneSwayPosition = new Vector3(
						Mathf.Clamp(swayInput.X, -phonePositionSwayAmount, phonePositionSwayAmount),
						Mathf.Clamp(swayInput.Y, -phonePositionSwayAmount, phonePositionSwayAmount),
						0.0f
					);

					phone.Holder.Position = phone.Holder.Position.Lerp(phoneSwayPosition, phonePositionSwaySpeed * (float)delta);
				}

				// Flashlight
				if (Input.IsActionJustPressed("toggle_flash") && phone.On)
				{
					phone.Flash = !phone.Flash;
					flashlightClickSound.Play();
				}

				// See Cameras
				if (Input.IsActionJustPressed("enter_camera") && stateTimer > 0.75f && phone.On)
				{
					State = States.ToCamera;
					phone.Animation = Phone.Animations.OpenCamera;
					phone.Flash = false;
					openCameraSound.Play();
				}

				break;


			case States.Hidden:
				targetRotation.X = Mathf.Lerp(targetRotation.X, Mathf.DegToRad(180), (float)GetProcessDeltaTime() * 2.5f);
				targetRotation.Y = Mathf.Lerp(targetRotation.Y, Mathf.DegToRad(0), (float)GetProcessDeltaTime() * 2.5f);
				Rotation = new Vector3(targetRotation.Y, targetRotation.X, Rotation.Z);
				// Unhide
				if (Input.IsActionJustPressed("toggle_flash") && animationPlayer.CurrentAnimation != "HideUnderTable")
				{
					animationPlayer.Play("UnhideUnderTable");
					Rotation = new Vector3(Mathf.DegToRad(0), Mathf.DegToRad(180), Rotation.Z);
					State = States.Default;
				}
				break;

			case States.ToCamera:
				if (stateTimer > 0.4f)
				{
					SwitchCamera(1);
					State = States.Camera;
					ToggleCameraUI(true);
				}
				break;

			case States.Camera:
				// Change to previous camera
				if (Input.IsActionJustReleased("previous_camera") && focusedInteractable == null)
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
				if (Input.IsActionJustReleased("next_camera"))
				{
					if (currentCamera < cameras.Count - 1)
					{
						SwitchCamera(currentCamera + 1);
					}
					else
					{
						SwitchCamera(1);
					}
				}


				if ((Input.IsActionPressed("previous_camera") && !Input.IsActionJustPressed("previous_camera")
				|| Input.IsActionPressed("next_camera") && !Input.IsActionJustPressed("next_camera")) && focusedInteractable == null)
				{
					exitClickHoldTime += 1.0 * delta;
				}
				else
				{
					exitClickHoldTime = 0.0;
					exitCamerasProgressBar.Visible = false;
				}

				// Have a delay to avoid visual clutter
				if (exitClickHoldTime > 0.2 && focusedInteractable == null)
				{
					exitCamerasProgressBar.Visible = true;
					exitCamerasProgressBar.Value = exitClickHoldTime * 100 * timeToExitCameras;
					exitCamerasProgressBar.GlobalPosition = GetViewport().GetMousePosition();

					// Hold to exit the cameras
					if (exitClickHoldTime > timeToExitCameras)
					{
						SwitchCamera(0);

						State = States.Default;
						phone.Animation = Phone.Animations.CloseCamera;
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

				if (Input.IsActionJustPressed("leave_cameras") || !phone.On)
				{
					SwitchCamera(0);

					State = States.Default;
					phone.Animation = Phone.Animations.CloseCamera;
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
			case States.Record:
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

			mouseMovement = mouseMotion.Relative;

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

		// Stop previous interaction if focused interactable changed
		if (previousFocused != null && focusedInteractable != previousFocused)
		{
			previousFocused.StopInteract();
		}

		if (previousFocused != null) previousFocused.Unhighlight();
		if (focusedInteractable != null) focusedInteractable.Highlight();
	}

	private void SwitchCamera(int newCameraIndex)
	{
		cameras[currentCamera].Used = false;

		currentCamera = newCameraIndex;

		cameras[currentCamera].Used = true;

		if (cameras[currentCamera].Environment != null)
		{
			cameras[currentCamera].Environment.TonemapExposure = 0.075f;
		}
	}

	private void ToggleCameraUI(bool visible)
	{
		phone.CameraUI.Visible = visible;
		cam.Posterize.Visible = visible;
		cam.Grain.Visible = visible;
	}
}
