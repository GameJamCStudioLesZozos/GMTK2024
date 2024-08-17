using Godot;
using System;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    [Signal]
    public delegate void PlayerDiedEventHandler();

    public override void _Ready()
    {
        Instance = this;
    }
}
