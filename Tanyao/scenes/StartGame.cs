using Godot;
using System;

public partial class StartGame : Node2D
{
	GameController _GC;
	
	public override void _Ready()
	{
		_GC = GetNode<GameController>("/root/GameController");
	}
	
	private void _on_start_game_button_pressed()
	{
		_GC.TransitionFromStartScreenToTable();
	}
	
	private void _on_exit_game_button_pressed()
	{
		GetTree().Quit();
	}
}
