using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
	public float Torque { get; set; } = 10f;

	[Signal]
	public delegate void ApplyTorqueEventHandler(float torque);

	[Export]
	public float SpeedMultiplier { get; set; } = 1.4f;
	private Vector2 appliedSpeedUp;

	private List<Node> groundCollisions = new();
	private ScalingComponent scalingComponent;

	private Vector2 ScaledJumpVector => new(0f, -JumpImpulse * (float)Math.Sqrt(scalingComponent.Size));
	private float ScaledTorque => Torque * scalingComponent.Size;
	private bool IsOnFloor => groundCollisions.Count != 0;
	private bool isGravityMultiplierOn = false;

	public override void _Ready()
	{
		scalingComponent = (ScalingComponent)GetParent().GetChildren().First(node => node is ScalingComponent);
	}


	public override void _PhysicsProcess(double delta)
	{
		checkJump();
		checkForceGravity();
		checkRotation();
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
		if (IsOnFloor && Input.IsActionJustPressed("jump"))
		{
			EmitSignal(SignalName.Jumped, ScaledJumpVector);
		}
	}

	private void checkRotation()
	{
		if (Input.IsActionPressed("right"))
			EmitSignal(SignalName.ApplyTorque, ScaledTorque);
		if (Input.IsActionPressed("left"))
			EmitSignal(SignalName.ApplyTorque, -ScaledTorque);
	}

	private void checkForceGravity()
	{
		if (!isGravityMultiplierOn && (!IsOnFloor && Input.IsActionJustPressed("force_gravity")))
		{
			isGravityMultiplierOn = true;
			EmitSignal(SignalName.ForceGravity, GravityMultiplier);
		}
		if (isGravityMultiplierOn && (IsOnFloor || Input.IsActionJustReleased("force_gravity")))
		{
			isGravityMultiplierOn = false;
			EmitSignal(SignalName.ForceGravity, 1f);
		}
	}
}
