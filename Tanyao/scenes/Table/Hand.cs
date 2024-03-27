using Godot;
using System;
using System.Collections.Generic;

public partial class Hand : HBoxContainer
{
	[Export]
	public PackedScene TileUIScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
		foreach(TileUI oTileUI in GetChildren())
		{
			oTileUI.ReparentRequested += OnTileUIReparentRequested;
			oTileUI.TileDiscarded += OnTileDiscarded;
		}
		SortTiles();
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
		foreach(TileUI oChild in GetChildren())
		{
			TilesInInitialPositions.Add(oChild);
			TilesToSort.Add(oChild._TileModel);
		}
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		oTilesManager.SortTiles(TilesToSort);
		
		foreach(TileUI oChild in GetChildren())
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
			MoveChild(TilesInInitialPositions[i],NewPositionIndexes[i]);
		}
	}
	
	public void AddTile(Mahjong.Model.Tile poNewTileModel)
	{
		TileUI NewTileUI = (TileUI) TileUIScene.Instantiate();
		NewTileUI.SetTile(poNewTileModel);
		
		NewTileUI.ReparentRequested += OnTileUIReparentRequested;
		NewTileUI.TileDiscarded += OnTileDiscarded;
		AddChild(NewTileUI);
		
		var tween = CreateTween();
		tween.TweenProperty(
			NewTileUI, 
			"position", 
			new Vector2((GetChildCount()) * (NewTileUI.Size.X),0),
		 	.1
		);
	}
	
	public void OnTileUIReparentRequested(TileUI oChild)
	{
		oChild.Reparent(this);
		SortTiles();
	}
	
	public void OnTileDiscarded()
	{
		SortTiles();
		//AddTile();
		var events = GetNode<Events>("/root/Events");
		events.EmitSignal(Events.SignalName.DrawTileRequested);
	}
	
	public void OnSortHandRequested()
	{
		SortTiles();
	}
	
}
