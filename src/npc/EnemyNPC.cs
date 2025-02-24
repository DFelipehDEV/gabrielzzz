using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class EnemyNPC : Node3D
{
	[Export]
	private string positionGroup;
	private Godot.Collections.Array<Node3D> positions;
	private Node3D nextPosition;
	private int currentPositionIndex = 0;
	[Export]
	private double timeUntilNextPosition = 40.0;
	
	[Export]
	private AudioStreamPlayer3D walkSound;

	private Random random = new Random();

	public override void _Ready()
	{
		base._Ready();
		if (positionGroup != "") {
			var positionsGroup = GetTree().GetNodesInGroup(positionGroup).Select(x => (Node3D)x).ToArray();
			positions = new Godot.Collections.Array<Node3D>(positionsGroup);
		} else {
			GD.PrintErr("Missing field 'positionGroup'");
		}
	}

	public override void _Process(double delta)
	{
		timeUntilNextPosition -= delta;

		if (timeUntilNextPosition <= 0)
		{
			MoveToNextPosition();
			walkSound.Play();
			SelectNextPosition();
		}
	}

	private void SelectNextPosition()
	{
		int randomIndex = random.Next(0, positions.Count);
		while (randomIndex == currentPositionIndex) {
			randomIndex = random.Next(0, positions.Count);
		}
		nextPosition = positions[randomIndex];
		currentPositionIndex = randomIndex;

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
