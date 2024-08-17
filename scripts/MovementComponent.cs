using Godot;
using System;
using System.Collections.Generic;

public partial class MovementComponent : Node
{
	[Export]
	public float JumpImpulse { get; set; } = 100f;

	[Signal]
	public delegate void JumpedEventHandler(Vector2 impulse);
	
	[Export]
	public float GravityMultiplier { get; set; } = 10f;

	[Signal]
	public delegate void ForceGravityEventHandler(float multiplier);

	private bool forcing_gravity = false;


	private List<Node> groundCollisions = new();


    public override void _Process(double delta)
	{
		checkJump();
		checkForceGravity();
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

	private void checkForceGravity()
	{
		float gravity_value = 1f;
		if (Input.IsActionPressed("force_gravity"))
			gravity_value = GravityMultiplier;

		EmitSignal(SignalName.ForceGravity, gravity_value);
	}
}
