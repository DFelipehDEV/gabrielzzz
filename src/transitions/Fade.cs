using Godot;

[GlobalClass]
public partial class Fade : ColorRect
{
	[Export]
	public float speed = 0.15f;
	
	[Export]
	public PackedScene nextScene;

	// why doesnt godot like constructors :(
	public void Initialize(float speed, PackedScene nextScene, Color color) {
		this.speed = speed;
		this.nextScene = nextScene;
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
		modulate.A += speed * (float)delta;
		Modulate = modulate;

		// Is fade complete
		if (modulate.A > 1.0f || modulate.A < 0.0f)
		{
			if (nextScene == null)
			{
				QueueFree();
			}
			else
			{
				GetTree().ChangeSceneToPacked(nextScene);
			}
		}
	}
}
