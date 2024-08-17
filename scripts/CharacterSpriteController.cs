using Godot;
using System;

public partial class CharacterSpriteController : Node2D
{
	private float rotationalSpeed;

	[Export] public Texture2D happy;
	[Export] public Texture2D surprised;
	[Export] public Texture2D concerned;
	[Export] public Texture2D ill;

	[Export] public float surprisedTreshold = 10;
	[Export] public float concernedTreshold = 20;
	[Export] public float illTreshold = 30;
	
	private int status = 0;

	private Sprite2D node;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		node = GetNode<Sprite2D>("Node");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		rotationalSpeed = (GetParent() as RigidBody2D).AngularVelocity;
		if (rotationalSpeed > 0 && status != 0)
		{
			node.Texture = happy;
			status = 0;
		}
		if (rotationalSpeed > surprisedTreshold && status != 1)
		{
			node.Texture = surprised;
			status = 1;
		}
		if (rotationalSpeed > concernedTreshold && status != 2)
		{
			node.Texture = concerned;
			status = 2;
		}
		if (rotationalSpeed > illTreshold && status != 3)
		{
			node.Texture = ill;
			status = 3;
		}
	}
}
