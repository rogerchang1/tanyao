using Godot;
using System;
using System.Collections.Generic;
using Mahjong;
using Mahjong.Model;

public partial class FuritenChecker : RefCounted
{
	public Boolean IsFuriten(List<Mahjong.Model.Tile> poDiscards, Mahjong.Model.Tile poTile){
		if(poDiscards.Contains(poTile)){
			return true;
		}
		return false;
	}
}
