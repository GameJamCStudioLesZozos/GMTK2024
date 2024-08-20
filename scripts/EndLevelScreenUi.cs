using Godot;
using System;

public partial class EndLevelScreenUi : Control
{
    [Export]
    public Control DeathScreen { get; set; }

    private ColorRect EndLevelScreen;
    private Main main;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        EndLevelScreen = GetNode<ColorRect>("EndLevelScreen");
        EndLevelScreen.Hide();

        main = GetTree().Root.GetNode<Main>("Main");

        SignalBus.Instance.PlayerFinishedLevel += PlayerFinishedLevel;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PlayerFinishedLevel()
    {
        Label time = EndLevelScreen.GetNode<Label>("TimeValue");

        string strTime = GetTime();
        time.Text = strTime;

        if(DeathScreen != null)
            DeathScreen.QueueFree();
        EndLevelScreen.Show();
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
