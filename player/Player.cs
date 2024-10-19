using Godot;
using System;

public partial class Player : Node3D
{
	private Camera3D cam;
	private float rotationSpeed = 0.05f; // Adjust for desired rotation speed
	private float targetRotation = 0f;
	private float maxRotation = Mathf.Pi / 8; // Maximum rotation angle (45 degrees)

	public override void _Ready()
	{
		cam = GetNode<Camera3D>("Camera");
	}

	public override void _Process(double delta)
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 screenCenter = GetViewport().GetVisibleRect().Size / 2;
		float direction = (mousePos.X - screenCenter.X) / screenCenter.X;
		
		targetRotation = Mathf.Lerp(targetRotation, -direction + Mathf.DegToRad(180), (float)delta * 2);

		Rotation = new Vector3(Rotation.X, targetRotation, Rotation.Z);
	}
}
