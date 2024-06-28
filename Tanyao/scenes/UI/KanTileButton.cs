using Godot;
using System;
using Mahjong.Model;

public partial class KanTileButton : Button
{
	[Export]
	public PackedScene TileUIScene;
	
	Events _Events;
	
	public KanOptionConfiguration _Tile1;
	
	public TileUI _TileUI;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		_TileUI = GetNode<TileUI>("TileUI");
	}
	
	public void SetUpWithTile(KanOptionConfiguration poTile1)
	{
		_TileUI.SetTile(poTile1._Tile);
		_Tile1 = poTile1;
	}
	
	public void _on_pressed()
	{
		GD.Print("KanTileButton Pressed: " +  _Tile1.ToString());
		_Events.EmitSignal(Events.SignalName.KanButtonPressed, _Tile1._Tile.ToString(), _Tile1._KanType);
	}

}
