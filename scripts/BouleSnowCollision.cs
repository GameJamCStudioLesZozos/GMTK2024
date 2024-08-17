using Godot;
using System;
using System.Collections.Generic;

public partial class BouleSnowCollision : Node
{
    [Signal]
    public delegate void ScaleRigidBodyEventHandler(float amount);

    [Export]
    public float SizeScaling { get; set; } = 0.01f;

    private List<Node> groundCollisions = new();

    public override void _Process(double delta)
    {
        if (groundCollisions.Count != 0)
            EmitSignal(SignalName.ScaleRigidBody, SizeScaling);
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
