using Godot;
using System;

[GlobalClass]
public partial class EnemyNPC : Node3D
{
	[Export]
	private Godot.Collections.Array<Node3D> positions;
	private Node3D nextPosition;
	[Export]
	private double timeUntilNextPosition = 40.0;

	private Random random = new Random();

	public override void _Process(double delta)
	{
		timeUntilNextPosition -= delta;

		if (timeUntilNextPosition <= 0)
		{
			MoveToNextPosition();
			SelectNextPosition();
		}
	}

	private void SelectNextPosition()
	{
		int randomIndex = random.Next(0, positions.Count);
		nextPosition = positions[randomIndex];

		timeUntilNextPosition = random.NextDouble() * 10;
	}

	private void MoveToNextPosition()
	{
		if (nextPosition != null)
		{
			Transform = nextPosition.GlobalTransform;
		}
	}
}
