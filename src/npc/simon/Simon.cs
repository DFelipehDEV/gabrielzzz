using System;
using Godot;

public partial class Simon : EnemyNPC
{
	[Export] 
	private AnimatedSprite3D sprite;

	[Export]
	private AnimationPlayer animationPlayer;

	[Export]
	private AudioStreamPlayer jumpScareAudio;

	private double timeInOffice = 0.0;

	private bool insideOffice = false;

	[Export]
	private double timeToJumpscare = 4.5;

	[Export]
	private bool awake = false;
	private Generator generator;
	private Transform3D initialTransform;

	public override void _Ready()
	{
		base._Ready();
		generator = (Generator)GetTree().GetFirstNodeInGroup("generator");
		generator.Connect("GeneratorBroken", Callable.From(GeneratorBroken));
		generator.Connect("GeneratorRepaired", Callable.From(GeneratorRepaired));
		animationPlayer.AnimationFinished += JumpScareFinished;
		initialTransform = Transform;
	}

   
	public override void _Process(double delta)
	{
		if (awake)
			base._Process(delta);


		if (insideOffice)
		{
			timeInOffice += delta;
			if (timeInOffice > timeToJumpscare)
			{
				if (animationPlayer.CurrentAnimation != "JumpScare")
				{
					animationPlayer.Play("JumpScare");
					jumpScareAudio.Play();
				}
			}
		}
	}

	public void GeneratorBroken() {
		awake = true;
		animationPlayer.Play("Standing");
		GD.Print(Name + " is now awake");
	}

	public void GeneratorRepaired() {
		awake = false;
		Transform = initialTransform;
		animationPlayer.Play("Sit");
		GD.Print(Name + " is now asleep");
	}

	public override void OnMovedToNewPosition(Node3D position)
	{
		insideOffice = position.IsInGroup("office");
	}

	public void JumpScareFinished(StringName animName)
	{
		if (animName != "JumpScare") return;
		
		OS.DelayMsec(1000);
		GetTree().ChangeSceneToFile("res://game_over/GameOver.tscn");
	}
}
