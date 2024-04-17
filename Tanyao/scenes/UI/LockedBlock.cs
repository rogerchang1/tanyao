using Godot;
using System;

public partial class LockedBlock : Control
{
	public TileUI _TileUI1;
	public TileUI _TileUI2;
	public TileUI _TileUI3;
	public TileUI _TileUI4;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("LockedBlock ready");
		_TileUI1 = GetNode<TileUI>("TileUI");
		_TileUI2 = GetNode<TileUI>("TileUI2");
		_TileUI3 = GetNode<TileUI>("TileUI3");
		//_TileUI4 = GetNode<TileUI>("TileUI4");
	}

	public void SetUp(Mahjong.Model.Tile poTile1, Mahjong.Model.Tile poTile2, 
					Mahjong.Model.Tile poTile3, Mahjong.Model.Tile poTile4 = null)
	{
		GD.Print("LockedBlock SetUp");
		_TileUI1.SetTile(poTile1);
		_TileUI2.SetTile(poTile2);
		_TileUI3.SetTile(poTile3);
		if(poTile4 != null)
		{
			_TileUI4.SetTile(poTile4);
		}
		
	}
}
