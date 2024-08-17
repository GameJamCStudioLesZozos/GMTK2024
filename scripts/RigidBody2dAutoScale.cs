using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class RigidBody2dAutoScale : Node
{
    [Export]
    public float Size { get; set; } = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		ScaleChildren();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ScaleChildren();
	}

	public float GetStrength()
	{
		// TODO update with what need
		return Size * 10;
	}

	public void ScaleSize(float amount)
	{
		Size += amount;
	}

	private void ScaleChildren()
	{
		foreach (Node child in GetParent().GetChildren())
		{
			if(child is Node2D child2D)
				child2D.Scale = new Vector2(Size, Size);
		}
	}
}
