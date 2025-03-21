using System.Linq;
using Godot;

public partial class Generator : StaticBody3D
{
	[Signal]
	public delegate void GeneratorBrokenEventHandler();

	[Signal]
	public delegate void GeneratorFixedEventHandler();

	[Export]
	private GpuParticles3D brokenParticles;

	[Export]
	private TextureProgressBar progressBar;
	
	[Export]
	private AudioStreamPlayer lightsOut;

	private OmniLight3D[] sceneLights;

	private bool broken = false;
	public bool Broken {
		get {
			return broken;
		}

		set {
			broken = value;
			brokenParticles.Emitting = broken;
			progressBar.Visible = broken;

			if (broken) {
				EmitSignal(SignalName.GeneratorBroken);
				lightsOut.Play();
			}

			foreach (OmniLight3D light in sceneLights) {
				light.Visible = !broken;
			}
		}
	}

	private bool beingFixed = false;
	private double fixProgress = 0.0;

	public override void _Ready()
	{
		base._Ready();

		// Get all visible scene lights named "OmniLight3D"
		// Its not a good practice to get all scene lights, but in this case its ok
		// If a problem ever happens we will need to use groups
		sceneLights = GetTree().Root.GetChild(0)
			.FindChildren("OmniLight3D", nameof(OmniLight3D), true)
			.Cast<OmniLight3D>()
			.Where(light => light.Visible)
			.ToArray();

		Broken = broken;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (broken && beingFixed) {
			fixProgress += 10.0f * delta;

			if (fixProgress >= 100.0) {
				Broken = false;
				fixProgress = 0.0;
				EmitSignal(SignalName.GeneratorFixed);
			}
		}

		progressBar.Value = fixProgress;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			var camera = GetViewport().GetCamera3D();
			var from = camera.ProjectRayOrigin(mouseMotion.Position);
			var to = from + camera.ProjectRayNormal(mouseMotion.Position) * 1000;

			var spaceState = GetWorld3D().DirectSpaceState;
			var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D
			{
				From = from,
				To = to,
				CollisionMask = 1
			});

			
			if (result.Count > 0 && result["collider"].AsGodotObject() == this)
			{
				beingFixed = true;
			}
			else
			{
				beingFixed = false;
			}
		}
	}
}
