using Godot;
using System;
using Mahjong;
using System.Collections.Generic;

//TODO: not sure if inheritance is the right thing to do
public partial class PlayerHandler : BaseHandler
{	
	[Export]
	Hand _PlayerHand;
	
	[Export]
	Label _ShantenLabel;
	
	[Export]
	CallOptionsUI _CallOptionsUI;
	
	[Export]
	Label _WinLabel;
	
	[Export]
	HBoxContainer _CalledHand;
	
	[Export]
	public PackedScene ChiScene;
	[Export]
	public PackedScene PonScene;
	//[Export]
	//public PackedScene KanScene;
	//[Export]
	//public PackedScene OpenKanScene;
	
	//Events _Events;
	
	public Mahjong.Model.Hand _Hand;
	
	public int _PlayerPoints = 0;
	
	public bool IsRiichi = false;
	public Enums.Wind _SeatWind;
	public Enums.Wind _RoundWind;
	public Mahjong.Model.Tile _DoraTile;
	
	//TODO: change this to an enum
	//Values are: START, BEFOREDRAW, AFTERDRAW, END
	public string TurnState = "START";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		//_Events.PlayerTileDiscarded += OnPlayerTileDiscarded;
		_Events.TileDiscarded += OnTileDiscarded;
		_Events.ChiButtonPressed += OnChiButtonPressed;
		_Events.PonButtonPressed += OnPonButtonPressed;
		_Events.RonButtonPressed += OnRonButtonPressed;
		_Events.TsumoButtonPressed += OnTsumoButtonPressed;
		_Events.RiichiButtonPressed += OnRiichiButtonPressed;
		_Events.KanButtonPressed += OnKanButtonPressed;
		_Events.CallOptionsCancelPressed += OnCallCancelButtonPressed;
		_Hand = new Mahjong.Model.Hand();
	}
	
	public void SortTilesUI()
	{
		_PlayerHand.SortTiles();
	}
	
	public override void StartTurn(string psDiscardedTile = "")
	{
		TurnState = "BEFOREDRAW";
		if(psDiscardedTile != "")
		{
			GD.Print("Enemy discarded " + psDiscardedTile);
			Mahjong.Model.Tile oDiscardedTile = new Mahjong.Model.Tile(psDiscardedTile);
			List<List<Mahjong.Model.Tile>> oChiAbleTiles = IsChi(oDiscardedTile);
			bool bShowCallOptions = false;
			if(oChiAbleTiles.Count > 0 && IsRiichi == false)
			{
				_CallOptionsUI.Show();
				_CallOptionsUI._Chi.Show();
				_CallOptionsUI.SetChiTileOptions(oChiAbleTiles);
				bShowCallOptions = true;
			}
			Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
			List<Mahjong.Model.Tile> oTempList = oTilesManager.GetTileListWithBlocksRemoved(_Hand.Tiles,_Hand.LockedBlocks);
			if(oTilesManager.CountNumberOfTilesOf(oTempList, oDiscardedTile) == 2 && IsRiichi == false)
			{
				_CallOptionsUI.Show();
				_CallOptionsUI._Pon.Show();
				bShowCallOptions = true;
			}
			
			_Hand.Tiles.Add(oDiscardedTile);
			Mahjong.Model.Score oScore = IsValidHand(oDiscardedTile, Enums.Agari.Ron);
			oTilesManager.RemoveSingleTileOf(_Hand.Tiles, oDiscardedTile);
			if(oScore != null && oScore.YakuList.Count > 0)
			{
				_CallOptionsUI.Show();
				_CallOptionsUI._Ron.Show();
				bShowCallOptions = true;
			}
			
			if(!bShowCallOptions)
			{
				_Events.EmitSignal(Events.SignalName.DrawTileRequested, this);
			}
			
		}
		else
		{
			_Events.EmitSignal(Events.SignalName.DrawTileRequested, this);
		}
	}
	
	public void EndTurn(Mahjong.Model.Tile oTile)
	{
		TurnState = "END";
		_PlayerHand.DisableAllTilesInteractability();
		_CallOptionsUI.HideAll();
		_Events.EmitSignal(Events.SignalName.PlayerTurnEnded, oTile.ToString());
	}
	
	public void AddTileToHandClosed(Mahjong.Model.Tile poNewTileModel)
	{
		TileUIConfiguration oTileUIConfiguration = new TileUIConfiguration();
		oTileUIConfiguration.InitialHandAreaToAppendTo = "CLOSED";
		_PlayerHand.AddTileToHand(poNewTileModel, oTileUIConfiguration);
		_Hand.Tiles.Add(poNewTileModel);
	}
	
	public void AddTileToHandTsumo(Mahjong.Model.Tile poNewTileModel)
	{
		TurnState = "AFTERDRAW";
		TileUIConfiguration oTileUIConfiguration = new TileUIConfiguration();
		oTileUIConfiguration.InitialHandAreaToAppendTo = "TSUMO";
		_PlayerHand.AddTileToHand(poNewTileModel, oTileUIConfiguration);
		
		_Hand.Tiles.Add(poNewTileModel);
		
		TriggerCallOptionsAtTsumo(poNewTileModel);
		
		if(!IsRiichi)
		{
			_PlayerHand.EnableAllTilesInteractability();	
		}
	}
	
	public async void TriggerCallOptionsAtTsumo(Mahjong.Model.Tile poNewTileModel)
	{
		Mahjong.CShantenEvaluator oShantenEvaluator = new Mahjong.CShantenEvaluator();
		int nShanten = oShantenEvaluator.EvaluateShanten(_Hand);
		_ShantenLabel.Text = "Shanten: " + nShanten;
		PrintHandForDebugging();
		if(nShanten == -1)
		{
			Mahjong.Model.Score oScore = IsValidHand(poNewTileModel, Enums.Agari.Tsumo);
			if(oScore != null && oScore.YakuList.Count > 0)
			{
				_CallOptionsUI.Show();
				_CallOptionsUI._Tsumo.Show();
			}
			if(IsRiichi)
			{
				return;
			}
		}
		if(nShanten == 0 && IsRiichi == false && _CalledHand.GetChildren().Count == 0)
		{
			_CallOptionsUI.Show();
			_CallOptionsUI._Riichi.Show();
		}else if(IsRiichi)
		{
			//TODO: Ideally not sure about this timer if needed.
			await ToSignal(GetTree().CreateTimer(.25), "timeout");
			foreach(TileUI oTsumoTile in _PlayerHand._HandTsumo.GetChildren())
			{
				var Discards = GetTree().GetFirstNodeInGroup("DiscardsGroup");
				if(Discards != null)
				{
					oTsumoTile.Reparent(Discards);
				}
				OnTileDiscarded(oTsumoTile);
			}
		}
	}
	
	public void Win(Mahjong.Model.Score poScore)
	{
		string sWinLabelText = "";
			
		foreach(Enums.Yaku yaku in poScore.YakuList)
		{
			sWinLabelText += yaku + "\n";
			GD.Print(yaku);
		}
		sWinLabelText += "Dora " + _Hand.DoraCount + "\n";
		sWinLabelText += poScore.Han + " Han " + poScore.Fu + " Fu\n";
		if(poScore.SinglePayment != 0){
			sWinLabelText += poScore.SinglePayment;
		}else{
			if(poScore.AllPayment["Dealer"] != 0)
			{
				sWinLabelText += poScore.AllPayment["Regular"] + " - " + poScore.AllPayment["Dealer"];
			}else{
				sWinLabelText += poScore.AllPayment["Regular"] + " All";
			}
			
		}
		
		UpdateWinLabel(sWinLabelText);
		
		int nPayment = poScore.SinglePayment;
		if(nPayment == 0)
		{
			if(_SeatWind == Enums.Wind.East)
			{
				nPayment = poScore.AllPayment["Regular"];
			}else{
				nPayment = poScore.AllPayment["Dealer"];
			}
			
		}
		
		_Events.EmitSignal(Events.SignalName.PlayerWinDeclared, nPayment);
		_Events.EmitSignal(Events.SignalName.RoundEnded);
	}
	
	public Mahjong.Model.Score IsValidHand(Mahjong.Model.Tile poWinTile, Enums.Agari peAgari)
	{
		//TODO evaluate the type of hand here.
		_Hand.Agari = peAgari;
		_Hand.SeatWind = _SeatWind;
		_Hand.RoundWind = _RoundWind;
		_Hand.WinTile = poWinTile;
		_Hand.DoraCount = 0;
		foreach(Mahjong.Model.Tile oTile in _Hand.Tiles)
		{
			if(oTile.CompareTo(_DoraTile) == 0)
			{
				_Hand.DoraCount++;
			}
		}
		
		
		Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
		Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(_Hand);
		return oScore;
		
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
	
	public void OnTileDiscarded(TileUI oTileUI)
	{
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		oTilesManager.RemoveSingleTileOf(_Hand.Tiles, oTileUI._TileModel);
		_Hand.DiscardedTiles.Add(oTileUI._TileModel);
		
		Mahjong.CShantenEvaluator oShantenEvaluator = new Mahjong.CShantenEvaluator();
		int nShanten = oShantenEvaluator.EvaluateShanten(_Hand);
		_ShantenLabel.Text = "Shanten: " + nShanten;
		
		EndTurn(oTileUI._TileModel);
	}
	
	public List<List<Mahjong.Model.Tile>> IsChi(Mahjong.Model.Tile poTile)
	{
		List<List<Mahjong.Model.Tile>> TilesCanChiWith = new List<List<Mahjong.Model.Tile>>();
		
		if(poTile.suit == "z")
		{
			return TilesCanChiWith;
		}
		
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		
		List<Mahjong.Model.Tile> TilesToSearchFor = new List<Mahjong.Model.Tile>();
		
		if(poTile.num - 2 > 0)
		{
			TilesToSearchFor.Add(new Mahjong.Model.Tile((poTile.num - 2) + poTile.suit));	
		}
		if(poTile.num - 1 > 0)
		{
			TilesToSearchFor.Add(new Mahjong.Model.Tile((poTile.num - 1) + poTile.suit));	
		}
		if(poTile.num + 1 < 10)
		{
			TilesToSearchFor.Add(new Mahjong.Model.Tile((poTile.num + 1) + poTile.suit));	
		}
		if(poTile.num + 2 < 10)
		{
			TilesToSearchFor.Add(new Mahjong.Model.Tile((poTile.num + 2) + poTile.suit));	
		}
		
		List<Mahjong.Model.Tile> oTempList = oTilesManager.GetTileListWithBlocksRemoved(_Hand.Tiles,_Hand.LockedBlocks);
		
		for(int i = 0;i < TilesToSearchFor.Count - 1;i++)
		{
			if(oTilesManager.ContainsTileOf(oTempList,TilesToSearchFor[i]) && 
			oTilesManager.ContainsTileOf(oTempList,TilesToSearchFor[i+1]))
			{
				List<Mahjong.Model.Tile> oChiTileBlock = new List<Mahjong.Model.Tile>(){TilesToSearchFor[i], TilesToSearchFor[i+1]};
				TilesCanChiWith.Add(oChiTileBlock);
			}
		}
		return TilesCanChiWith;
	}
	
	public void OnChiButtonPressed(string psTile1, string psTile2)
	{
		GD.Print("PlayerHandler: OnChiButtonPressed");
		GridContainer EnemyDiscardsGroup = (GridContainer) GetTree().GetFirstNodeInGroup("EnemyDiscardsGroup");
		TileUI oEnemyTileUI = (TileUI) EnemyDiscardsGroup.GetChild(EnemyDiscardsGroup.GetChildren().Count - 1);
		
		_Hand.Tiles.Add(oEnemyTileUI._TileModel);
		
		bool bTile1Found = false;
		bool bTile2Found = false;
		TileUI oTileUI1 = null;
		TileUI oTileUI2 = null;
		
		LockedBlock ChiBlock = (LockedBlock) ChiScene.Instantiate();
		_CalledHand.AddChild(ChiBlock);
		_CalledHand.MoveChild(ChiBlock, 0);
		//oEnemyTileUI.Reparent(_CalledHand);
		
		foreach(TileUI oTileUI in _PlayerHand._HandClosed.GetChildren())
		{
			if(oTileUI._TileModel.ToString() == psTile1 && !bTile1Found)
			{
				//oTileUI.Reparent(_CalledHand);
				oTileUI1 = oTileUI;
				bTile1Found = true;
			}
			if(oTileUI._TileModel.ToString() == psTile2 && !bTile2Found)
			{
				//oTileUI.Reparent(_CalledHand);
				oTileUI2 = oTileUI;
				bTile2Found = true;
			}
		}
		
		
		ChiBlock.SetUp(oEnemyTileUI._TileModel, oTileUI1._TileModel, oTileUI2._TileModel);
		
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		Mahjong.Model.Block oBlock = new Mahjong.Model.Block();
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oBlock.Tiles.Add(oTileUI1._TileModel);
		oBlock.Tiles.Add(oTileUI2._TileModel);
		oTilesManager.SortTiles(oBlock.Tiles);
		oBlock.IsOpen = true;
		oBlock.Type = Enums.Mentsu.Shuntsu;
		_Hand.LockedBlocks.Add(oBlock);
		
		oEnemyTileUI.QueueFree();
		oTileUI1.QueueFree();
		oTileUI2.QueueFree();
		
		_CallOptionsUI.HideAll();
		_PlayerHand.EnableAllTilesInteractability();
	}
	
	public void OnPonButtonPressed()
	{
		GD.Print("PlayerHandler: OnPonButtonPressed");
		GridContainer EnemyDiscardsGroup = (GridContainer) GetTree().GetFirstNodeInGroup("EnemyDiscardsGroup");
		TileUI oEnemyTileUI = (TileUI) EnemyDiscardsGroup.GetChild(EnemyDiscardsGroup.GetChildren().Count - 1);
		
		_Hand.Tiles.Add(oEnemyTileUI._TileModel);
		
		int nCount = 0;
		
		LockedBlock PonBlock = (LockedBlock) PonScene.Instantiate();
		_CalledHand.AddChild(PonBlock);
		_CalledHand.MoveChild(PonBlock, 0);
		
		foreach(TileUI oTileUI in _PlayerHand._HandClosed.GetChildren())
		{
			if(nCount >= 2)
			{
				break;
			}
			if(oTileUI._TileModel.ToString() == oEnemyTileUI._TileModel.ToString() && nCount < 2)
			{
				oTileUI.QueueFree();
				nCount++;
			}
		}
		
		PonBlock.SetUp(oEnemyTileUI._TileModel, oEnemyTileUI._TileModel, oEnemyTileUI._TileModel);
		
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		Mahjong.Model.Block oBlock = new Mahjong.Model.Block();
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oTilesManager.SortTiles(oBlock.Tiles);
		oBlock.IsOpen = true;
		oBlock.Type = Enums.Mentsu.Koutsu;
		_Hand.LockedBlocks.Add(oBlock);
		
		oEnemyTileUI.QueueFree();
		
		_CallOptionsUI.HideAll();
		_PlayerHand.EnableAllTilesInteractability();
	}
	
	public void OnRonButtonPressed()
	{
		GD.Print("PlayerHandler: OnRonButtonPressed");
		GridContainer EnemyDiscardsGroup = (GridContainer) GetTree().GetFirstNodeInGroup("EnemyDiscardsGroup");
		TileUI oEnemyTileUI = (TileUI) EnemyDiscardsGroup.GetChild(EnemyDiscardsGroup.GetChildren().Count - 1);
		
		_Hand.Tiles.Add(oEnemyTileUI._TileModel);
		Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
		Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(_Hand);
		if(oScore != null && oScore.YakuList.Count > 0)
		{
			Win(oScore);
		}
		_CallOptionsUI.HideAll();
	}
	
	public void OnTsumoButtonPressed()
	{
		GD.Print("PlayerHandler: OnTsumoButtonPressed");
		Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
		Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(_Hand);
		if(oScore != null && oScore.YakuList.Count > 0)
		{
			Win(oScore);
		}
		_CallOptionsUI.HideAll();
	}
	
	
	public void OnRiichiButtonPressed()
	{
		GD.Print("PlayerHandler: OnRiichiButtonPressed");
		IsRiichi = true;
		//TODO: double riichi?
		_Hand.IsRiichi = true;
		_PlayerPoints = _PlayerPoints - 1000;
		_CallOptionsUI.HideAll();
		_Events.EmitSignal(Events.SignalName.RiichiDeclared);
	}
	
	public void OnKanButtonPressed()
	{
		//TODO: Implement this
	}
	
	//TODO: Need to think about cancelling before and after drawn tile
	public void OnCallCancelButtonPressed()
	{
		if(TurnState == "BEFOREDRAW")
		{
			_PlayerHand.EnableAllTilesInteractability();
			_Events.EmitSignal(Events.SignalName.DrawTileRequested, this);
		}else if(TurnState == "AFTERDRAW" && IsRiichi)
		{
			foreach(TileUI oTsumoTile in _PlayerHand._HandTsumo.GetChildren())
			{
				var Discards = GetTree().GetFirstNodeInGroup("DiscardsGroup");
				if(Discards != null)
				{
					oTsumoTile.Reparent(Discards);
				}
				OnTileDiscarded(oTsumoTile);
			}

		}
	}
	
	public void CleanUp()
	{
		_PlayerHand.Clear();
		
		foreach(LockedBlock oLockedBlock in _CalledHand.GetChildren())
		{
			oLockedBlock.QueueFree();
		}
		
		var Discards = GetTree().GetFirstNodeInGroup("DiscardsGroup");
		foreach(TileUI oTileUI in Discards.GetChildren())
		{
			oTileUI.QueueFree();
		}
		
		//TODO: maybe make an init function for this.
		_Hand = new Mahjong.Model.Hand();
		IsRiichi = false;
	}
	
	public void PrintHandForDebugging()
	{
		string s = "";
		for(int i = 0;i<_Hand.Tiles.Count;i++)
		{
			s += _Hand.Tiles[i].ToString();
		}
		GD.Print(s);
	}
}
