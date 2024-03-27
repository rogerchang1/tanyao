using Godot;
using System;

public partial class Events : Node
{
	//Tile-related events
	
	//Player-related events
	[Signal]
	public delegate void DrawTileRequestedEventHandler();
}
