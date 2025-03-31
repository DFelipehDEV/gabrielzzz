using Godot;

public partial class NightStart : Control
{
	[Export]
	private PackedScene nextNight;
	
	private float time;

	private Fade fadeOut;

	[Export]
	private Label dayLabel;

	public override void _Ready()
	{
		base._Ready();
		fadeOut = new Fade();
		fadeOut.Initialize(0.3f, nextNight, Colors.Black);
		fadeOut.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		time += (float)delta;
		if (time >= 4.5f && fadeOut.GetParent() == null)
		{
			FileAccess file = FileAccess.Open("user://nightdata.json", FileAccess.ModeFlags.Write);
			file.StoreString(nextNight.ResourcePath);
			file.Close();
			
			GetTree().CurrentScene.AddChild(fadeOut);
		}
	}
}
