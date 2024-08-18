using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ScalingComponent : Node
{
	[Export] public float sizeStrengthFactor = 1;
	[Export] public float speedStrengthFactor = 1;
	[Export] public float rotationStrengthFactor = 1;
    [Export] public float InvincibilityDuration = 1f;

    [Signal]
	public delegate void ScaledEventHandler(float newScale);

	[Export]
	public float Size { get; set; } = 1;

	public bool IsGameOver => Size <= 0;
	public bool IsInvincible { get; private set; } = false;
	private Timer InvincibilityTimer = new Timer();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		ScaleChildren();
		EmitSignal(SignalName.Scaled, Size);
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
		return parent.LinearVelocity.Length() * speedStrengthFactor + Size * sizeStrengthFactor + parent.AngularVelocity * rotationStrengthFactor;
	}

	public void TakeDamage(float amout)
	{
		if (IsInvincible)
			return;

		IsInvincible = true;
		InvincibilityTimer.Start(InvincibilityDuration);
		ScaleSize(-amout);
	}

	public void ScaleSize(float amount)
	{
		if (IsGameOver)
			return;

		Size += amount;
		if (IsGameOver)
		{
			Size = 0;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerDied);

			GetParent<Node2D>().Visible = false;
			GetParent().ProcessMode = ProcessModeEnum.Disabled;
			return;
		}

		EmitSignal(SignalName.Scaled, Size);
	}

	private void ScaleChildren()
	{
		foreach (Node child in GetParent().GetChildren())
		{
			if (child is Node2D child2D)
				child2D.Scale = new Vector2(Size, Size);
		}
	}
}
