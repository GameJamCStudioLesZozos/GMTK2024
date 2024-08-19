using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class MovementComponent : Node
{
	// serialized values
	[Export] public float JumpImpulse { get; set; } = 300f;
	[Export] public PackedScene SplitInstance;
	[Export] public float GravityMultiplier { get; set; } = 10f;
	[Export] public float Torque { get; set; } = 10f;
	[Export] public float GroundDashRotation { get; set; } = 30f;
	[Export] public float AirDashRotation { get; set; } = 30f;
	[Export] public float AerialDashHorizontalSpeed { get; set; } = 30f;
	[Export] public float AerialDashVerticalSpeed { get; set; } = 10f;
	[Export] public float MinSizeDashThreshold { get; set; } = 2f;
	[Export] public float DashSizeLostFactor { get; set; } = 0.25f;
	[Export] public double DashCooldown { get; set; } = 2;
	[Export] public double SplitCooldown { get; set; } = 2;
	[Export] public int NbFramesBufferJump { get; set; } = 6;
	[Export] public int NbFramesBufferDash { get; set; } = 3;

	// signals
	[Signal] public delegate void JumpedEventHandler(Vector2 impulse);
	[Signal] public delegate void ForceGravityEventHandler(float multiplier);
	[Signal] public delegate void ApplyTorqueEventHandler(float torque);
	[Signal] public delegate void ApplyDashRotationEventHandler(float dashRotation);
	[Signal] public delegate void ApplyAerialDashImpulseEventHandler(Vector2 aerialDashImpulse);
	[Signal] public delegate void LoseSizeEventHandler(float amount);
	[Signal] public delegate void SetBouncinessEventHandler(bool on);

	// fields
	private List<Node> groundCollisions = new();
	private ScalingComponent scalingComponent = null;
	private bool isGravityMultiplierOn = false;
	private Timer dashCooldownTimer = new();
	private Timer splitCooldownTimer = new();
	private bool isOnDashCooldown = false;
	private bool isOnSplitCooldown = false;
	private int jumpBufferFramesLeft = 0;
	private int dashBufferFramesLeft = 0;

	// properties
	private Vector2 ScaledJumpVector => new(0f, -JumpImpulse * (float)Math.Sqrt(scalingComponent.Size));
	private float ScaledTorque => Torque * scalingComponent.Size;
	private float ScaledGroundDashRotation => GroundDashRotation * scalingComponent.Size * scalingComponent.Size;
	private float ScaledAirDashRotation => AirDashRotation * scalingComponent.Size * scalingComponent.Size;
	private float CurrentScaledDashRotation => IsOnFloor ? ScaledGroundDashRotation : ScaledAirDashRotation;
	private float ScaledAerialDashHorizontalSpeed => AerialDashHorizontalSpeed * (float)Math.Sqrt(scalingComponent.Size);
	private float ScaledAerialDashVerticalSpeed => -AerialDashVerticalSpeed * (float)Math.Sqrt(scalingComponent.Size);
	private bool IsOnFloor => groundCollisions.Count != 0;
	private bool IsJumpRequested => jumpBufferFramesLeft >= 0;
	private bool IsDashRequested => dashBufferFramesLeft >= 0;

	public override void _Ready()
	{
		scalingComponent = (ScalingComponent)GetParent().GetChildren().First(node => node is ScalingComponent);
		AddChild(dashCooldownTimer);
		AddChild(splitCooldownTimer);
		dashCooldownTimer.Timeout += () => isOnDashCooldown = false;
		splitCooldownTimer.Timeout += () => isOnSplitCooldown = false;
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
		if (IsJumpRequested)
			--jumpBufferFramesLeft;

		if (Input.IsActionJustPressed("jump"))
			jumpBufferFramesLeft = NbFramesBufferJump;

		if (IsOnFloor && IsJumpRequested)
		{
			jumpBufferFramesLeft = 0;
			EmitSignal(SignalName.Jumped, ScaledJumpVector);
		}
	}

	private void checkSplit()
	{
		if (Input.IsActionJustPressed("split") && !isOnSplitCooldown)
		{
			scalingComponent.ScaleSize(-scalingComponent.Size / 2);
			var scene = GD.Load<PackedScene>(SplitInstance.ResourcePath);
			var instance = scene.Instantiate<Node2D>();
			instance.Position = GetParent<Node2D>().Position - this.GetParent<RigidBody2D>().LinearVelocity*0.5f;
			var newScalingComponent = instance.GetNode<ScalingComponent>("ScalingComponent");
			newScalingComponent.ScaleSize(scalingComponent.Size - newScalingComponent.Size);
			AddChild(instance);
			isOnSplitCooldown = true;
			splitCooldownTimer.Start(SplitCooldown);
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
		// dash at the end of the buffer, or as soon as we touch the ground

		bool dashBufferEnded = false;
		if (IsDashRequested)
		{
			--dashBufferFramesLeft;
			if (!IsDashRequested)
			{
				dashBufferEnded = true;
				EmitSignal(SignalName.SetBounciness, true);
			}
		}

		if (isOnDashCooldown || scalingComponent.Size <= MinSizeDashThreshold)
			return;

		if (!IsDashRequested && Input.IsActionJustPressed("dash"))
		{
			dashBufferFramesLeft = NbFramesBufferDash;
			EmitSignal(SignalName.SetBounciness, false);
		}

		if ((!IsDashRequested || !IsOnFloor) && !dashBufferEnded)
			return;

		float sign = Input.IsActionPressed("left") ? -1f : 1f;

		// if in the air : move horizontally
		if (!IsOnFloor)
			EmitSignal(SignalName.ApplyAerialDashImpulse, new Vector2(sign * ScaledAerialDashHorizontalSpeed, ScaledAerialDashVerticalSpeed));

		// always : increase rotation speed
		EmitSignal(SignalName.ApplyDashRotation, sign * CurrentScaledDashRotation);

		dashBufferFramesLeft = 0;
		isOnDashCooldown = true;
		dashCooldownTimer.Start(DashCooldown);
		EmitSignal(SignalName.LoseSize, -DashSizeLostFactor * scalingComponent.Size);
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
