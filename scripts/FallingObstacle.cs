using Godot;
using System;

public partial class FallingObstacle : Obstacle
{
	[Export]
	public double FallingSpeed = 20;

	[Export]
	public Area2D FallingTriggerArea;

	private bool isFalling;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		isFalling = false;
		if (FallingTriggerArea != null)
			FallingTriggerArea.BodyEntered += CheckForBeginFall;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (isFalling)
			Position += new Vector2(0, 4);
	}

    public override void _OnBodyEntered(Node2D node)
    {
        base._OnBodyEntered(node);
		if(isFalling)
			QueueFree();
    }

	public void CheckForBeginFall(Node2D node)
	{
		if (node.IsInGroup("Player"))
			isFalling = true;
	}
}
