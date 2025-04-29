using Godot;
using System;
using System.Linq;

public partial class TapeRecorder : StaticBody3D, Interactable
{
	[Export]
	private TextureProgressBar progressBar;

	[Export]
	private AudioStreamPlayer3D recordSound;

	[Export]
	private double recordIncreaseRate = 1.75;

	[Export]
	private ProgressBar timeUntilFailProgressBar;

	private StyleBoxFlat failStyleBox;

	private Player player;

	private bool canRecord = true;
	private bool recorded = false;
	public bool Recorded {
		get => recorded;
		set {
			recorded = value;
			if (recorded) 
			{
				recordSound.Stop();
				recordingProgress = 0.0;
			}

			timeUntilFailProgressBar.Visible = !recorded;

			nightTimeSystem.CanEndNight = recorded;
		}
	}
	private bool recording = false;
	private double recordingProgress = 0.0;

	private double timeUntilFail = 99.0;
	public double TimeUntilFail {
		get => timeUntilFail;
		set {
			timeUntilFail = value;
		}
	}
	private double timeUntilReset = 0.0;

	private NightTimeSystem nightTimeSystem;

	public bool IsInteractable => player.State == Player.States.Record;

	public Color[] timeUntilFailColors;

	public void StartInteract()
	{
		if (!recorded) 
		{
			recording = true;
			if (!recordSound.Playing)
				recordSound.Play();
		}
	}

	public void StopInteract()
	{
		recording = false;
		recordSound.Stop();
	}

	public override void _Ready()
	{
		base._Ready();
		timeUntilFailColors = [
			Colors.Red,
			Colors.Yellow,
			Colors.White,
		];
		timeUntilFailProgressBar.AddThemeStyleboxOverride("fill", failStyleBox);
		player = GetTree().GetFirstNodeInGroup("player") as Player;
		nightTimeSystem = GetTree().CurrentScene.GetNode<NightTimeSystem>("NightTimeSystem");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Recorded && canRecord)
		{
			timeUntilFail -= delta;
			if (recording)
			{
				recordingProgress += recordIncreaseRate * delta;

				if (recordingProgress >= 100.0)
				{
					Recorded = true;
					timeUntilReset = new Random().Next(70, 100);
				}
			}
			progressBar.Value = recordingProgress;

			timeUntilFailProgressBar.Value = timeUntilFail;

			failStyleBox.BgColor = timeUntilFailColors[
				Mathf.Clamp((int)(timeUntilFailProgressBar.Value / (100.0f / timeUntilFailColors.Length)), 0, timeUntilFailColors.Length - 1)
			];
		}
		else
		{
			timeUntilReset -= delta;

			if (timeUntilReset <= 0)
			{
				Recorded = false;
				TimeUntilFail = 99.0;
			}

			if (TimeUntilFail <= 0)
			{
				canRecord = false;
			}

			progressBar.Value = 100.0f;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			var camera = GetViewport().GetCamera3D();
			var from = camera.ProjectRayOrigin(mouseButton.Position);
			var to = from + camera.ProjectRayNormal(mouseButton.Position) * 1000;

			var spaceState = GetWorld3D().DirectSpaceState;
			var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D
			{
				From = from,
				To = to,
				CollisionMask = 1
			});

			if (result.Count > 0)
			{
				var collider = result["collider"].AsGodotObject();

				if (collider == this)
				{
					if (!IsInteractable)
					{
						if (Input.IsActionJustPressed("toggle_flash"))
						{
							GD.Print("clicked");
							if (player.State != Player.States.Record)
							{
								player.AnimationPlayer.Play("RecordTape");
								player.State = Player.States.Record;
							}
						}
					}
				}
			}
			else if (player.State == Player.States.Record)
			{
				player.AnimationPlayer.PlayBackwards("RecordTape");
				player.State = Player.States.Default;
			}
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
