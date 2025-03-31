using Godot;

public partial class Vitel : EnemyNPC
{
	[Export]
	private AnimatedSprite2D jumpScare;

	[Export]
	private AudioStreamPlayer jumpScareAudio;

	private double timeInOffice = 0.0;
	private bool insideOffice = false;

	[Export]
	private double timeToJumpscare = 8.5;

	private Player player;

	public override void _Ready()
	{
		base._Ready();
		player = (Player)GetTree().GetFirstNodeInGroup("player");

		jumpScare.AnimationFinished += JumpScareFinished;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (insideOffice && player.State != Player.States.Hidden)
		{
			timeInOffice += delta;
			if (timeInOffice > timeToJumpscare)
			{
				if (!jumpScare.Visible)
				{
					jumpScare.Visible = true;
					jumpScare.Play("JumpScare");
					jumpScareAudio.Play();
				}
			}
		}
	}

	public void JumpScareFinished()
	{
		OS.DelayMsec(1000);
		GetTree().ChangeSceneToFile("res://game_over/GameOver.tscn");
	}

	public override void OnMovedToNewPosition(Node3D position)
	{
		if (position.IsInGroup("office"))
		{
			insideOffice = true;
		}
	}
}
