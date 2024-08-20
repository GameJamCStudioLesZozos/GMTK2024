using Godot;
using System;

public partial class EndLevelTrigger : Area2D
{
    public void _OnBodyEntered(Node node)
    {
        if (node.IsInGroup("Player"))
        {
            if(node.GetNode("Camera") == null)
            {
                return;
            }

            node.GetNode("Camera").Reparent(this);
            node.GetNode("MovementComponent").QueueFree();
            node.GetNode("ScalingComponent").QueueFree();
            node.GetNode("SnowCollisionComponent").QueueFree();

            SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerFinishedLevel);
        }
    }
}
