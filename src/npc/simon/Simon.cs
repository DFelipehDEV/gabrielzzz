using Godot;

public partial class Simon : EnemyNPC
{
	[Export] 
	private AnimatedSprite3D sprite;

	private bool awake = false;
	private Generator generator;
	private Transform3D initialTransform;

	public override void _Ready()
	{
		base._Ready();
		generator = (Generator)GetTree().GetFirstNodeInGroup("generator");
		generator.Connect("GeneratorBroken", Callable.From(GeneratorBroken));
		generator.Connect("GeneratorRepaired", Callable.From(GeneratorRepaired));
		initialTransform = Transform;
	}

	public override void _Process(double delta)
	{
		if (awake)
			base._Process(delta);
	}

	public void GeneratorBroken() {
		awake = true;
		sprite.Animation = "standing";
		GD.Print(Name + " is now awake");
	}

	public void GeneratorRepaired() {
		awake = false;
		Transform = initialTransform;
		sprite.Animation = "sit";
		GD.Print(Name + " is now asleep");
	}
}
