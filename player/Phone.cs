using Godot;
using System;

public partial class Phone : Node3D
{
	private SpotLight3D light;
	private CsgBox3D flashIcon;
	private Material offMaterial;
	private Material onMaterial;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		light = GetNode<SpotLight3D>("Flash");
		flashIcon = GetNode<CsgBox3D>("FlashIcon");

		// Store the original material and load the alternate material.
		offMaterial = flashIcon.Material;
		onMaterial = (Material)GD.Load("res://office/phone/ui/FlashlightOnIcon.tres");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("toggle_flash"))
		{
			light.Visible = !light.Visible;

			if (light.Visible)
			{
				flashIcon.Material = onMaterial;
			}
			else
			{
				flashIcon.Material = offMaterial;
			}
		}
	}
}
