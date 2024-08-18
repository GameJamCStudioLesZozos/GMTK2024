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
	public PackedScene SplitInstance;

	[Export]
	public float GravityMultiplier { get; set; } = 10f;

	[Signal]
	public delegate void ForceGravityEventHandler(float multiplier);

	[Export]
	public float Torque { get; set; } = 10f;

	[Signal]
	public delegate void ApplyTorqueEventHandler(float torque);

	[Export]
	public float DashRotation { get; set; } = 30f;

	[Signal]
	public delegate void ApplyDashRotationEventHandler(float dashRotation);

	[Export]
	public float AerialDashHorizontalSpeed { get; set; } = 30f;

	[Export]
	public float AerialDashVerticalSpeed { get; set; } = 10f;

	[Signal]
	public delegate void ApplyAerialDashImpulseEventHandler(Vector2 aerialDashImpulse);

	[Export]
	public float SpeedMultiplier { get; set; } = 1.4f;
	private Vector2 appliedSpeedUp;

	private List<Node> groundCollisions = new();
	private ScalingComponent scalingComponent;

	private Vector2 ScaledJumpVector => new(0f, -JumpImpulse * (float)Math.Sqrt(scalingComponent.Size));
	private float ScaledTorque => Torque * scalingComponent.Size;
	private float ScaledDashRotation => DashRotation * scalingComponent.Size * scalingComponent.Size;
	private float ScaledAerialDashHorizontalSpeed => AerialDashHorizontalSpeed * (float)Math.Sqrt(scalingComponent.Size);
	private float ScaledAerialDashVerticalSpeed => -AerialDashVerticalSpeed * (float)Math.Sqrt(scalingComponent.Size);
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
		checkDash();
		checkSplit();
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
		if (IsOnFloor && Input.IsActionJustPressed("jump"))
		{
			EmitSignal(SignalName.Jumped, ScaledJumpVector);
		}
	}
	
	private void checkSplit()
	{
		if (Input.IsActionJustPressed("split"))
		{
			scalingComponent.ScaleSize(-scalingComponent.Size/2);
			var scene = GD.Load<PackedScene>(SplitInstance.ResourcePath);
			var instance = scene.Instantiate<Node2D>();
			instance.Position = GetParent<Node2D>().Position;
			var newScalingComponent = instance.GetNode<ScalingComponent>("ScalingComponent");
			newScalingComponent.ScaleSize(scalingComponent.Size - newScalingComponent.Size);
			AddChild(instance);
		}
	}

	private void checkRotation()
	{
		if (Input.IsActionPressed("right"))
			EmitSignal(SignalName.ApplyTorque, ScaledTorque);
		if (Input.IsActionPressed("left"))
			EmitSignal(SignalName.ApplyTorque, -ScaledTorque);
	}

	private void checkDash()
	{
		if (!Input.IsActionJustPressed("dash"))
			return;

		if (IsOnFloor)
		{
			if (Input.IsActionPressed("left"))
				EmitSignal(SignalName.ApplyDashRotation, -ScaledDashRotation);
			else
				EmitSignal(SignalName.ApplyDashRotation, ScaledDashRotation);
		}
		else
		{
			if (Input.IsActionPressed("left"))
				EmitSignal(SignalName.ApplyAerialDashImpulse, new Vector2(-ScaledAerialDashHorizontalSpeed, ScaledAerialDashVerticalSpeed));
			else
				EmitSignal(SignalName.ApplyAerialDashImpulse, new Vector2(ScaledAerialDashHorizontalSpeed, ScaledAerialDashVerticalSpeed));
		}
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
