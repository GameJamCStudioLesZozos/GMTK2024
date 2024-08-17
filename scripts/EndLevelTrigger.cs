using Godot;
using System;

public partial class EndLevelTrigger : Area2D
{
    public void _OnBodyEntered(Node node)
    {
        if (node.IsInGroup("Player"))
        {
            node.GetChild(-1).GetChild(-1).Reparent(this);
        }
    }
}
