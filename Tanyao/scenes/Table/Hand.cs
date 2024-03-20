using Godot;
using System;

public partial class Hand : HBoxContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach(TileUI oTileUI in GetChildren())
		{
			oTileUI.ReparentRequested += OnTileUIReparentRequested;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnTileUIReparentRequested(TileUI oChild)
	{
		oChild.Reparent(this);
	}
	
}
