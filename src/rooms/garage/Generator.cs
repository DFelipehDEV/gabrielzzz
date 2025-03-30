using System.Linq;
using Godot;

public partial class Generator : StaticBody3D, Interactable
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
	public bool Broken
	{
		get
		{
			return broken;
		}

		set
		{
			broken = value;
			brokenParticles.Emitting = broken;
			progressBar.Visible = broken;

			if (broken)
			{
				EmitSignal(SignalName.GeneratorBroken);
				lightsOut.Play();
			}

			foreach (OmniLight3D light in sceneLights)
			{
				light.Visible = !broken;
			}
		}
	}

	private bool beingFixed = false;
	public bool BeingFixed
	{
		get => beingFixed;
		set => beingFixed = value;
	}
	private double fixProgress = 0.0;

	private bool isMouseOverGenerator = false;
	private bool isMousePressed = false;

	public bool IsInteractable => Broken;

	public void StartInteract()
	{
		beingFixed = true;
	}

	public void StopInteract()
	{
		beingFixed = false;
	}

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

		if (broken)
		{
			if (beingFixed)
			{
				fixProgress += 10.0 * delta;

				if (fixProgress >= 100.0)
				{
					Broken = false;
					fixProgress = 0.0;
					EmitSignal(SignalName.GeneratorFixed);
				}
			}
			progressBar.Value = fixProgress;
		}
	}

	public void Highlight()
	{
		foreach (MeshInstance3D mesh in GetMeshes())
		{
			var material = mesh.GetActiveMaterial(0);
			if (material != null)
			{
				var newMat = material.Duplicate() as StandardMaterial3D;
				newMat.AlbedoColor = new Color(4.0f, 4.0f, 4.0f);
				mesh.MaterialOverride = newMat;
			}
		}
	}

	public void Unhighlight()
	{
		foreach (MeshInstance3D mesh in GetMeshes())
		{
			mesh.MaterialOverride = null;
		}
	}

	private MeshInstance3D[] GetMeshes()
	{
		return FindChildren("*", "MeshInstance3D", true)
				.Cast<MeshInstance3D>()
				.ToArray();
	}
}
