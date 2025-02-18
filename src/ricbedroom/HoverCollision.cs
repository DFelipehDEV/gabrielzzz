using Godot;
using System;

public partial class HoverCollision : Area3D
{
	public Sprite3D progress;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		progress = GetNode<Sprite3D>("../Progress");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnMouseEntered() {
		progress.Visible = true;
	}
	
	private void OnMouseExited() {
		progress.Visible = false;
	}




}
