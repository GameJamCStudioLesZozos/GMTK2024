using Godot;
using System;

public partial class DeathscreenUI : Control
{

	private ColorRect deathScreen;
	private Main main;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		deathScreen = GetNode<ColorRect>("Deathscreen");
		deathScreen.Hide();

		main = GetTree().Root.GetNode<Main>("Main");

		SignalBus.Instance.Connect(SignalBus.SignalName.PlayerDied, Callable.From(OnPlayerDead));
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPlayerDead()
	{
		Label time = deathScreen.GetNode<Label>("Time");

		string strTime = GetTime();
		time.Text += strTime;

        deathScreen.Show();
	}
	

	public void OnRetryPressed()
	{
        GetTree().ReloadCurrentScene();
	}


	private string GetTime()
	{
		TimeSpan timeSpan = main.GetTime();
		return string.Format("{1:s\\.fff}", "s\\.fff", timeSpan);
	}
}
