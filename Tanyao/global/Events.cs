using Godot;
using System;
using Mahjong.Model;

public partial class Events : Node
{
	//Tile-related events
	[Signal]
	public delegate void TileDiscardedEventHandler(TileUI oTile);
	
	//Player-related events
	//TODO: maybe move the tile drawing under Tile-Related events as we will make an enemy handler later
	//and want to pass the type of handler to the table.
	[Signal]
	public delegate void DrawTileRequestedEventHandler(BaseHandler oBaseHandler);
	[Signal]
	public delegate void DrawKanTileRequestedEventHandler(BaseHandler oBaseHandler);
	[Signal]
	public delegate void PlayerTileDiscardedEventHandler(TileUI oTile);
	[Signal]
	public delegate void PlayerTurnStartedEventHandler(string psTile);
	[Signal]
	public delegate void PlayerTurnEndedEventHandler(string psTile);
	
	[Signal]
	public delegate void EnemyTurnStartedEventHandler(string psTile);
	[Signal]
	public delegate void EnemyTurnEndedEventHandler(string psTile);
	
	[Signal]
	public delegate void CallOptionsCancelPressedEventHandler();
	[Signal]
	public delegate void ChiButtonPressedEventHandler(string psTile1, string psTile2);
	[Signal]
	public delegate void KanButtonPressedEventHandler(string psTile1);
	[Signal]
	public delegate void PonButtonPressedEventHandler();
	[Signal]
	public delegate void RonButtonPressedEventHandler();
	[Signal]
	public delegate void TsumoButtonPressedEventHandler();
	[Signal]
	public delegate void RiichiButtonPressedEventHandler();
	
	[Signal]
	public delegate void PlayerWinDeclaredEventHandler(int pnPayment, HandGodotWrapper poHand, ScoreGodotWrapper poScore);
	[Signal]
	public delegate void EnemyWinDeclaredEventHandler(int pnPayment);
	[Signal]
	public delegate void RoundEndedEventHandler();
	[Signal]
	public delegate void RiichiDeclaredEventHandler();
	
	[Signal]
	public delegate void HandScoreConfirmButtonPressedEventHandler();
}
