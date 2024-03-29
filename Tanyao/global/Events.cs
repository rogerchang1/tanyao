using Godot;
using System;

public partial class Events : Node
{
	//Tile-related events
	
	//Player-related events
	//TODO: maybe move the tile drawing under Tile-Related events as we will make an enemy handler later
	//and want to pass the type of handler to the table.
	[Signal]
	public delegate void InitialTilesRequestedEventHandler();
	[Signal]
	public delegate void DrawTileRequestedEventHandler();
	[Signal]
	public delegate void PlayerTileDiscardedEventHandler();
	[Signal]
	public delegate void PlayerTurnStartedEventHandler();
	[Signal]
	public delegate void PlayerTurnEndedEventHandler();
}
