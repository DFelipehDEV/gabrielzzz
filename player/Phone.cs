using Godot;

public partial class Phone : Node3D
{
	private SpotLight3D light;
	private CsgBox3D flashIcon;
	private bool flash;
	public bool Flash {
		get {
			return flash;
		}
		set {
			flash = value;
			light.Visible = value;

			flashIcon.Material = value ? onMaterial : offMaterial;
		}
	}
	private Material offMaterial, onMaterial;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		light = GetNode<SpotLight3D>("Flash");
		flashIcon = GetNode<CsgBox3D>("FlashIcon");

		// Store the original material and load the alternate material.
		offMaterial = flashIcon.Material;
		onMaterial = (Material)GD.Load("res://office/phone/ui/FlashlightOnIcon.tres");
	}
}
