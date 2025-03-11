using Godot;

public partial class Nokia : Node3D
{
	private TextureProgressBar progress;
	private bool delayingAlarm = false;

	public override void _Ready()
	{
		progress = GetNode<TextureProgressBar>("Progress/SubViewport/ProgressBar");	
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
