using Godot;
using System;
using System.Collections.Generic;

public partial class MovementComponent : Node
{
	[Export]
	public float JumpImpulse { get; set; } = 100f;

	[Signal]
	public delegate void JumpedEventHandler(Vector2 impulse);

	private List<Node> groundCollisions = new();

	public override void _Process(double delta)
	{
		checkJump();
	}

	public void _OnBodyEntered(Node node)
	{
		if (node.IsInGroup("Ground"))
			groundCollisions.Add(node);
	}

	public void _OnBodyExited(Node node)
	{
		groundCollisions.Remove(node);
	}

	private void checkJump()
	{
		if (Input.IsActionPressed("jump") && groundCollisions.Count != 0)
		{
			EmitSignal(SignalName.Jumped, new Vector2(0f, -JumpImpulse));
		}
	}
}
