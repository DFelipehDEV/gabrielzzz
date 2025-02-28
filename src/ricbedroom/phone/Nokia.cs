using Godot;

public partial class Nokia : Node3D
{
	private TextureProgressBar progress;
	private Area3D area;
	private bool delayingAlarm = false;

	public override void _Ready()
	{
		area = GetNode<Area3D>("Area");
		progress = area.GetNode<TextureProgressBar>("Progress/SubViewport/ProgressBar");	
		area.MouseEntered += OnMouseEntered;
		area.MouseExited += OnMouseExited;
	}

	public override void _Process(double delta)
	{
		

		if (delayingAlarm) {
			progress.Value += 30.0 * delta;
		} else {
			progress.Value -= 30.0 * delta;
		}
		progress.Value %= 100;
		GD.Print(progress.Value);
	}

	public void OnMouseEntered() {
		delayingAlarm = true;
		GD.Print(delayingAlarm);
	}

	public void OnMouseExited() {
		delayingAlarm = false;
	}
}
