using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class MovementComponent : Node
{
	[Export]
	public float JumpImpulse { get; set; } = 300f;

	[Signal]
	public delegate void JumpedEventHandler(Vector2 impulse);
	
	[Export]
	public float GravityMultiplier { get; set; } = 10f;

	[Signal]
	public delegate void ForceGravityEventHandler(float multiplier);


	[Export]
	public float SpeedMultiplier { get; set; } = 1.4f;
	private Vector2 appliedSpeedUp;



	private List<Node> groundCollisions = new();
	private RigidBody2D rb;


    public override void _Ready()
    {
        rb = GetParent<RigidBody2D>();
    }


    public override void _PhysicsProcess(double delta)
    {
        checkJump();
		checkForceGravity();
    }


    public void _OnBodyEntered(Node node)
	{
		if (node.IsInGroup("Snow"))
			groundCollisions.Add(node);
	}

	public void _OnBodyExited(Node node)
	{
		groundCollisions.Remove(node);
	}

	private void checkJump()
	{
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			EmitSignal(SignalName.Jumped, new Vector2(0f, -JumpImpulse));
		}
	}

	private void speedForward()
	{
		if (!Input.IsActionPressed("force_gravity"))
			return;

		appliedSpeedUp = rb.LinearVelocity * SpeedMultiplier;
		rb.ApplyCentralForce(appliedSpeedUp);

	}

	private void setGravity()
	{
		float gravity_value = 1f;
		if (Input.IsActionPressed("force_gravity"))
			gravity_value = GravityMultiplier;

		EmitSignal(SignalName.ForceGravity, gravity_value);
	}

	private void checkForceGravity()
	{
		if(IsOnFloor())
		{
			speedForward();
		}
		else
		{
			setGravity();
		}
	}

	private bool IsOnFloor(){
		return groundCollisions.Count != 0;
	}
}
