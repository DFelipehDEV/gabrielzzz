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

	private double defaultTimeToMove;
	private double timeToMoveInOffice;

	private Player player;

	public override void _Ready()
	{
		base._Ready();
		player = GetTree().GetFirstNodeInGroup("player") as Player;

		jumpScare.AnimationFinished += JumpScareFinished;

		defaultTimeToMove = TimeToMove;
		timeToMoveInOffice = Mathf.Max(timeToJumpscare + 1.0, defaultTimeToMove * 0.5);
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
		else
		{
			timeInOffice = 0;
		}
	}

	public void JumpScareFinished()
	{
		OS.DelayMsec(1000);
		GetTree().ChangeSceneToFile("res://game_over/GameOver.tscn");
	}

	public override void OnMovedToNewPosition(Node3D position)
	{
		insideOffice = position.IsInGroup("office");
		TimeToMove = insideOffice ? timeToMoveInOffice : defaultTimeToMove;

		// Reset time in office again just to make sure
		if (!insideOffice)
    	{
        	timeInOffice = 0.0;
    	}
		else
		{
			GD.Print(Name + " is in office");
		}
	}
}
