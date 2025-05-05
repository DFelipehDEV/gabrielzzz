using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class EnemyNPC : Node3D
{
	[Export]
	private string positionGroup;

	private Godot.Collections.Array<Node3D> positions;
	private int nextPositionIndex;
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
									.OfType<Node3D>()
									.ToArray();
			positions = new Godot.Collections.Array<Node3D>(positionsGroup);
		}
		else
		{
			GD.PrintErr("Missing field 'positionGroup'");
		}

		SelectNextPosition();
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
		} while (positions.Count > 1 && randomIndex == currentPositionIndex);

		nextPositionIndex = randomIndex;
		timeUntilNextMove = timeToMove * (0.8 + 0.4 * random.NextDouble());
	}

	private void MoveToNextPosition()
	{
		if (nextPositionIndex >= 0)
		{
			MoveToPosition(nextPositionIndex);
		}
	}

	public void MoveToPosition(int index)
	{
		if (index < 0)
			return;

		currentPositionIndex = index;
		Transform = positions[index].GlobalTransform;
		OnMovedToNewPosition(positions[index]);
	}

	public virtual void OnMovedToNewPosition(Node3D position) { }
}
