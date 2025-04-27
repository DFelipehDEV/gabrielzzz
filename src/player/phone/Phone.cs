using Godot;

public partial class Phone : Node3D
{
	[Export]
	public Node3D Holder { get; private set; }

	[Export]
	public Light3D Light { get; private set; }

	[Export]
	public CanvasLayer CameraUI { get; private set; }

	[Export]
	public Label DateLabel { get; private set; }

	[Export]
	public Label TimeLabel { get; private set; }

	[Export]
	public Label EnergyLabel { get; private set; }

	[Export]
	public TextureRect FlashIcon { get; private set; }

	[Export]
	private Node3D eye;

	[Export]
	public MeshInstance3D Screen { get; private set; }

	private Vector3 normalPhonePosition;
	private Texture2D flashOffTexture;

	[Export]
	private Texture2D flashOnTexture;

	private bool on = true;
	public bool On {
		get => on;
		set {
			on = value;
			if (!on) 
			{
				Screen.Visible = false;
				Light.Visible = false;
			}
		}
	}

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

	private Energy energy;

	public bool Flash
	{
		get => Light.Visible;
		set
		{
			Light.Visible = value;
			energy.WasteMultiplier = value ? 5.0f : 1.0f;
			FlashIcon.Texture = value ? flashOnTexture : flashOffTexture;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		energy = new Energy();
		normalPhonePosition = Position;

		// Store the original Texture and load the alternate Texture.
		flashOffTexture = FlashIcon.Texture;

		NightTimeSystem nightTimeSystem = GetTree().CurrentScene.GetNode<NightTimeSystem>("NightTimeSystem");
		nightTimeSystem.HourChanged += OnHourChanged;

		DateLabel.Text = (GetTree().CurrentScene as Night).Day;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		energy.Update(delta);
		EnergyLabel.Text = energy.ToString();
		
		if (energy.CurrentEnergy == 0)
		{
			On = false;
		}
	}


	private void OnHourChanged(int hour)
	{
		TimeLabel.Text = $"{hour:D2}:00";
	}
}
