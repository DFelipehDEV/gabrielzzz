using Godot;

public partial class NightEnd : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A + (1.0f * (float)delta));
	}
}
