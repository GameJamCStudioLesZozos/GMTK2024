using Godot;
using System;

public partial class Obstacle : Area2D
{
    [Export]
    public float RequiredStrength { get; set; } = 10;

    [Export]
    public float DamageDealt { get; set; } = 0.5f;

    public void _OnBodyEntered(Node2D node)
    {
        if (node.IsInGroup("Player"))
        {
            ScalingComponent playerScalingComponent = node.GetNode<ScalingComponent>("ScalingComponent");
            if (playerScalingComponent == null)
                return;

            if (playerScalingComponent.GetStrength() < RequiredStrength)
            {
                playerScalingComponent.ScaleSize(-DamageDealt);
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