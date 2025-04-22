using Godot;
using System.Linq;

public partial class TapeRecorder : StaticBody3D, Interactable
{
	[Export]
	private TextureProgressBar progressBar;

	[Export]
	private AudioStreamPlayer3D recordSound;

	private Player player;

	private bool recorded = false;
	private bool recording = false;
	private double recordingProgress = 0.0;
	private bool recordingFailed = false;

	private NightTimeSystem nightTimeSystem;

	public bool IsInteractable => player.State == Player.States.Record;

	private EnemyNPC[] enemies;

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
		player = (Player)GetTree().GetFirstNodeInGroup("player");
		enemies = GetTree().GetNodesInGroup("enemy").Select(x => (EnemyNPC)x).ToArray();
		nightTimeSystem = GetTree().CurrentScene.GetNode<NightTimeSystem>("NightTimeSystem");
		nightTimeSystem.HourChanged += OnHourChanged;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (recordingFailed) return;

		if (!recorded)
		{
			if (recording)
			{
				recordingProgress += 5.0f * delta;

				if (recordingProgress >= 100.0)
				{
					recorded = true;
					recordSound.Stop();
					recordingProgress = 0.0;
				}
			}
			else
			{
				recordingProgress = Mathf.Max(recordingProgress - 1.5f * delta, 0);
			}
		}

		progressBar.Value = recordingProgress;
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
	
	private void OnHourChanged(int hour)
	{
		if (!recorded && !recordingFailed)
		{
			recordingFailed = true;
			foreach (EnemyNPC enemy in enemies)
			{
				enemy.TimeToMove -= 5;
				GD.Print(enemy.Name + " now takes " + enemy.TimeToMove + " seconds to move");
			}
			GD.Print("Failed to record at " + hour);
		}
		else
		{
			recorded = false;
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
