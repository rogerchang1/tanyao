using Godot;
using System;
using Mahjong.Model;

public partial class ChiTileButton : Button
{
	[Export]
	public PackedScene TileUIScene;
	
	[Export]
	public HBoxContainer _HBoxContainer;
	
	Events _Events;
	
	public Mahjong.Model.Tile _Tile1;
	public Mahjong.Model.Tile _Tile2;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//_HBoxContainer = GetNode<HBoxContainer>("HBoxContainer");
		//CreateChiTileButton(new Mahjong.Model.Tile("3p"), new Mahjong.Model.Tile("6s"));
		_Events = GetNode<Events>("/root/Events");
	}
	
	public void CreateChiTileButton(Mahjong.Model.Tile poTile1, Mahjong.Model.Tile poTile2)
	{
		ClearTiles();
		TileUI NewTileUI1 = (TileUI) TileUIScene.Instantiate();
		NewTileUI1.SetTile(poTile1);
		NewTileUI1._IsInteractable = false;
		_Tile1 = poTile1;
		NewTileUI1.MouseFilter = MouseFilterEnum.Ignore;
		
		TileUI NewTileUI2 = (TileUI) TileUIScene.Instantiate();
		NewTileUI2.SetTile(poTile2);
		NewTileUI2._IsInteractable = false;
		_Tile2 = poTile2;
		NewTileUI2.MouseFilter = MouseFilterEnum.Ignore;
		
		_HBoxContainer.AddChild(NewTileUI1);
		_HBoxContainer.AddChild(NewTileUI2);
	}
	
	public void ClearTiles()
	{
		foreach(TileUI oTileUI in _HBoxContainer.GetChildren())
		{
			_HBoxContainer.RemoveChild(oTileUI);
			oTileUI.QueueFree();
		}
	}
	
	public void _on_pressed()
	{
		GD.Print("ChiTileButotn Pressed: " +  _Tile1.ToString() + _Tile2.ToString());
		_Events.EmitSignal(Events.SignalName.ChiButtonPressed, _Tile1.ToString(), _Tile2.ToString());
	}

}
