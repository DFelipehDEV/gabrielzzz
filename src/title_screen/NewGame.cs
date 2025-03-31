using Godot;

public partial class NewGame : Button
{
	public override void _Ready()
	{
		Pressed += NewNight;
	}

	public void NewNight()
	{
		FileAccess file = FileAccess.Open("user://nightdata.json", FileAccess.ModeFlags.Write);
		file.StoreString("res://Night1.tscn");
		file.Close();
		Fade fadeOut = new Fade();
		fadeOut.Initialize(0.3f, "res://night_start/NightStart.tscn", Colors.Black);
		fadeOut.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		GetTree().Root.GetChildren()[0].AddChild(fadeOut);
	}
}
