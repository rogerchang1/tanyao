using Godot;
using System;
using Mahjong;

public partial class HandScore : Node2D
{
	
	public Mahjong.Model.Hand _Hand;
	public Mahjong.Model.Score _Score;
	
	Events _Events;
	
	[Export]
	public PackedScene _TileUIScene;
	
	Label _WinLabel;
	
	HBoxContainer _ClosedHand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		_ClosedHand = GetNode<HBoxContainer>("ClosedHand");
		_WinLabel = GetNode<Label>("WinLabel");
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
			for(int i = 0; i < _Hand.Tiles.Count;i++)
			{
				Mahjong.Model.Tile oTile = _Hand.Tiles[i];
				TileUI oTileUI = (TileUI) _TileUIScene.Instantiate();
				_ClosedHand.AddChild(oTileUI);
				oTileUI.SetTile(oTile);
				oTileUI._IsInteractable = false;
			}
		}
	}
	
	private void _on_confirm_pressed()
	{
		_Events.EmitSignal(Events.SignalName.HandScoreConfirmButtonPressed);
	}
}



