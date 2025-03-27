using Godot;
using System.Linq;

public partial class TapeRecorder : StaticBody3D
{
	[Export]
	private TextureProgressBar progressBar;

	[Export]
	private Area3D recordButton;

    private Player player;

	private bool recorded = false;
	private bool recording = false;
	private double recordingProgress = 0.0;

    public override void _Ready()
    {
        base._Ready();
        player = (Player)GetTree().GetFirstNodeInGroup("player");
    }
    
    public override void _Process(double delta)
	{
		base._Process(delta);

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

				recording = collider == recordButton;
				GD.Print(recording);
			}
			
		}
	}
}
