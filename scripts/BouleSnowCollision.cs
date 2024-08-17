using Godot;
using System;
using System.Collections.Generic;

public partial class BouleSnowCollision : Node
{
    [Signal]
    public delegate void ScaleRigidBodyEventHandler(float amount);

    [Export]
    public float SizeScaling { get; set; } = 1f;

    private List<Node> groundCollisions = new();

    public override void _PhysicsProcess(double delta)
    {
        if (groundCollisions.Count != 0)
            EmitSignal(SignalName.ScaleRigidBody, SizeScaling * (float)delta);
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
