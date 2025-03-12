using Godot;

public partial class Nokia : StaticBody3D
{
    [Export]
    public double AlarmIncreaseRate = 9.5;

    [Export]
    public double AlarmDecreaseRate = 3.5;

    private TextureProgressBar progress;
    private bool delayingAlarm = false;
    private double alarmTimer = 100.0;

    public override void _Ready()
    {
        progress = GetNode<TextureProgressBar>("Alarm/SubViewport/ProgressBar");
    }

    public override void _Process(double delta)
    {
        if (delayingAlarm)
        {
            alarmTimer += AlarmIncreaseRate * delta;
        }
        else
        {
            alarmTimer -= AlarmDecreaseRate * delta;
        }
        alarmTimer = Mathf.Clamp(alarmTimer, 0, 100);

        progress.Value = alarmTimer;
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
				delayingAlarm = true;
			}
			else
			{
				delayingAlarm = false;
			}
		}
	}
}