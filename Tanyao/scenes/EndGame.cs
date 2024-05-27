using Godot;
using System;

public partial class EndGame : Node2D
{
	public Label _EndScoresLabel;
	GameController _GC;
	
	public override void _Ready()
	{
		_EndScoresLabel = GetNode<Label>("EndScoresLabel");
		_GC = GetNode<GameController>("/root/GameController");
	}
	
	public void SetEndScoresLabel(string psPlayerScore, string psEnemyScore)
	{
		string sEndScoresText = "Player Score = " + psPlayerScore + "\n" + "Enemy Score = " + psEnemyScore + "\n";
		_EndScoresLabel.Text = sEndScoresText;
	}
	
	private void _on_end_game_confirm_button_pressed()
	{
		_GC.TransitionFromEndScreenToStartScreen();
	}
}



