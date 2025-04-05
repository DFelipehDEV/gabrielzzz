using Godot;

[GlobalClass]
public partial class RoomCamera : Camera3D
{
	private AudioListener3D listener;
	public AudioListener3D Listener {
		get => listener;
	}

	public bool Deactivated {
		get {
			return deactivatedTime > 0;
		}
	}
	private double deactivatedTime;

	public override void _Ready()
	{
		listener = new AudioListener3D();
		AddChild(listener);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		deactivatedTime = Mathf.Max(deactivatedTime - delta, 0.0);

		if (Deactivated) {
			Environment.TonemapExposure = 0;
		}
	}

	public void Deactivate(double time) {
		deactivatedTime = time;
		GD.Print("Camera deactivated for " + time.ToString() + " seconds");
	}
}
