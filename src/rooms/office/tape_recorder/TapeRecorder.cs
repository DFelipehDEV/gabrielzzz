using Godot;
using System.Linq;

public partial class TapeRecorder : StaticBody3D, Interactable
{
	[Export]
	private TextureProgressBar progressBar;

	[Export]
	private Area3D recordButton;

	private Player player;

	private bool recorded = false;
	private bool recording = false;
	private double recordingProgress = 0.0;
	private bool recordingFailed = false;

	private int previousHour = 1;
	private int hour = 1;
	private double time = 0.0;

	public bool IsInteractable => player.animationPlayer.AssignedAnimation == "RecordTape";

	private EnemyNPC[] enemies;

	public void StartInteract()
	{
		recording = true;
	}

	public void StopInteract()
	{
		recording = false;
	}

	public override void _Ready()
	{
		base._Ready();
		player = (Player)GetTree().GetFirstNodeInGroup("player");
		enemies = GetTree().GetNodesInGroup("enemy").Select(x  => (EnemyNPC)x).ToArray();
	}
	
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (recordingFailed) return;
		
		previousHour = hour;
		time += (delta / Phone.HOUR_LENGTH);
		hour = ((int)time);

		// Hour change
		if (hour != previousHour) {
			if (!recorded) {
				recordingFailed = true;
				foreach (EnemyNPC enemy in enemies) {
					enemy.TimeToMove -= 5;
					GD.Print(enemy.Name + " now takes " + enemy.TimeToMove + " seconds to move");
				}
				GD.Print("Failed to record at " + hour);
			}
		}
		

		if (!recorded) {
			if (recording) {
				recordingProgress += 5.0f * delta;

				if (recordingProgress >= 100.0) {
					recorded = true;
					recordingProgress = 0.0;
				}
			} else {
				recordingProgress -= 1.5f * delta;
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
				
				if (collider == this) {
					if (player.FocusedInteractable == null)  
					{
						if (Input.IsActionJustPressed("toggle_flash")) {
							if (player.State != Player.States.RECORD) {
								player.animationPlayer.Play("RecordTape");
								player.State = Player.States.RECORD;
							} else {
								player.animationPlayer.PlayBackwards("RecordTape");
								player.State = Player.States.NORMAL;
							}
						}
					}
				}
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
