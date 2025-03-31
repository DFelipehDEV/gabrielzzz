using System;
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
	private double time = 0;
	private Vector3 normalPhonePosition;
	private bool nightEnded = false;
	private Material offMaterial, onMaterial;


	public enum Animations
	{
		DEFAULT,
		OPEN_CAMERA,
		CLOSE_CAMERA,
	}

	private Animations animation;

	public static readonly float HOUR_LENGTH = 1.2f * 60.0f; // Takes 72 seconds to go from 00:00 to 01:00
	public static readonly uint NIGHT_LENGTH = 6;

	public Animations Animation
	{
		get => animation;
		set
		{
			animation = value;

			switch (animation)
			{
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
		onMaterial = (Material)GD.Load("res://player/phone/ui/FlashlightOnIcon.tres");
	}

	public override void _Process(double delta)
	{
		time += (delta / HOUR_LENGTH);
		int hour = ((int)time);
		TimeLabel.Text = $"{hour:D2}:00";
		if (hour == 6 && !nightEnded)
		{
			nightEnded = true;
			var nightendScene = ResourceLoader.Load<PackedScene>("res://night_end/NightEnd.tscn").Instantiate();
			GetTree().Root.AddChild(nightendScene);
		}
	}
}
