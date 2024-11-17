using Godot;

public partial class Phone : Node3D
{
	private SpotLight3D light;
	public CanvasLayer cameraUI { get; set; }
	private double time = 0;
	private Label3D timeLabel;
	private CsgBox3D flashIcon;
	private bool flash;
	public static readonly float HOUR_LENGTH = 1.2f * 60.0f; // Takes 72 seconds to go from 00:00 to 01:00
	public static readonly uint NIGHT_LENGTH = 6;
	
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
		timeLabel = GetNode<Label3D>("Time");
		cameraUI = GetNode<CanvasLayer>("CameraUI");

		// Store the original material and load the alternate material.
		offMaterial = flashIcon.Material;
		onMaterial = (Material)GD.Load("res://player/phone/ui/FlashlightOnIcon.tres");
	}

	public override void _Process(double delta)
	{	
		time += (delta / HOUR_LENGTH);
		time %= 6;
		int hour = ((int)time);
		timeLabel.Text = $"{hour:D2}:00";
	}
}
