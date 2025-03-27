using Godot;
[GlobalClass]
public partial class NightEnd : Control
{
	public string nextNight = "res://Night2.tscn";
	private float time = 0;
	private Label timeLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timeLabel = GetNode<Label>("Time");
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += (float)delta;
		timeLabel.Visible = (int)time % 2 > 0.5;
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A + (1.0f * (float)delta));
		if (time >= 5.0f)
		{
			FileAccess file = FileAccess.Open("user://nightdata.json", FileAccess.ModeFlags.Write);
			file.StoreString(nextNight);
			file.Close();
			GetTree().ChangeSceneToFile(nextNight);
		}
	}
}
