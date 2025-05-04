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

	private Player player;
	private Generator generator;

	public override void _Ready()
	{
		base._Ready();
		player = GetTree().GetFirstNodeInGroup("player") as Player;
		generator = GetTree().GetFirstNodeInGroup("generator") as Generator;
		generator.Connect("GeneratorBroken", Callable.From(GeneratorBroken));
		generator.Connect("GeneratorRepaired", Callable.From(GeneratorRepaired));
		animationPlayer.AnimationFinished += JumpScareFinished;
	}


	public override void _Process(double delta)
	{
		if (awake && animationPlayer.AssignedAnimation != "JumpScare" && animationPlayer.AssignedAnimation != "JumpScareTable")
			base._Process(delta);

		if (insideOffice)
		{
			timeInOffice += delta;
			if (timeInOffice > timeToJumpscare)
			{
				string jumpScareAnimationName = player.State == Player.States.Hidden ? "JumpScareTable" : "JumpScare";
				if (animationPlayer.CurrentAnimation != jumpScareAnimationName)
				{
					animationPlayer.Play(jumpScareAnimationName);
				}
			}
		}
		else
		{
			timeInOffice = 0;
		}
	}

	public void GeneratorBroken()
	{
		awake = true;
		animationPlayer.Play("Standing");
		GD.Print(Name + " is now awake");
	}

	public void GeneratorRepaired()
	{
		awake = false;
		MoveToPosition(1);
		animationPlayer.Play("RESET");
		GD.Print(Name + " is now asleep");
	}

	public override void OnMovedToNewPosition(Node3D position)
	{
		insideOffice = position.IsInGroup("office");
	}

	public void JumpScareFinished(StringName animName)
	{
		if (animName != "JumpScare" && animName != "JumpScareTable") return;

		OS.DelayMsec(1000);
		GetTree().ChangeSceneToFile("res://game_over/GameOver.tscn");
	}
}
