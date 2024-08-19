using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ScalingComponent : Node
{
	[Export] public float RadiusStrengthFactor = 1;
	[Export] public float SpeedStrengthFactor = 1;
	[Export] public float InvincibilityDuration = 1f;
	[Export] public float MinRadius = 0.1f;
	[Export] public float MaxRadius = 100f;

	[Signal]
	public delegate void ScaledEventHandler(float newRadius);

	[Export]
	public float Radius { get; set; } = 1;

	public bool IsGameOver => Radius <= 0;
	public bool IsInvincible { get; private set; } = false;
	private Timer InvincibilityTimer = new Timer();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScaleChildren();
		EmitSignal(SignalName.Scaled, Radius);
		AddChild(InvincibilityTimer);
		InvincibilityTimer.Timeout += () => IsInvincible = false;
		InvincibilityTimer.OneShot = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ScaleChildren();
	}

	public float GetStrength()
	{
		var parent = GetParent<RigidBody2D>();
		return parent.LinearVelocity.Length() * SpeedStrengthFactor + Radius * RadiusStrengthFactor;
	}

	public void TakeDamage(float amout)
	{
		if (IsInvincible)
			return;

		IsInvincible = true;
		InvincibilityTimer.Start(InvincibilityDuration);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerStartInvincibility, InvincibilityDuration);
		AddRadius(-amout);
	}

	public void AddRadiusRatio(float ratio)
	{
		SetRadius(Radius * (1f + ratio));
	}

	public void SetRadiusRatio(float ratio)
	{
		SetRadius(Radius * ratio);
	}

	public void AddRadius(float amount)
	{
		SetRadius(Radius + amount);
	}

	public void SetRadius(float radius)
	{
		if (IsGameOver)
			return;

		Radius = Math.Clamp(radius, -MaxRadius, MaxRadius);
		if (IsGameOver)
		{
			Radius = 0;
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerDied);

			GetParent<Node2D>().Visible = false;
			GetParent().ProcessMode = ProcessModeEnum.Disabled;
			return;
		}

		EmitSignal(SignalName.Scaled, Radius);
	}

	private void ScaleChildren()
	{
		foreach (Node child in GetParent().GetChildren())
		{
			if (child is Node2D child2D)
				child2D.Scale = new Vector2(Radius, Radius);
		}
	}
}
