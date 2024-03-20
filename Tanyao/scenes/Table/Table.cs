using Godot;
using System;
using Mahjong;
using Mahjong.Model;

public partial class Table : Godot.Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Mahjong.Model.Tile tile = new Mahjong.Model.Tile("1s");
		//CTableManager tableManager = new CTableManager();
		//Mahjong.Model.Table table = new Mahjong.Model.Table();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnMouseEntered()
	{
		return;
	}
}
