using Godot;

public partial class Richard : EnemyNPC
{
	private bool awake = false;

	[Export]
	private AnimatedSprite2D jumpScare;
	[Export]
	private AudioStreamPlayer jumpScareAudio;
	
	private double timeInOffice = 0.0;

	private bool insideOffice = false;


	public override void _Ready()
	{
		base._Ready();
		GetTree().GetFirstNodeInGroup("nokia").Connect("AlarmFired", Callable.From(AlarmFired));
		jumpScare.AnimationFinished += JumpScareFinished;

	}
	public override void _Process(double delta)
	{
		if (awake)
			base._Process(delta);

		if (insideOffice) {
			timeInOffice += 1.0 * delta;
			if (timeInOffice > 5.00) {
				if (!jumpScare.Visible) {
					jumpScare.Visible = true;
					jumpScare.Play("JumpScare");
					jumpScareAudio.Play();
				}
			}
		}
	}

	public void AlarmFired() {
		awake = true;
		GD.Print(Name + " is now awake");
	}

	public override void MovedToNewPosition(Node3D position) {
		if (position.IsInGroup("office")) {
			insideOffice = true;
		}
	}

	public void JumpScareFinished() {
		OS.DelayMsec(1000);
		GetTree().ChangeSceneToFile("res://game_over/GameOver.tscn");
	}



}
