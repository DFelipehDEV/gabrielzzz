using Godot;
[GlobalClass]
public partial class NightEnd : Control
{
	[Export]
	private PackedScene nextNight;

	private float time = 0;
	private Label timeLabel;

	private Fade fadeOut;
	
	public override void _Ready()
	{
		timeLabel = GetNode<Label>("Time");

		fadeOut = new Fade();
		fadeOut.Initialize(0.3f, nextNight, Colors.Black);
		fadeOut.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	}

	public override void _Process(double delta)
	{
		time += (float)delta;
		timeLabel.Visible = (int)time % 2 > 0.5;
		if (time >= 4.5f && fadeOut.GetParent() == null)
		{
			FileAccess file = FileAccess.Open("user://nightdata.json", FileAccess.ModeFlags.Write);
			file.StoreString(nextNight.ResourcePath);
			file.Close();
			GetTree().Root.GetChildren()[0].AddChild(fadeOut);
		}
	}
}
