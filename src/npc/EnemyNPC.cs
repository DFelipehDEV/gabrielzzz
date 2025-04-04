using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class EnemyNPC : Node3D
{
	[Export]
	private string positionGroup;

	private Godot.Collections.Array<Node3D> positions;
	private Node3D currentPosition;
	private Node3D nextPosition;
	private int currentPositionIndex = 0;

	[Export]
	private double timeToMove = 40.0;
	public double TimeToMove
	{
		get => timeToMove;
		set => timeToMove = value;
	}

	private double timeUntilNextMove = 40.0;

	[Export]
	private AudioStreamPlayer3D walkSound;

	private Random random = new Random();

	public override void _Ready()
	{
		base._Ready();
		timeUntilNextMove = timeToMove;
		if (!string.IsNullOrEmpty(positionGroup))
		{
			var positionsGroup = GetTree().GetNodesInGroup(positionGroup)
									.Select(x => (Node3D)x)
									.ToArray();
			positions = new Godot.Collections.Array<Node3D>(positionsGroup);
		}
		else
		{
			GD.PrintErr("Missing field 'positionGroup'");
		}
	}

	public override void _Process(double delta)
	{
		timeUntilNextMove -= delta;

		if (timeUntilNextMove <= 0)
		{
			MoveToNextPosition();
			if (walkSound != null)
				walkSound.Play();
			SelectNextPosition();
		}
	}

	private void SelectNextPosition()
	{
		int randomIndex;
        do
        {
            randomIndex = random.Next(0, positions.Count);
        } 
        while (positions.Count > 1 && randomIndex == currentPositionIndex);

		nextPosition = positions[randomIndex];
		currentPositionIndex = randomIndex;
		timeUntilNextMove = timeToMove * (0.8 + 0.4 * random.NextDouble());
	}

	private void MoveToNextPosition()
	{
		if (nextPosition != null)
		{
			currentPosition = nextPosition;
			Transform = currentPosition.GlobalTransform;
			OnMovedToNewPosition(currentPosition);
		}
	}

	public virtual void OnMovedToNewPosition(Node3D position) { }
}
