using System.ComponentModel;
using Godot;

public partial class NightStart : Control
{	
	[Export]
	private string date;

	private float time;

	private Fade fadeOut;

	[Export]
	private Label dayLabel;

	public override void _Ready()
	{
		base._Ready();
		fadeOut = new Fade();
		fadeOut.Initialize(0.3f, null, Colors.Black);
		fadeOut.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		fadeOut.Faded += OnFadeOutFaded;

		dayLabel.Text = date;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		time += (float)delta;
		if (time >= 4.5f && fadeOut.GetParent() == null)
		{
			GetTree().CurrentScene.AddChild(fadeOut);
		}
	}

	private void OnFadeOutFaded()
	{
		Fade fadeIn = new Fade();
		fadeIn.Initialize(-0.3f, null, Colors.Black);
		fadeIn.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		GetParent().AddChild(fadeIn);
		
		QueueFree();
	}
}
