using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class RigidBody2dAutoScale : RigidBody2D
{
	[Export]
	public float Size { get; set; } = 1;

	[Export]
	public float SizeScaling { get; set; } = 1f;

	[Export]
	public float JumpImpulse { get; set; } = 100f;

	private List<Node> collisions = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scaleChildren();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (collisions.Count != 0)
			Size += SizeScaling * (float)delta;

		scaleChildren();

		jump();
	}

	public void _OnBodyEntered(Node node)
	{
		if (node.IsInGroup("Ground"))
			collisions.Add(node);
	}

	public void _OnBodyExited(Node node)
	{
		collisions.Remove(node);
	}

	private void scaleChildren()
	{
		foreach (Node2D child in GetChildren())
		{
			child.Scale = new Vector2(Size, Size);
		}
	}

	private void jump()
	{
		if(Input.IsActionPressed("jump") && collisions.Count != 0)
		{
			ApplyImpulse(new Vector2(0f, -JumpImpulse));
		}
	}
}
