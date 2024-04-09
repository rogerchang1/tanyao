using Godot;
using System;

public partial class CallOptions : PanelContainer
{
	Button _Chi;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Chi = GetNode<Button>("ButtonContainer/Chi");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
