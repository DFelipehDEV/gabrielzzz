using Godot;

public partial class Simon : EnemyNPC
{
	private bool awake = false;
	private Generator generator;
    private Transform3D initialTransform;

	public override void _Ready()
	{
		base._Ready();
		generator = (Generator)GetTree().GetFirstNodeInGroup("generator");
		generator.Connect("GeneratorBroken", Callable.From(GeneratorBroken));
		generator.Connect("GeneratorFixed", Callable.From(GeneratorFixed));
        initialTransform = Transform;
	}

	public override void _Process(double delta)
	{
		if (awake)
			base._Process(delta);
	}

	public void GeneratorBroken() {
		awake = true;
		GD.Print(Name + " is now awake");
	}

	public void GeneratorFixed() {
		awake = false;
        Transform = initialTransform;
		GD.Print(Name + " is now asleep");
	}
}
