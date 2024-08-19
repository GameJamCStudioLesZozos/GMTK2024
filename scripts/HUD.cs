using Godot;
using System;
using System.Runtime.Serialization;

public partial class HUD : Control
{
	[Export]
	float updateUICooldown = 0.05f;
	Timer updateUITimer;

	private Main main;
	private RigidBody2D snowball;
	private ScalingComponent scalingComponent;
	private Label speed;
	private Label size;
	private Label time;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		main = GetTree().Root.GetNode<Main>("Main");
		snowball = GetTree().Root.GetNode<RigidBody2D>("Main/SnowBall");
		scalingComponent = snowball.GetNode<ScalingComponent>("ScalingComponent");
		speed = GetNode<Label>("ReferenceRect/Speed");
		size = GetNode<Label>("ReferenceRect/Size");
		time = GetNode<Label>("ReferenceRect/Time");

		updateUITimer = new Timer
		{
			WaitTime = updateUICooldown
		};
		AddChild(updateUITimer);

		updateUITimer.Timeout += UpdateUI;
		updateUITimer.Start();

		SignalBus.Instance.Connect(SignalBus.SignalName.PlayerDied, Callable.From(OnPlayerDead));
	}

	void UpdateUI()
	{
		float speedValue = snowball.LinearVelocity.Length()/100*3.6f;
		speed.Text = $"Speed: {speedValue:0} km/h";

		float scaleValue = scalingComponent.Radius;
		size.Text = "Size: " + (scaleValue * 2f).ToString("0.0") + " m";
		time.Text = "Time: " + GetTime();

		updateUITimer.Start();
	}


	private string GetTime()
	{
		TimeSpan timeSpan = main.GetTime();
		return string.Format("{0:m\\:ss\\.fff}", timeSpan);
	}


	public void OnPlayerDead()
	{
		updateUITimer.Stop();
	}

}
