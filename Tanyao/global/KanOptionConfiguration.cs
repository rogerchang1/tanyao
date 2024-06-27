using Godot;
using System;
using Mahjong.Model;

//Godot wrapper class for Mahjong.Model.Hand
public partial class KanOptionConfiguration : RefCounted
{
	public Mahjong.Model.Tile _Tile;
	public string _KanType = "";
	
	public KanOptionConfiguration(Mahjong.Model.Tile poTile, string psKanType){
		_Tile = poTile;
		_KanType = psKanType;
	}
}
