using Godot;
using System;
using Mahjong.Model;

public partial class KanTileButton : Button
{
	[Export]
	public PackedScene TileUIScene;
	
	Events _Events;
	
	public Mahjong.Model.Tile _Tile1;
	
	public TileUI _TileUI;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		_TileUI = GetNode<TileUI>("TileUI");
	}
	
	public void SetUpWithTile(Mahjong.Model.Tile poTile1)
	{
		//ClearTiles();
		//TileUI NewTileUI1 = (TileUI) TileUIScene.Instantiate();
		//AddChild(NewTileUI1);
		//NewTileUI1._IsInteractable = false;
		//NewTileUI1.MouseFilter = MouseFilterEnum.Ignore;
		//NewTileUI1.SetTile(poTile1);
		_TileUI.SetTile(poTile1);
		_Tile1 = poTile1;
	}
	
	public void ClearTiles()
	{
		//TileUI oTileUIToDelete = GetNode<TileUI>("TileUI");
		//RemoveChild(oTileUIToDelete);
		//oTileUIToDelete.QueueFree();
	}
	
	public void _on_pressed()
	{
		GD.Print("KanTileButton Pressed: " +  _Tile1.ToString());
		_Events.EmitSignal(Events.SignalName.KanButtonPressed, _Tile1.ToString());
	}

}
