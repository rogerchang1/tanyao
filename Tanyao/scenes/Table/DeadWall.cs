using Godot;
using System;

public partial class DeadWall : HBoxContainer
{
	
	
	[Export]
	PackedScene _TileUIScene;
	
	public void InitializeDeadWallUI(Mahjong.Model.Tile[] poDoraIndicatorArr)
	{
		TileUIConfiguration oTileUIConfiguration = new TileUIConfiguration();
		oTileUIConfiguration.IsInteractable = false;
		CreateNewTileUI(poDoraIndicatorArr[0], oTileUIConfiguration);
		oTileUIConfiguration.IsShown = false;
		CreateNewTileUI(poDoraIndicatorArr[1], oTileUIConfiguration);
		CreateNewTileUI(poDoraIndicatorArr[2], oTileUIConfiguration);
		CreateNewTileUI(poDoraIndicatorArr[3], oTileUIConfiguration);
		CreateNewTileUI(poDoraIndicatorArr[4], oTileUIConfiguration);
	}
	
	public void FlipKanDora(int nKanDoraTileIndex)
	{
		TileUI oTileUI = (TileUI) GetChild(nKanDoraTileIndex);
		oTileUI._IsShown = true;
		oTileUI.SetSprite();
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
