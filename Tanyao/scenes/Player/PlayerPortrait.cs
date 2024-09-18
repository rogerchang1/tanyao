using Godot;
using System;

public partial class PlayerPortrait : Node2D
{
	AnimationPlayer _AnimationPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_AnimationPlayer.Play("blink");
	}
}
