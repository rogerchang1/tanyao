using Godot;
using System;
using System.Collections.Generic;
using Mahjong;
using Mahjong.Model;

//Mods the Table for easy testing of certin scenarios
public partial class TableWallModder : RefCounted
{
	Mahjong.CHandParser _HandParser = new Mahjong.CHandParser();
	
	public void ModTable(string psStartingHand1, string psStartingHand2, string psNextDraws, Mahjong.Model.Table poTable){
		List<Mahjong.Model.Tile> oStartingHand1 = _HandParser.ParseTileStringToTileList(psStartingHand1);
		List<Mahjong.Model.Tile> oStartingHand2 = _HandParser.ParseTileStringToTileList(psStartingHand2);
		List<Mahjong.Model.Tile> oNextDraws = _HandParser.ParseTileStringToTileList(psNextDraws);
		
		int index1 = 0;
		int index2 = 4;
		for(int i = 0; i < oStartingHand1.Count - 1; i++){
			poTable.Wall[index1] = oStartingHand1[i];
			poTable.Wall[index2] = oStartingHand2[i];
			index1++;
			index2++;
			if(index1 % 4 == 0){
				index1 += 4;
				index2 += 4;
			}
		}
		poTable.Wall[24] = oStartingHand1[oStartingHand1.Count - 1];
		poTable.Wall[25] = oStartingHand2[oStartingHand2.Count - 1];
		int index3 = 26;
		for(int i = 0; i < oNextDraws.Count; i++){
			poTable.Wall[index3] = oNextDraws[i];
			index3++;
		}
	}
}
