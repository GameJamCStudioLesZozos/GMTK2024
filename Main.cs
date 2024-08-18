using Godot;
using System;
using System.Diagnostics;

public partial class Main : Node2D
{

	private Stopwatch stopwatch;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stopwatch = new Stopwatch();
		stopwatch.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public TimeSpan GetTime()
	{
		return stopwatch.Elapsed;
	}
}
