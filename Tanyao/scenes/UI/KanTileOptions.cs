using Godot;
using System;
using Mahjong.Model;

public partial class KanTileOptions : Node2D
{
	[Export]
	public PackedScene KanTileButton;
	
	public HBoxContainer _HBoxContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_HBoxContainer = GetNode<HBoxContainer>("HBoxContainer");
	}
	
	public void AddKanTileOption(KanOptionConfiguration poTile1)
	{
		KanTileButton NewKanTileButton = (KanTileButton) KanTileButton.Instantiate();
		_HBoxContainer.AddChild(NewKanTileButton);
		NewKanTileButton.SetUpWithTile(poTile1);
	}
	
	public void ClearButtons()
	{
		foreach(KanTileButton oKanTileButton in _HBoxContainer.GetChildren())
		{
			_HBoxContainer.RemoveChild(oKanTileButton);
			oKanTileButton.QueueFree();
		}
	}
}
