using Godot;

public partial class Phone : Node3D
{
	[Export]
	public Light3D Light { get; private set; }

	[Export]
	public CanvasLayer CameraUI { get; private set; }

	[Export]
	public Label3D TimeLabel { get; private set; }

	[Export]
	public CsgBox3D FlashIcon { get; private set; }

	[Export]
	private Node3D eye;

	[Export]
	public Energy Energy { get; private set; }

	private bool flash;
	private Vector3 normalPhonePosition;
	private Material offMaterial;

	[Export]
	private Material onMaterial;

	public enum Animations
	{
		Default,
		OpenCamera,
		CloseCamera,
	}

	private Animations animation;
	public Animations Animation
	{
		get => animation;
		set
		{
			animation = value;

			switch (animation)
			{
				case Animations.OpenCamera:
					Tween toEyePosition = CreateTween().SetTrans(Tween.TransitionType.Quad);
					toEyePosition.TweenProperty(this, "position", eye.Position, 0.5);
					Tween toEyeRotation = CreateTween().SetTrans(Tween.TransitionType.Quad);
					toEyeRotation.TweenProperty(this, "rotation_degrees", new Vector3(0.0f, 0.0f, 90.0f), 0.4);
					break;

				case Animations.CloseCamera:
					Tween toNormalPosition = CreateTween().SetTrans(Tween.TransitionType.Quad);
					toNormalPosition.TweenProperty(this, "rotation_degrees", new Vector3(0.0f, 0.0f, 0.0f), 0.5);
					Tween toNormalRotation = CreateTween().SetTrans(Tween.TransitionType.Quad);
					toNormalRotation.TweenProperty(this, "position", normalPhonePosition, 0.3);
					break;
			}
		}
	}

	public bool Flash
	{
		get => flash;
		set
		{
			flash = value;
			Light.Visible = value;
			Energy.WasteMultiplier = value ? 2.0f : 1.0f;
			FlashIcon.Material = value ? onMaterial : offMaterial;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		normalPhonePosition = Position;

		// Store the original material and load the alternate material.
		offMaterial = FlashIcon.Material;

		NightTimeSystem nightTimeSystem = GetTree().CurrentScene.GetNode<NightTimeSystem>("NightTimeSystem");
		nightTimeSystem.HourChanged += OnHourChanged;
	}

	private void OnHourChanged(int hour)
	{
		TimeLabel.Text = $"{hour:D2}:00";
	}
}
