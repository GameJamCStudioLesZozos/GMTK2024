using Godot;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;

public partial class Control : Godot.Control
{

	private Stopwatch stopwatch;
	private SignalBus signalBus;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stopwatch = new Stopwatch();
		stopwatch.Start();

		GetNode<ColorRect>("Deathscreen").Hide();

		signalBus = GetNode<SignalBus>("/root/SignalBus");
		signalBus.Connect(SignalBus.SignalName.PlayerDied, Callable.From(OnPlayerDead));
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPlayerDead()
	{
		stopwatch.Stop();

		Label time = GetNode<Label>("Deathscreen/Time");

		string strTime = stopwatch.Elapsed.ToString();
		Debug.WriteLine(strTime);
		time.Text += strTime;

        GetTree().Paused = true;
        GetNode<ColorRect>("Deathscreen").Show();
	}
	

	public void OnRetryPressed()
	{
		GetTree().ReloadCurrentScene();
	}

}
