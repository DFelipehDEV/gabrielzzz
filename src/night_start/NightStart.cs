using Godot;

public partial class NightStart : Control
{
	public string nextNight = "res://Night1.tscn";
	private float time;

	[Export]
	private Label dayLabel;

	public override void _Ready()
	{
		base._Ready();
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		time += (float)delta;
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
