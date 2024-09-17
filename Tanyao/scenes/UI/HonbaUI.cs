using Godot;
using System;

public partial class HonbaUI : Control
{
	Label _HonbaLabel;
	Label _RiichiLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_HonbaLabel = GetNode<Label>("HonbaLabel");
		_RiichiLabel = GetNode<Label>("RiichiLabel");
	}
	
	public void SetHonbaLabelText(int pnHonbaValue){
		_HonbaLabel.Text = "x" + pnHonbaValue;
		
	}
	
	public void SetRiichiLabelText(int pnRiichiValue){
		_RiichiLabel.Text = "x" + pnRiichiValue;
	}
}
