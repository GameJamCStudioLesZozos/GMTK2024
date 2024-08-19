using Godot;
using System;

public partial class SnowBallPhysics : RigidBody2D
{
	private float initialBounciness = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		initialBounciness = PhysicsMaterialOverride.Bounce;
	}

	public void SetBounciness(bool on)
	{
		PhysicsMaterialOverride.Bounce = on ? initialBounciness : 0f;
	}
}
