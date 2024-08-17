using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public partial class BouleGroundCollision : RigidBody2D
{
    [Export]
    public float SizeScaling { get; set; } = 0.01f;

    private List<Node> groundCollisions = new();

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        //if (groundCollisions.Count != 0)
            //Size += SizeScaling;
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
}
