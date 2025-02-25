using Godot;

public partial class Phone : Node3D
{
	private SpotLight3D light;
	public CanvasLayer cameraUI { get; set; }

	private double time = 0;
	private Label3D timeLabel;

	private bool flash;
	private CsgBox3D flashIcon;

	[Export]
	private Node3D eye;

	private Vector3 normalPhonePosition;

	public enum Animations {
		DEFAULT,
		OPEN_CAMERA,
		CLOSE_CAMERA,
	}

	private Animations animation;
	
	public static readonly float HOUR_LENGTH = 1.2f * 60.0f; // Takes 72 seconds to go from 00:00 to 01:00
	public static readonly uint NIGHT_LENGTH = 6;

	public Animations Animation {
		get {
			return animation;
		}
		set {
			animation = value;
			// Animation
			switch (animation) {
			case Animations.OPEN_CAMERA:
				Tween toEyePosition = CreateTween().SetTrans(Tween.TransitionType.Quad);
				toEyePosition.TweenProperty(this, "position", eye.Position, 0.5);
				Tween toEyeRotation = CreateTween().SetTrans(Tween.TransitionType.Quad);
				toEyeRotation.TweenProperty(this, "rotation_degrees", new Vector3(0.0f, 0.0f, 90.0f), 0.4);
				break;

			case Animations.CLOSE_CAMERA:	
				Tween toNormalPosition = CreateTween().SetTrans(Tween.TransitionType.Quad);
				toNormalPosition.TweenProperty(this, "rotation_degrees", new Vector3(0.0f, 0.0f, 0.0f), 0.5);	
				Tween toNormalRotation = CreateTween().SetTrans(Tween.TransitionType.Quad);	
				toNormalRotation.TweenProperty(this, "position", normalPhonePosition, 0.3);	
				break;
		}
		}
	}
	
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

		normalPhonePosition = Position;

		// Store the original material and load the alternate material.
		offMaterial = flashIcon.Material;
		onMaterial = (Material)GD.Load("res://player/phone/ui/FlashlightOnIcon.tres");
	}

	public override void _Process(double delta)
	{	
		time += (delta / HOUR_LENGTH);
		int hour = ((int)time);
		timeLabel.Text = $"{hour:D2}:00";
		if (hour == 6){
			var nightendScene = ResourceLoader.Load<PackedScene>("res://NightEnd/NightEnd.tscn").Instantiate();
			GetTree().Root.AddChild(nightendScene);
		}
	}
}
