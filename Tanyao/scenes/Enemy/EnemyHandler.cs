using Godot;
using System;

//TODO: not sure if inheritance is the right thing to do
public partial class EnemyHandler : BaseHandler
{	
	
	[Export]
	Hand _EnemyDiscards;
	
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
	
	public void AddTileToDiscards(Mahjong.Model.Tile poNewTileModel)
	{
		var Discards = GetTree().GetFirstNodeInGroup("EnemyDiscardsGroup");
		TileUI NewTileUI = (TileUI) TileUIScene.Instantiate();
		NewTileUI.SetTile(poNewTileModel);
		Discards.AddChild(NewTileUI);
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
