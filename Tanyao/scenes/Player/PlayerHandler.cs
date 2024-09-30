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
	[Export]
	public PackedScene ClosedKanScene;
	[Export]
	public PackedScene DaiminKanScene;
	[Export]
	public PackedScene ShouminKanScene;
	
	//Events _Events;
	
	public Mahjong.Model.Hand _Hand;
	
	public int _PlayerPoints = 0;
	public int _Honba = 0;
	
	public bool IsRiichi = false;
	private List<Mahjong.Model.Tile> _RiichiWaits = new List<Mahjong.Model.Tile>();
	public Enums.Wind _SeatWind;
	public Enums.Wind _RoundWind;
	public Mahjong.Model.Tile[] _DoraTileArr;
	public Mahjong.Model.Tile[] _UraDoraTileArr;
	public int _NumKanDoraActive = 0;
	public bool _RequestFlipKanDora = false;
	public int _RiichiTileCounter;
	public int _WallTileCounter;
	
	//For testing purposes and convenience. Should be set to false when not testing.
	public bool _BypassFuriten = false;
	
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
		_Events.DaiminKanButtonPressed += OnDaiminKanButtonPressed;
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
		_PlayerHand.DisableAllTilesInteractability();
		if(psDiscardedTile != "")
		{
			GD.Print("PlayerHandler: Enemy discarded " + psDiscardedTile);
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
			int nTileCount = oTilesManager.CountNumberOfTilesOf(oTempList, oDiscardedTile);
			if(nTileCount >= 2 && IsRiichi == false)
			{
				_CallOptionsUI.Show();
				_CallOptionsUI._Pon.Show();
				if(nTileCount >= 3){
					_CallOptionsUI._DaiminKan.Show();
				}
				bShowCallOptions = true;
			}
			
			_Hand.Tiles.Add(oDiscardedTile);
			Mahjong.Model.Score oScore = IsValidHand(oDiscardedTile, Enums.Agari.Ron);
			oTilesManager.RemoveSingleTileOf(_Hand.Tiles, oDiscardedTile);
			if(oScore != null && oScore.YakuList.Count > 0)
			{
				FuritenChecker oFuritenChecker = new FuritenChecker();
				if(!oFuritenChecker.IsFuriten(_Hand.DiscardedTiles,oDiscardedTile) || _BypassFuriten){
					_CallOptionsUI.Show();
					_CallOptionsUI._Ron.Show();
					bShowCallOptions = true;
				}
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
		if(_RequestFlipKanDora){
			_RequestFlipKanDora = false;
			_Events.EmitSignal(Events.SignalName.FlipKanDoraDeclared);
		}
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
		
		bool bPauseAutoDiscardForCalls = false;
		
		//TODO: should be allowed to riichi/tsumo/kan when drawn from dead wall
		//TODO: Needs checks if it's not the last tile in the wall 
		List<KanOptionConfiguration> oKannableTiles = GetKanTiles();
		if(oKannableTiles.Count > 0)
		{
			_CallOptionsUI.Show();
			_CallOptionsUI._Kan.Show();
			_CallOptionsUI.SetKanTileOptions(oKannableTiles);
			if(IsRiichi)
			{
				bPauseAutoDiscardForCalls = true;
			}
		}
		
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
				bPauseAutoDiscardForCalls = true;
			}
		}
		
		if(bPauseAutoDiscardForCalls){
			return;
		}

		if(nShanten == 0 && IsRiichi == false && !HasOpenBlocks())
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
	
	public bool HasOpenBlocks(){
		for(int i = 0;i<_Hand.LockedBlocks.Count;i++){
			Mahjong.Model.Block oBlock = _Hand.LockedBlocks[i];
			if(oBlock.IsOpen == true){
				return true;
			}
		}
		return false;
	}
	
	public void Win(Mahjong.Model.Score poScore)
	{
		string sWinLabelText = "";
			
		foreach(Enums.Yaku yaku in poScore.YakuList)
		{
			sWinLabelText += yaku + "\n";
			GD.Print("PlayerHandler Win(): " + yaku);
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
		
		int nPayment = poScore.SinglePayment + _Honba;
		if(nPayment == 0)
		{
			if(_SeatWind == Enums.Wind.East)
			{
				nPayment = poScore.AllPayment["Regular"];
			}else{
				nPayment = poScore.AllPayment["Dealer"];
			}
			nPayment += (_Honba * 100);
		}else{
			nPayment += (_Honba * 300);
		}
		
		ScoreGodotWrapper gScore = new ScoreGodotWrapper();
		gScore._Score = poScore;
		HandGodotWrapper gHand = new HandGodotWrapper();
		gHand._Hand = _Hand;
		
		_Events.EmitSignal(Events.SignalName.PlayerWinDeclared, nPayment, gHand, gScore);
		//_Events.EmitSignal(Events.SignalName.RoundEnded);
	}
	
	public Mahjong.Model.Score IsValidHand(Mahjong.Model.Tile poWinTile, Enums.Agari peAgari)
	{
		//TODO evaluate the type of hand here.
		_Hand.Agari = peAgari;
		_Hand.SeatWind = _SeatWind;
		_Hand.RoundWind = _RoundWind;
		_Hand.WinTile = poWinTile;
		_Hand.DoraCount = 0;
		_Hand.UraDoraCount = 0;
		
		
		if(_RiichiTileCounter != -1 && _RiichiTileCounter <= 2){
			_Hand.IsDoubleRiichi = true;
		}
		foreach(Mahjong.Model.Tile oTile in _Hand.Tiles)
		{
			for(int i = 0; i <= _NumKanDoraActive; i++)
			{
				Mahjong.Model.Tile oDoraTile = _DoraTileArr[i];
				if(oTile.CompareTo(oDoraTile) == 0)
				{
					_Hand.DoraCount++;
				}
				
				Mahjong.Model.Tile oUraDoraTile = _UraDoraTileArr[i];
				if((_Hand.IsRiichi || _Hand.IsDoubleRiichi) && oTile.CompareTo(oUraDoraTile) == 0)
				{
					_Hand.UraDoraCount++;
				}
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
			GD.Print("PlayerHandler TestWinLabel(): " + yaku);
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
		
		if(IsRiichi && _RiichiWaits.Count == 0){
			Mahjong.CHandWaitEvaluator oHandWaitEvaluator = new Mahjong.CHandWaitEvaluator();
			_RiichiWaits = oHandWaitEvaluator.EvaluateWaits(_Hand);
		}
		
		EndTurn(oTileUI._TileModel);
	}
	
	public List<KanOptionConfiguration> GetKanTiles(){
		List<Mahjong.Model.Tile> oDaiminKannableTiles = GetKannableTiles();
		List<KanOptionConfiguration> oKanTiles = new List<KanOptionConfiguration>();
		foreach(Mahjong.Model.Tile oTile in oDaiminKannableTiles){
			oKanTiles.Add(new KanOptionConfiguration(oTile, "ankan"));
		}
		List<Mahjong.Model.Tile> oShouminKannableTiles = GetShouminKannableTiles();
		foreach(Mahjong.Model.Tile oTile in oShouminKannableTiles){
			oKanTiles.Add(new KanOptionConfiguration(oTile, "shouminkan"));
		}
		return oKanTiles;
	}
	
	public void IppatsuCheck()
	{
		//TODO: Consider making ippatsu break be having riichitilecounter go back to -1?
		GD.Print("_RiichiTilecounter: " + _RiichiTileCounter + " _WallTileCounter: " + _WallTileCounter);
		if(_RiichiTileCounter != -1 && _RiichiTileCounter + 2 >= _WallTileCounter){
			GD.Print("IsIppatsu = true");
			_Hand.IsIppatsu = true;
		}
	}
	
	public List<Mahjong.Model.Tile> GetKannableTiles()
	{
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		List<Mahjong.Model.Tile> oTempList = oTilesManager.GetTileListWithBlocksRemoved(_Hand.Tiles,_Hand.LockedBlocks);
		oTilesManager.SortTiles(oTempList);
		int cnt = 1;
		Mahjong.Model.Tile oCurTile = oTempList[0];
		List<Mahjong.Model.Tile> oKannableTiles = new List<Mahjong.Model.Tile>();
		for(int i = 1;i < oTempList.Count;i++)
		{
			if(oTempList[i].CompareTo(oCurTile) == 0)
			{
				cnt++;
			}else{
				cnt = 1;
				oCurTile = oTempList[i];
			}
			if(cnt == 4)
			{
				oKannableTiles.Add(oCurTile);
			}
		}
		if(IsRiichi == true && oKannableTiles.Count > 0){
			for(int i = 0; i < oKannableTiles.Count; i++){
				Mahjong.Model.Tile oKanTile = oKannableTiles[i];
				Mahjong.CHandUtility oHandUtility = new Mahjong.CHandUtility();
				Mahjong.Model.Hand oTempHand = oHandUtility.Clone(_Hand);
				Mahjong.Model.Block oBlock = new Mahjong.Model.Block();
				oBlock.KanType = Enums.KanType.Ankan;
				oBlock.Type = Enums.Mentsu.Kantsu;
				oBlock.Tiles.Add(oKanTile);
				oBlock.Tiles.Add(oKanTile);
				oBlock.Tiles.Add(oKanTile);
				oBlock.Tiles.Add(oKanTile);
				oTempHand.LockedBlocks.Add(oBlock);
				Mahjong.CHandWaitEvaluator oHandWaitEvaluator = new Mahjong.CHandWaitEvaluator();
				List<Mahjong.Model.Tile> oNewWaits = oHandWaitEvaluator.EvaluateWaits(oTempHand);
				
				if(oNewWaits.Count != _RiichiWaits.Count){
					oKannableTiles.RemoveAt(i);
				}
			}
		}
		return oKannableTiles;
	}
	
	public List<Mahjong.Model.Tile> GetShouminKannableTiles()
	{
		List<Mahjong.Model.Tile> oShounminKannableTiles = new List<Mahjong.Model.Tile>();
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		List<Mahjong.Model.Tile> oTempList = oTilesManager.GetTileListWithBlocksRemoved(_Hand.Tiles,_Hand.LockedBlocks);
		for(int i = 0;i<_Hand.LockedBlocks.Count;i++){
			Mahjong.Model.Block oBlock = _Hand.LockedBlocks[i];
			if(oBlock.Type == Enums.Mentsu.Koutsu && oTilesManager.CountNumberOfTilesOf(oTempList, oBlock.Tiles[0]) >= 1){
				oShounminKannableTiles.Add(oBlock.Tiles[0]);
			}
		}
		return oShounminKannableTiles;
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
		_CallOptionsUI.HideAll();
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
		
		_PlayerHand.EnableAllTilesInteractability();
	}
	
	public void OnPonButtonPressed()
	{
		GD.Print("PlayerHandler: OnPonButtonPressed");
		_CallOptionsUI.HideAll();
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
		
		_PlayerHand.EnableAllTilesInteractability();
	}
	
	public void OnDaiminKanButtonPressed()
	{
		GD.Print("PlayerHandler: OnDaiminKanButtonPressed");
		_CallOptionsUI.HideAll();
		GridContainer EnemyDiscardsGroup = (GridContainer) GetTree().GetFirstNodeInGroup("EnemyDiscardsGroup");
		TileUI oEnemyTileUI = (TileUI) EnemyDiscardsGroup.GetChild(EnemyDiscardsGroup.GetChildren().Count - 1);
		
		_Hand.Tiles.Add(oEnemyTileUI._TileModel);
		
		int nCount = 0;
		
		LockedBlock DaiminKanBlock = (LockedBlock) DaiminKanScene.Instantiate();
		_CalledHand.AddChild(DaiminKanBlock);
		_CalledHand.MoveChild(DaiminKanBlock, 0);
		
		foreach(TileUI oTileUI in _PlayerHand._HandClosed.GetChildren())
		{
			if(nCount >= 3)
			{
				break;
			}
			if(oTileUI._TileModel.ToString() == oEnemyTileUI._TileModel.ToString() && nCount < 3)
			{
				oTileUI.QueueFree();
				nCount++;
			}
		}
		
		DaiminKanBlock.SetUp(oEnemyTileUI._TileModel, oEnemyTileUI._TileModel, oEnemyTileUI._TileModel, oEnemyTileUI._TileModel);
		
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		Mahjong.Model.Block oBlock = new Mahjong.Model.Block();
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oBlock.Tiles.Add(oEnemyTileUI._TileModel);
		oTilesManager.SortTiles(oBlock.Tiles);
		oBlock.IsOpen = true;
		oBlock.Type = Enums.Mentsu.Kantsu;
		oBlock.KanType = Enums.KanType.Daiminkan;
		_Hand.LockedBlocks.Add(oBlock);
		
		oEnemyTileUI.QueueFree();
		
		_Events.EmitSignal(Events.SignalName.DrawKanTileRequested, this, false);
		_RequestFlipKanDora = true;
		_PlayerHand.EnableAllTilesInteractability();
	}
	
	public void OnRonButtonPressed()
	{
		GD.Print("PlayerHandler: OnRonButtonPressed");
		_CallOptionsUI.HideAll();
		GridContainer EnemyDiscardsGroup = (GridContainer) GetTree().GetFirstNodeInGroup("EnemyDiscardsGroup");
		TileUI oEnemyTileUI = (TileUI) EnemyDiscardsGroup.GetChild(EnemyDiscardsGroup.GetChildren().Count - 1);
		
		_Hand.Tiles.Add(oEnemyTileUI._TileModel);
		
		IppatsuCheck();
		
		Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
		Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(_Hand);
		if(oScore != null && oScore.YakuList.Count > 0)
		{
			Win(oScore);
		}
	}
	
	public void OnTsumoButtonPressed()
	{
		GD.Print("PlayerHandler: OnTsumoButtonPressed");
		_CallOptionsUI.HideAll();
		IppatsuCheck();
		
		Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
		Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(_Hand);
		if(oScore != null && oScore.YakuList.Count > 0)
		{
			Win(oScore);
		}
	}
	
	
	public void OnRiichiButtonPressed()
	{
		GD.Print("PlayerHandler: OnRiichiButtonPressed");
		_CallOptionsUI.HideAll();
		IsRiichi = true;
		//TODO: double riichi?
		_Hand.IsRiichi = true;
		_PlayerPoints = _PlayerPoints - 1000;
		_Events.EmitSignal(Events.SignalName.RiichiDeclared);
	}
	
	//TODO: This is for closed kan
	public void OnKanButtonPressed(string psTile, string psKanType)
	{
		GD.Print("PlayerHandler: OnKanButtonPressed");
		_CallOptionsUI.HideAll();
		if(psKanType == "ankan"){
			LockedBlock oClosedKanBlock = (LockedBlock) ClosedKanScene.Instantiate();
			_CalledHand.AddChild(oClosedKanBlock);
			_CalledHand.MoveChild(oClosedKanBlock, 0);
			
			int nCntKanTiles = 0;
			foreach(TileUI oTileUI in _PlayerHand._HandClosed.GetChildren())
			{
				if(oTileUI._TileModel.ToString() == psTile && nCntKanTiles < 4)
				{
					oTileUI.QueueFree();
					nCntKanTiles++;
				}
			}
			foreach(TileUI oTileUI in _PlayerHand._HandTsumo.GetChildren())
			{
				if(oTileUI._TileModel.ToString() == psTile && nCntKanTiles < 4)
				{
					oTileUI.QueueFree();
					nCntKanTiles++;
				}
			}
			
			Mahjong.Model.Tile oTileModel = new Mahjong.Model.Tile(psTile);
			
			oClosedKanBlock.SetUp(oTileModel,oTileModel,oTileModel,oTileModel);
			
			Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
			Mahjong.Model.Block oBlock = new Mahjong.Model.Block();
			oBlock.Tiles.Add(oTileModel);
			oBlock.Tiles.Add(oTileModel);
			oBlock.Tiles.Add(oTileModel);
			oBlock.Tiles.Add(oTileModel);
			oTilesManager.SortTiles(oBlock.Tiles);
			oBlock.IsOpen = false;
			oBlock.Type = Enums.Mentsu.Kantsu;
			oBlock.KanType = Enums.KanType.Ankan;
			_Hand.LockedBlocks.Add(oBlock);
			
			foreach(TileUI oTsumoTile in _PlayerHand._HandTsumo.GetChildren())
			{
				oTsumoTile.Reparent(_PlayerHand._HandClosed);
			}
			_Events.EmitSignal(Events.SignalName.DrawKanTileRequested, this, true);
		}else if(psKanType == "shouminkan"){
			LockedBlock oShouminKanBlock = (LockedBlock) ShouminKanScene.Instantiate();
			
			int index = 0;
			foreach(LockedBlock oBlock in _CalledHand.GetChildren()){
				if(oBlock._TileUI1._Tile.ToString() == psTile){
					oBlock.QueueFree();
					break;
				}else{
					index++;
				}
			}
			_CalledHand.AddChild(oShouminKanBlock);
			_CalledHand.MoveChild(oShouminKanBlock, index);
			
			foreach(TileUI oTileUI in _PlayerHand._HandClosed.GetChildren())
			{
				if(oTileUI._TileModel.ToString() == psTile)
				{
					oTileUI.QueueFree();
					break;
				}
			}
			foreach(TileUI oTileUI in _PlayerHand._HandTsumo.GetChildren())
			{
				if(oTileUI._TileModel.ToString() == psTile)
				{
					oTileUI.QueueFree();
				}
			}
			
			Mahjong.Model.Tile oTileModel = new Mahjong.Model.Tile(psTile);
			
			oShouminKanBlock.SetUp(oTileModel,oTileModel,oTileModel,oTileModel);
			
			Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
			foreach(Mahjong.Model.Block oBlock in _Hand.LockedBlocks){
				if(oBlock.Type == Enums.Mentsu.Koutsu && oBlock.Tiles[0].ToString() == psTile){
					oBlock.Type = Enums.Mentsu.Kantsu;
					oBlock.KanType = Enums.KanType.Shouminkan;
					oBlock.Tiles.Add(oTileModel);
					break;
				}
			}
			
			foreach(TileUI oTsumoTile in _PlayerHand._HandTsumo.GetChildren())
			{
				oTsumoTile.Reparent(_PlayerHand._HandClosed);
			}
			_RequestFlipKanDora = true;
			_Events.EmitSignal(Events.SignalName.DrawKanTileRequested, this, false);
		}
		_PlayerHand.EnableAllTilesInteractability();
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
		_RiichiWaits = new List<Mahjong.Model.Tile>();
		IsRiichi = false;
	}
	
	public int GetShanten()
	{
		Mahjong.CShantenEvaluator oShantenEvaluator = new Mahjong.CShantenEvaluator();
		return oShantenEvaluator.EvaluateShanten(_Hand);
	}
	
	public void PrintHandForDebugging()
	{
		string s = "";
		for(int i = 0;i<_Hand.Tiles.Count;i++)
		{
			s += _Hand.Tiles[i].ToString();
		}
		GD.Print("PlayerHandler PrintHandForDebugging(): " + s);
		for(int i = 0;i<_Hand.LockedBlocks.Count;i++)
		{
			Mahjong.Model.Block oBlock = _Hand.LockedBlocks[i];
			string b = "PlayerHandler PrintHandForDebugging(): LockedBlock " + i + ": ";
			for(int j = 0;j<oBlock.Tiles.Count;j++)
			{
				b += oBlock.Tiles[j].ToString();
			}
			GD.Print(b);
		}
	}
	
	public void DisconnectSignals()
	{
		_Events.TileDiscarded -= OnTileDiscarded;
		_Events.ChiButtonPressed -= OnChiButtonPressed;
		_Events.PonButtonPressed -= OnPonButtonPressed;
		_Events.RonButtonPressed -= OnRonButtonPressed;
		_Events.TsumoButtonPressed -= OnTsumoButtonPressed;
		_Events.RiichiButtonPressed -= OnRiichiButtonPressed;
		_Events.KanButtonPressed -= OnKanButtonPressed;
		_Events.CallOptionsCancelPressed -= OnCallCancelButtonPressed;
		_PlayerHand.DisconnectSignals();
	}
}
