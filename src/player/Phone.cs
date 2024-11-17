using Godot;

public partial class Phone : Node3D
{
	private SpotLight3D light;
	private double time = 0;
	private Label3D timeLabel;
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
		timeLabel = GetNode<Label3D>("Time");

		// Store the original material and load the alternate material.
		offMaterial = flashIcon.Material;
		onMaterial = (Material)GD.Load("res://player/phone/ui/FlashlightOnIcon.tres");
	}

    public override void _Process(double delta)
    {
        time += 0.02f * delta;
		int hour = ((int)time);
		timeLabel.Text = $"{hour:D2}:00";
    }
}
