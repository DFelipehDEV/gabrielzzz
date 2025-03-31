using Godot;

[GlobalClass]
public partial class NightTimeSystem : Node
{
	[Signal]
	public delegate void NightEndedEventHandler();
	
	[Signal]
	public delegate void HourChangedEventHandler(int hour);

	[Export]
	private PackedScene nightEndScene;

	[Export]
	private PackedScene nextNightScene;

	private double time;

	private int hour;
	public int Hour {
		get => hour;
	}

	private bool nightEnded = false;

	public static readonly float HOUR_LENGTH = 70.0f;
	public static readonly uint NIGHT_LENGTH = 6;

	public override void _Process(double delta)
	{
		base._Process(delta);
		time += delta / HOUR_LENGTH;
		int newHour = (int)time;

		// Hour changed
		if (newHour != hour) 
		{
			hour = newHour;
			EmitSignal(SignalName.HourChanged, hour);

			if (hour == 6 && !nightEnded)
			{
				nightEnded = true;
				EmitSignal(SignalName.NightEnded);
				
				Node currentScene = GetTree().CurrentScene;
				var nightEndInstance = nightEndScene.Instantiate();
				if (nightEndInstance is NightEnd nightEnd)
				{
					nightEnd.NextNight = nextNightScene;
				}

				GetTree().Root.AddChild(nightEndInstance);
				GetTree().CurrentScene = nightEndInstance;
				
				currentScene?.QueueFree();
			}
		}

	}

}
