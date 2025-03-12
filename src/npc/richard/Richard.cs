using Godot;

public partial class Richard : EnemyNPC
{
	private bool awake = false;

	public override void _Ready()
	{
		base._Ready();
		GetTree().GetFirstNodeInGroup("nokia").Connect("AlarmFired", Callable.From(AlarmFired));
		
	}
	public override void _Process(double delta)
	{
		if (awake)
			base._Process(delta);
	}

	public void AlarmFired() {
		awake = true;
		GD.Print(Name + " is now awake");
	}
}
