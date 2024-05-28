using Godot;
using System;
using System.Collections.Generic;
using Mahjong;

public partial class HandScore : Node2D
{
	
	public Mahjong.Model.Hand _Hand;
	public Mahjong.Model.Score _Score;
	
	Events _Events;
	
	[Export]
	public PackedScene _TileUIScene;
	[Export]
	public PackedScene _ChiScene;
	[Export]
	public PackedScene _PonScene;
	
	Label _WinLabel;
	Label _AgariLabel;
	
	HBoxContainer _ClosedHand;
	HBoxContainer _OpenHand;
	HBoxContainer _WinTile;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		_ClosedHand = GetNode<HBoxContainer>("ClosedHand");
		_OpenHand = GetNode<HBoxContainer>("OpenHand");
		_WinTile = GetNode<HBoxContainer>("WinTile");
		_WinLabel = GetNode<Label>("WinLabel");
		_AgariLabel = GetNode<Label>("AgariLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetLabels()
	{
		if(_Score != null && _Hand != null)
		{
			string sWinLabelText = "";
			foreach(Enums.Yaku yaku in _Score.YakuList)
			{
				sWinLabelText += yaku + "\n";
				
			}
			sWinLabelText += "Dora " + _Hand.DoraCount + "\n";
			if(_Hand.IsRiichi || _Hand.IsDoubleRiichi)
			{
				sWinLabelText += "UraDora " + _Hand.UraDoraCount + "\n";	
			}
			sWinLabelText += _Score.Han + " Han " + _Score.Fu + " Fu\n";
			if(_Score.SinglePayment != 0){
				sWinLabelText += _Score.SinglePayment;
			}else{
				if(_Score.AllPayment["Dealer"] != 0)
				{
					sWinLabelText += _Score.AllPayment["Regular"] + " - " + _Score.AllPayment["Dealer"];
				}else{
					sWinLabelText += _Score.AllPayment["Regular"] + " All";
				}
			}
			_WinLabel.Text = sWinLabelText;
			
			
			Mahjong.CTilesManager oTilesManager = new CTilesManager();
			oTilesManager.SortTiles(_Hand.Tiles);
			
			TileUI oWinTile = (TileUI) _TileUIScene.Instantiate();
			_WinTile.AddChild(oWinTile);
			oWinTile.SetTile(_Hand.WinTile);
			
			if(_Hand.Agari == Enums.Agari.Tsumo)
			{
				_AgariLabel.Text = "TSUMO";
			}else{
				_AgariLabel.Text = "RON";
			}
			
			oTilesManager.RemoveSingleTileOf(_Hand.Tiles,_Hand.WinTile);
			
			List<Mahjong.Model.Tile> ClosedTileList = oTilesManager.GetTileListWithBlocksRemoved(_Hand.Tiles,_Hand.LockedBlocks);
			for(int i = 0; i < ClosedTileList.Count;i++)
			{
				Mahjong.Model.Tile oTile = ClosedTileList[i];
				TileUI oTileUI = (TileUI) _TileUIScene.Instantiate();
				_ClosedHand.AddChild(oTileUI);
				oTileUI.SetTile(oTile);
				oTileUI._IsInteractable = false;
			}
			for(int i = 0; i < _Hand.LockedBlocks.Count;i++)
			{
				Mahjong.Model.Block oBlock = _Hand.LockedBlocks[i];
				if(oBlock.Type == Enums.Mentsu.Shuntsu)
				{
					LockedBlock ChiBlock = (LockedBlock) _ChiScene.Instantiate();
					_OpenHand.AddChild(ChiBlock);
					_OpenHand.MoveChild(ChiBlock, 0);
					ChiBlock.SetUp(oBlock.Tiles[0], oBlock.Tiles[1], oBlock.Tiles[2]);
				}
				if(oBlock.Type == Enums.Mentsu.Koutsu)
				{
					LockedBlock PonBlock = (LockedBlock) _PonScene.Instantiate();
					_OpenHand.AddChild(PonBlock);
					_OpenHand.MoveChild(PonBlock, 0);
					PonBlock.SetUp(oBlock.Tiles[0], oBlock.Tiles[1], oBlock.Tiles[2]);
				}
			}
		}
	}
	
	private void _on_confirm_pressed()
	{
		_Events.EmitSignal(Events.SignalName.HandScoreConfirmButtonPressed);
	}
}



