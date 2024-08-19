using Godot;
using System;
using System.Collections.Generic;

public partial class BallSnowCollision : Node
{
    [Signal]
    public delegate void ScaleRigidBodyEventHandler(float amount);

    [Export]
    public float SizeScaling { get; set; } = 1f;

    private RigidBody2D rigidBody2D;
    private List<Node> snowCollisions = new();

    public override void _Ready()
    {
        rigidBody2D = GetParent<RigidBody2D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (snowCollisions.Count != 0)
            EmitSignal(SignalName.ScaleRigidBody, SizeScaling * (float)delta * Math.Abs(rigidBody2D.AngularVelocity));
    }

    public void _OnBodyEntered(Node node)
    {
        if (node.IsInGroup("Snow"))
            snowCollisions.Add(node);
    }

    public void _OnBodyExited(Node node)
    {
        snowCollisions.Remove(node);
    }
}
