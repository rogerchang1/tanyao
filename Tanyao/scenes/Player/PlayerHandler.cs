using Godot;
using System;
using Mahjong;
//TODO: not sure if inheritance is the right thing to do
public partial class PlayerHandler : BaseHandler
{	
	[Export]
	Hand _PlayerHand;
	
	[Export]
	Label _ShantenLabel;
	
	[Export]
	Label _WinLabel;
	
	//Events _Events;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		_Events.PlayerTileDiscarded += OnPlayerTileDiscarded;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void EndTurn()
	{
		_Events.EmitSignal(Events.SignalName.PlayerTurnEnded);
	}
	
	public void AddTileToHandClosed(Mahjong.Model.Tile poNewTileModel)
	{
		_PlayerHand.AddTileToHandClosed(poNewTileModel);
	}
	
	public void AddTileToHandTsumo(Mahjong.Model.Tile poNewTileModel)
	{
		_PlayerHand.AddTileToHandTsumo(poNewTileModel);
		IsValidHand(poNewTileModel);
	}
	
	public bool IsValidHand(Mahjong.Model.Tile poWinTile)
	{
		Mahjong.Model.Hand oHand = new Mahjong.Model.Hand();
		foreach(TileUI oTileUI in _PlayerHand._HandClosed.GetChildren())
		{
			oHand.Tiles.Add(oTileUI._TileModel);
		}
		if(_PlayerHand._HandTsumo.GetChildren().Count == 0)
		{
			return false;
		}else{
			var oNode = _PlayerHand._HandTsumo.GetChild(0);
			if(oNode != null && oNode.GetType() == typeof(TileUI))
			{
				oHand.Tiles.Add(((TileUI) oNode)._TileModel);
			}
		}
		
		Mahjong.CShantenEvaluator oShantenEvaluator = new Mahjong.CShantenEvaluator();
		int nShanten = oShantenEvaluator.EvaluateShanten(oHand);
		_ShantenLabel.Text = "Shanten: " + nShanten;
		if(nShanten == -1)
		{
			//TODO evaluate the type of hand here.
			oHand.Agari = Enums.Agari.Tsumo;
			oHand.SeatWind = Enums.Wind.East;
			oHand.RoundWind = Enums.Wind.East;
			oHand.WinTile = poWinTile;
			
			Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
			Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(oHand);
			string sWinLabelText = "";
			
			foreach(Enums.Yaku yaku in oScore.YakuList)
			{
				sWinLabelText += yaku + "\n";
				GD.Print(yaku);
			}
			sWinLabelText += oScore.Han + " Han " + oScore.Fu + " Fu";
			UpdateWinLabel(sWinLabelText);
		}
		
		return false;
	}
	
	public async void UpdateWinLabel(string psWinLabelText)
	{
		_WinLabel.Text = psWinLabelText;
		await ToSignal(GetTree().CreateTimer(5), "timeout");	
		_WinLabel.Text = "";
	}
	
	
	public void TestWinLabel()
	{
		Mahjong.CHandParser oHandParser = new Mahjong.CHandParser();
		Mahjong.Model.Hand oHand = oHandParser.ParseHand("123456s234567p55m");
		oHand.WinTile = new Mahjong.Model.Tile("7p");
		oHand.Agari = Enums.Agari.Tsumo;
		oHand.SeatWind = Enums.Wind.East;
		oHand.RoundWind = Enums.Wind.East;
		
		Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
		Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(oHand);
		string sWinLabelText = "";
		
		foreach(Enums.Yaku yaku in oScore.YakuList)
		{
			sWinLabelText += yaku + "\n";
			GD.Print(yaku);
		}
		sWinLabelText += oScore.Han + " Han " + oScore.Fu + " Fu";
		_WinLabel.Text = sWinLabelText;
	}
	
	public void OnPlayerTileDiscarded()
	{
		EndTurn();
	}

}
