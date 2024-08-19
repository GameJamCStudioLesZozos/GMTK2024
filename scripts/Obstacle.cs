using Godot;
using System;

public partial class Obstacle : Area2D
{
    [Export]
    public float RequiredStrength { get; set; } = 10;

    [Export]
    public float DamageDealt { get; set; } = 0.5f;

    [Export]
    public bool DestroyedOnColison { get; set; } = false;

    public virtual void _OnBodyEntered(Node2D node)
    {
        if (node.IsInGroup("Player"))
        {
            ScalingComponent playerScalingComponent = node.GetNode<ScalingComponent>("ScalingComponent");
            if (playerScalingComponent == null)
                return;

            if (playerScalingComponent.GetStrength() < RequiredStrength)
            {
                Shake();
            }
            else
            {
                DeleteObstacle();
            }
        }
    }

    public void Shake()
    {
        // Todo
    }

    public void DeleteObstacle()
    {
        QueueFree();
    }
}