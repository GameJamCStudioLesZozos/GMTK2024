using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class RigidBody2dAutoScale : RigidBody2D
{
    [Export]
    public float Size { get; set; } = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		scaleChildren();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		scaleChildren();
	}

	private void scaleChildren()
	{
		foreach (Node2D child in GetChildren())
		{
			child.Scale = new Vector2(Size, Size);
		}
	}
}
