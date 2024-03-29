using Godot;
using System;
using Mahjong;
using Mahjong.Model;

public partial class Table : Godot.Node2D
{
	Mahjong.Model.Table _TableModel;
	Mahjong.CTableManager _TableManager;
	
	public Hand _PlayerHand;
	public PlayerHandler _PlayerHandler;
	
	Events _Events;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_TableModel = new Mahjong.Model.Table();
		_TableManager = new Mahjong.CTableManager();
		
		_PlayerHand = GetNode<Hand>("TableUI/Hand");
		_PlayerHandler = GetNode<PlayerHandler>("PlayerHandler");
		_Events = GetNode<Events>("/root/Events");
		_Events.DrawTileRequested += OnDrawTileRequested;
		_Events.InitialTilesRequested += OnInitialTilesRequested;
		_Events.PlayerTurnEnded += OnPlayerTurnEnded;
		
		InitializeTable();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void InitializeTable()
	{
		_TableManager.InitializeTable(_TableModel);
		_PlayerHandler.InitializePlayerHand();
		StartRound();
	}
	
	public void StartRound()
	{
		//TODO: Later when there's an enemy handler
		//Just do a rand to flip a coin on who starts
		
		_PlayerHandler.StartTurn();
	}
	
	public void OnDrawTileRequested()
	{
		Mahjong.Model.Tile DrawnTile = _TableManager.DrawNextTileFromWall(_TableModel);
		_PlayerHandler.AddTileToHandTsumo(DrawnTile);
	}
	
	public void OnInitialTilesRequested()
	{
		Mahjong.Model.Tile DrawnTile = _TableManager.DrawNextTileFromWall(_TableModel);
		_PlayerHandler.AddTileToHandClosed(DrawnTile);
	}
	
	public void OnPlayerTurnStarted()
	{
		_PlayerHandler.StartTurn();
	}
	
	//TODO: Switch to EnemyHandler Turn when you implement EnemyHandler
	//TODO: Remove async when you don't need it anymore
	public async void OnPlayerTurnEnded()
	{
		await ToSignal(GetTree().CreateTimer(.5), "timeout");
		_PlayerHandler.StartTurn();
	}
}
