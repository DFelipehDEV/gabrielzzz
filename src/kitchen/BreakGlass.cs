using Godot;
using System;

public partial class BreakGlass : AudioStreamPlayer3D
{
	private RandomNumberGenerator rng;

	private float elapsedTime = 0f;

	private float currentInterval = 0f;

	[Export]
	private float MinInterval = 100f;
	
	[Export]
	private float MaxInterval = 220f;

	public override void _Ready()
	{
		rng = new RandomNumberGenerator();
		rng.Randomize();

		currentInterval = GetRandomInterval();
	}

	public override void _Process(double delta)
	{
		elapsedTime += (float)delta;

		// Check if the elapsed time exceeds the current interval
		if (elapsedTime >= currentInterval)
		{
			if (!Playing)
			{
				Play();
			}

			elapsedTime = 0f;
			currentInterval = GetRandomInterval();
		}
	}

	private float GetRandomInterval()
	{
		return rng.RandfRange(MinInterval, MaxInterval);
	}
}
