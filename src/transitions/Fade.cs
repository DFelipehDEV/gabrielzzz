using Godot;

[GlobalClass]
public partial class Fade : ColorRect
{
	[Export]
	public float Speed = 0.15f;
	
	[Export]
	public string NextScene = "";

	// why doesnt godot like constructors :(
	public void Initialize(float speed, string nextScene, Color color) {
		Speed = speed;
		NextScene = nextScene;
		Color = color;
	}

	public override void _Ready()
	{
		base._Ready();
		SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
		//SetAnchorsPreset(LayoutPreset.FullRect);
	}

	public override void _Process(double delta)
	{
		Color modulate = Modulate;
		modulate.A += Speed * (float)delta;
		Modulate = modulate;
		GD.Print(Modulate.A);

		// Is fade complete
		if (modulate.A > 1.0f || modulate.A < 0.0f)
		{
			if (NextScene == "")
			{
				QueueFree();
			}
			else
			{
				GetTree().ChangeSceneToFile(NextScene);
			}
		}
	}
}
