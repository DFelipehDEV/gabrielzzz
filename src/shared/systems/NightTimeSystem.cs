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

	public static readonly float HOUR_LENGTH = 1.2f * 60.0f; // Takes 72 seconds to go from 00:00 to 01:00
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
				NightEnd nightEndNode = nightEndScene.Instantiate() as NightEnd;
				if (nightEndNode != null) 
				{
					nightEndNode.NextNight = nextNightScene;
					GetTree().Root.AddChild(nightEndNode);
				}
			}
		}

	}

}
