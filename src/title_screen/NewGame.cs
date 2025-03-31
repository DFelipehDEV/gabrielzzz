using Godot;

public partial class NewGame : Button
{
	[Export]
	private PackedScene nextScene;
	
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
		fadeOut.Initialize(0.3f, nextScene, Colors.Black);
		fadeOut.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		GetTree().CurrentScene.AddChild(fadeOut);
	}
}
