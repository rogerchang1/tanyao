using Godot;
using System;
using Mahjong;
using Mahjong.Model;

public partial class Table : Godot.Node2D
{
	Mahjong.Model.Table _TableModel;
	Mahjong.CTableManager _TableManager;
	
	public Hand _PlayerHand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Mahjong.Model.Tile tile = new Mahjong.Model.Tile("1s");
		//CTableManager tableManager = new CTableManager();
		//Mahjong.Model.Table table = new Mahjong.Model.Table();
		_TableModel = new Mahjong.Model.Table();
		_TableManager = new Mahjong.CTableManager();
		InitializeTable();
		_PlayerHand = GetNode<Hand>("TableUI/Hand");
		var events = GetNode<Events>("/root/Events");
		events.DrawTileRequested += OnDrawTileRequested;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void InitializeTable()
	{
		_TableManager.InitializeTable(_TableModel);
	}
	
	public void OnDrawTileRequested()
	{
		Mahjong.Model.Tile DrawnTile = _TableManager.DrawNextTileFromWall(_TableModel);
		_PlayerHand.AddTile(DrawnTile);
		
	}
}
