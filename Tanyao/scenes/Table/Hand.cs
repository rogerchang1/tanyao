using Godot;
using System;

public partial class Hand : HBoxContainer
{
	[Export]
	public PackedScene TileUIScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//TileUIScene = GD.Load<PackedScene>("res://Tanyao/scenes/TileUI/TileUI.tscn");
		foreach(TileUI oTileUI in GetChildren())
		{
			oTileUI.ReparentRequested += OnTileUIReparentRequested;
			oTileUI.TileDiscarded += OnTileDiscarded;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void AddTile()
	{
		TileUI NewTileUI = (TileUI) TileUIScene.Instantiate();
		
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
	}
	
	public void OnTileDiscarded()
	{
		AddTile();
	}
	
}
