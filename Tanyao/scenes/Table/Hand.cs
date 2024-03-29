using Godot;
using System;
using System.Collections.Generic;

public partial class Hand : HBoxContainer
{
	[Export]
	public PackedScene TileUIScene;
	
	public HBoxContainer _HandClosed;
	public HBoxContainer _HandTsumo;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
		_HandClosed = GetNode<HBoxContainer>("HandClosed");
		_HandTsumo = GetNode<HBoxContainer>("HandTsumo");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public async void SortTiles()
	{
		List<TileUI> TilesInInitialPositions = new List<TileUI>();
		List<int> NewPositionIndexes = new List<int>();
		List<Mahjong.Model.Tile> TilesToSort = new List<Mahjong.Model.Tile>();
		foreach(TileUI oChild in _HandClosed.GetChildren())
		{
			TilesInInitialPositions.Add(oChild);
			TilesToSort.Add(oChild._TileModel);
		}
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		oTilesManager.SortTiles(TilesToSort);
		
		foreach(TileUI oChild in _HandClosed.GetChildren())
		{
			int newPositionIndex = oTilesManager.FindFirstIndexOfTile(TilesToSort, oChild._TileModel);
			NewPositionIndexes.Add(newPositionIndex);
			TilesToSort[newPositionIndex] = null;
		}
		
		//Tweening Start
		var tween = CreateTween();
		for(int i = 0; i < NewPositionIndexes.Count;i++)
		{
			//GD.Print(oChild._Tile + ": " + oChild.Position + ", From: " + oChild.Position.X/64 + " To: " + newPositionIndex);
			TileUI t = TilesInInitialPositions[i];
			GD.Print(t._Tile + ": " + t.Position + ", From: " + t.Position.X/64 + " To: " + NewPositionIndexes[i]);
			tween.Parallel().TweenProperty(
				TilesInInitialPositions[i], 
				"position", 
				new Vector2(NewPositionIndexes[i] * (TilesInInitialPositions[i].Size.X+4), 0),
			 	.15
			);
		}
		await ToSignal(tween, Tween.SignalName.Finished);
		GD.Print("----");
		//Tweening End

		for(int i = 0; i < NewPositionIndexes.Count;i++)
		{
			_HandClosed.MoveChild(TilesInInitialPositions[i],NewPositionIndexes[i]);
		}
	}
	
	public void AddTileToHandTsumo(Mahjong.Model.Tile poNewTileModel)
	{
		TileUI NewTileUI = CreateNewTileUI(poNewTileModel);
		NewTileUI._ParentContainer = _HandTsumo;
		_HandTsumo.AddChild(NewTileUI);
	}
	
	public void AddTileToHandClosed(Mahjong.Model.Tile poNewTileModel)
	{
		TileUI NewTileUI = CreateNewTileUI(poNewTileModel);
		NewTileUI._ParentContainer = _HandClosed;
		_HandClosed.AddChild(NewTileUI);
	}
	
	private TileUI CreateNewTileUI(Mahjong.Model.Tile poNewTileModel)
	{
		TileUI NewTileUI = (TileUI) TileUIScene.Instantiate();
		NewTileUI.SetTile(poNewTileModel);
		NewTileUI.ReparentRequested += OnTileUIReparentRequested;
		NewTileUI.TileDiscarded += OnTileDiscarded;
		return NewTileUI;
	}
	
	public void OnTileUIReparentRequested(TileUI oChild, HBoxContainer oParent)
	{
		oChild.Reparent(oParent);
		if(oParent.Name == "HandClosed" && oParent.GetChildren().Count > 0)
		{
			SortTiles();
		}
	}
	
	//TODO: Remove async if you remove await later.
	public void OnTileDiscarded()
	{
		if(_HandTsumo.GetChildren().Count > 0)
		{
			foreach(TileUI oTileUI in _HandTsumo.GetChildren())
			{
				oTileUI.Reparent(_HandClosed);
				oTileUI._ParentContainer = _HandClosed;
			}
		}
		SortTiles();
		//TODO: Remove this later
		//await ToSignal(GetTree().CreateTimer(.5), "timeout");

		//TODO: This should later go in the StartTurn phase.
		var events = GetNode<Events>("/root/Events");
		events.EmitSignal(Events.SignalName.PlayerTileDiscarded);
	}
	
	public void OnSortHandRequested()
	{
		SortTiles();
	}
	
}
