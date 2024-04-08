using Godot;
using System;

//TODO: not sure if inheritance is the right thing to do
public partial class EnemyHandler : BaseHandler
{	
	[Export]
	Hand _EnemyHand;
	
	
	//TODO: remove this when we fully build out enemy hand in UI layer.
	[Export]
	public PackedScene TileUIScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void AddTileToHandClosed(Mahjong.Model.Tile poNewTileModel)
	{
		TileUIConfiguration oTileUIConfiguration = new TileUIConfiguration();
		oTileUIConfiguration.IsInteractable = false;
		oTileUIConfiguration.InitialHandAreaToAppendTo = "CLOSED";
		_EnemyHand.AddTileToHand(poNewTileModel, oTileUIConfiguration);
	}
	
	public void AddTileToHandTsumo(Mahjong.Model.Tile poNewTileModel)
	{
		TileUIConfiguration oTileUIConfiguration = new TileUIConfiguration();
		oTileUIConfiguration.IsInteractable = false;
		oTileUIConfiguration.InitialHandAreaToAppendTo = "TSUMO";
		_EnemyHand.AddTileToHand(poNewTileModel, oTileUIConfiguration);
		DiscardTile();
	}
	
	public void DiscardTile()
	{
		//TODO: determine which tile to discard for enemy AI
		//For now, just discard the tsumo tile.
		if(_EnemyHand._HandTsumo.GetChildren().Count > 0)
		{
			TileUI oTileUI = (TileUI) _EnemyHand._HandTsumo.GetChild(0);
			AddTileToDiscards(oTileUI);
		}
		
	}
	public async void AddTileToDiscards(TileUI poTileUIToDiscard)
	{
		await ToSignal(GetTree().CreateTimer(.5), "timeout");
		var Discards = GetTree().GetFirstNodeInGroup("EnemyDiscardsGroup");
		poTileUIToDiscard.Reparent(Discards);
		//Discards.AddChild(poTileUIToDiscard);
		OnEnemyTileDiscarded();
	}
	
	public void OnEnemyTileDiscarded()
	{
		EndTurn();
	}
	
	public void EndTurn()
	{
		_Events.EmitSignal(Events.SignalName.EnemyTurnEnded);
	}
}
