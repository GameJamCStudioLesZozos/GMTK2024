using Godot;
using System;

public partial class AnimationComponent : Node
{
    [Export]
	public int InvincibilityBlinkCount { get; set; } = 1;

	private float lastInvincibilityDuration = 1;
	private float playerTakeDamageDuration;
    private Timer InvincibilityAnimationTimer = new Timer();
    private AnimationPlayer playerAnimation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        playerAnimation = GetNode<AnimationPlayer>("%AnimationPlayer");

        AddChild(InvincibilityAnimationTimer);
        InvincibilityAnimationTimer.OneShot = true;
        InvincibilityAnimationTimer.Timeout += () => playerAnimation.Play("Idle");

		playerTakeDamageDuration = playerAnimation.GetAnimation("SnowBallTakeDamage").Length;

        SignalBus.Instance.PlayerStartInvincibility += StartTakeDamageAnimation;
    }

	private void StartTakeDamageAnimation(float InvincibilityDuration)
	{
		lastInvincibilityDuration = InvincibilityDuration;
		playerAnimation.Play("SnowBallTakeDamage");
		playerAnimation.AnimationFinished += StartInvincibilityAnimation;
	}

	private void StartInvincibilityAnimation(StringName animmationName)
	{
		if(animmationName.Equals("SnowBallTakeDamage"))
		{
            playerAnimation.AnimationFinished -= StartInvincibilityAnimation;
			float InvincibilityDurationLeft = lastInvincibilityDuration - playerTakeDamageDuration;
            if (InvincibilityDurationLeft <= 0)
				return;
            playerAnimation.SpeedScale = InvincibilityBlinkCount / InvincibilityDurationLeft;
			InvincibilityAnimationTimer.Start(InvincibilityDurationLeft);
            playerAnimation.Play("SnowBallInvincibility");
        }
	}
}
