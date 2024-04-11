using Godot;
using System;
using Mahjong.Model;

public partial class ChiTileOptions : Node2D
{
	[Export]
	public PackedScene ChiTileButton;
	
	[Export]
	public HBoxContainer _HBoxContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//ClearButtons();
		//AddChiTileOption(new Mahjong.Model.Tile("3p"), new Mahjong.Model.Tile("6s"));
		//AddChiTileOption(new Mahjong.Model.Tile("3p"), new Mahjong.Model.Tile("6s"));
		//AddChiTileOption(new Mahjong.Model.Tile("3p"), new Mahjong.Model.Tile("6s"));
	}
	
	public void AddChiTileOption(Mahjong.Model.Tile poTile1, Mahjong.Model.Tile poTile2)
	{
		ChiTileButton NewChiTileButton = (ChiTileButton) ChiTileButton.Instantiate();
		NewChiTileButton.CreateChiTileButton(poTile1, poTile2);
		_HBoxContainer.AddChild(NewChiTileButton);
	}
	
	public void ClearButtons()
	{
		foreach(ChiTileButton oChiTileButton in _HBoxContainer.GetChildren())
		{
			_HBoxContainer.RemoveChild(oChiTileButton);
			oChiTileButton.QueueFree();
		}
	}
}
