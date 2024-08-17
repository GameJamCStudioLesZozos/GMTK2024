using Godot;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;

public partial class Control : Godot.Control
{

	Stopwatch stopwatch;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stopwatch = new Stopwatch();
		stopwatch.Start();

		GetNode<ColorRect>("Deathscreen").Hide();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_player_dead()
	{
		stopwatch.Stop();

		Label time = GetNode<Label>("Deathscreen/Time");

		string strTime = stopwatch.Elapsed.ToString();
		Debug.WriteLine(strTime);
		time.Text += strTime;

		GetNode<ColorRect>("Deathscreen").Show();
	}
	

	public void _on_retry_pressed()
	{
		GetTree().ReloadCurrentScene();
	}

}
