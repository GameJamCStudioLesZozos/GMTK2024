using Godot;
using System;

public partial class Obstacle : StaticBody2D
{
    [Export]
    public float RequiredPower { get; set; } = 10;

	// Todo shake on weak colision

	// Todo delete on strong enought colision
}
