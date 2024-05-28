using Godot;
using System;

public partial class DeadWall : HBoxContainer
{
	
	
	[Export]
	PackedScene _TileUIScene;
	
	public void InitializeDeadWallUI(Mahjong.Model.Tile poDoraIndicator,
								Mahjong.Model.Tile poKanDora1,
								Mahjong.Model.Tile poKanDora2,
								Mahjong.Model.Tile poKanDora3,
								Mahjong.Model.Tile poKanDora4)
	{
		TileUIConfiguration oTileUIConfiguration = new TileUIConfiguration();
		oTileUIConfiguration.IsInteractable = false;
		CreateNewTileUI(poDoraIndicator, oTileUIConfiguration);
		oTileUIConfiguration.IsShown = false;
		CreateNewTileUI(poKanDora1, oTileUIConfiguration);
		CreateNewTileUI(poKanDora2, oTileUIConfiguration);
		CreateNewTileUI(poKanDora3, oTileUIConfiguration);
		CreateNewTileUI(poKanDora4, oTileUIConfiguration);
	}
	
	private TileUI CreateNewTileUI(Mahjong.Model.Tile poNewTileModel, TileUIConfiguration poTileUIConfiguration)
	{
		TileUI NewTileUI = (TileUI) _TileUIScene.Instantiate();
		AddChild(NewTileUI);
		NewTileUI._IsInteractable = poTileUIConfiguration.IsInteractable;
		NewTileUI._IsShown = poTileUIConfiguration.IsShown;
		NewTileUI.SetTile(poNewTileModel);
		return NewTileUI;
	}
	
	public void CleanUp()
	{
		foreach(TileUI oTileUI in GetChildren())
		{
			oTileUI.QueueFree();
		}
	}
	
}
