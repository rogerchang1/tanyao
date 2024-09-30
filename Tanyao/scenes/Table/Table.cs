using Godot;
using System;
using Mahjong;
using Mahjong.Model;

public partial class Table : Godot.Node2D
{	
	Mahjong.Model.Table _TableModel;
	Mahjong.CTableManager _TableManager;
	
	public Hand _PlayerHand;
	public PlayerHandler _PlayerHandler;
	public EnemyHandler _EnemyHandler;
	
	public DeadWall _DeadWall;
	public HonbaUI _HonbaUI;
	public PlayerPortrait _PlayerPortrait;
	
	public Label _TilesLeftLabel;
	public Label _EnemyPointsLabel;
	public Label _PlayerPointsLabel;
	public Label _PotLabel;
	public Label _HonbaLabel;
	public Label _DoraIndicatorLabel;
	public Label _RoundWindLabel;
	public Label _SeatWindLabel;
	public Label _RoundNumberLabel;
	
	public int _PlayerPoints;
	public int _EnemyPoints;
	public int _Pot;
	public int _Honba;
	//For 2 players, round 1, 2 means East. Round 3, 4 means South.
	public int _RoundNumber;
	
	public int _TileDrawLimit;
	public int _TileDrawCounter;
	
	//TODO: Think about additional dora tiles when kan 
	public Mahjong.Model.Tile[] _DoraIndicatorArr;
	public Mahjong.Model.Tile[] _UraDoraIndicatorArr;
	public Mahjong.Model.Tile[] _DoraTileArr;
	public Mahjong.Model.Tile[] _UraDoraTileArr;
	public int _NumKanDoraActive;
	public Enums.Wind _RoundWind;
	
	[Export]
	public PackedScene _HandScoreScene;
	
	Events _Events;
	GameController _GC;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_TableModel = new Mahjong.Model.Table();
		_TableManager = new Mahjong.CTableManager();
		
		_PlayerHand = GetNode<Hand>("TableUI/Hand");
		_PlayerHandler = GetNode<PlayerHandler>("PlayerHandler");
		_EnemyHandler = GetNode<EnemyHandler>("EnemyHandler");
		
		_DeadWall = GetNode<DeadWall>("TableUI/DeadWall");
		_HonbaUI = GetNode<HonbaUI>("TableUI/HonbaUI");
		_PlayerPortrait = GetNode<PlayerPortrait>("PlayerPortrait");
		
		_TilesLeftLabel = GetNode<Label>("DebugInfoContainer/TilesLeftLabel");
		_EnemyPointsLabel = GetNode<Label>("DebugInfoContainer/EnemyPointsLabel");
		_PlayerPointsLabel = GetNode<Label>("DebugInfoContainer/PlayerPointsLabel");
		_PotLabel = GetNode<Label>("DebugInfoContainer/PotLabel");
		_HonbaLabel = GetNode<Label>("DebugInfoContainer/HonbaLabel");
		_DoraIndicatorLabel = GetNode<Label>("DebugInfoContainer/DoraIndicatorLabel");
		_RoundWindLabel = GetNode<Label>("DebugInfoContainer/RoundWindLabel");
		_SeatWindLabel = GetNode<Label>("DebugInfoContainer/SeatWindLabel");
		_RoundNumberLabel = GetNode<Label>("DebugInfoContainer/RoundNumberLabel");
		
		_Events = GetNode<Events>("/root/Events");
		_GC = GetNode<GameController>("/root/GameController");
		_Events.DrawTileRequested += OnDrawTileRequested;
		_Events.DrawKanTileRequested += OnDrawKanTileRequested;
		_Events.PlayerTurnEnded += OnPlayerTurnEnded;
		_Events.EnemyTurnEnded += OnEnemyTurnEnded;
		_Events.RoundEnded += OnRoundEnded;
		_Events.PlayerWinDeclared += OnPlayerWinDeclared;
		_Events.RiichiDeclared += OnRiichiDeclared;
		_Events.FlipKanDoraDeclared += OnFlipKanDoraDeclared;
		_Events.HandScoreConfirmButtonPressed += OnHandScoreConfirmButtonPressed;
		
		InitializeTableValues();
		UpdateDebugInfoLabel();
		UpdateHonbaUI();
		InitializeTable();
	}
	
	public void InitializeTableValues()
	{
		_PlayerHandler._PlayerPoints = 25000;
		_EnemyPoints = 25000;
		_RoundWind = Enums.Wind.East;
		_RoundNumber = 1;
		_Honba = 0;
		_TileDrawLimit = 36;
		//TODO: randomize who draws first
		Random rnd = new Random();
		int coinToss   = rnd.Next(2);
		if(coinToss == 0)
		{
			_PlayerHandler._SeatWind = Enums.Wind.East;
			_EnemyHandler._SeatWind = Enums.Wind.South;
		}else{
			_PlayerHandler._SeatWind = Enums.Wind.South;
			_EnemyHandler._SeatWind = Enums.Wind.East;
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void InitializeTable()
	{
		Mahjong.Model.WallConfiguration oWallConfig = new Mahjong.Model.WallConfiguration();
		oWallConfig.LoadManzu = false;
		oWallConfig.LoadWest = false;
		oWallConfig.LoadNorth = false;
		////oWallConfig.LoadEast = false;
		////oWallConfig.LoadSouth = false;
		//oWallConfig.LoadDragons = false;
		//oWallConfig.LoadPinzu = false;
		_TableManager.InitializeTableWithWallConfiguration(_TableModel,oWallConfig);
		_TileDrawCounter = 0;
		SetDeadWall(); //Sets _DoraIndicatorArr and _UraDoraIndicatorArr
		SetDoraArrFromDoraIndicatorArr();
		SetUraDoraArrFromUraDoraIndicatorArr();
		_NumKanDoraActive = 0;
		_PlayerHandler._DoraTileArr = _DoraTileArr;
		_PlayerHandler._UraDoraTileArr = _UraDoraTileArr;
		_PlayerHandler._NumKanDoraActive = 0;
		_EnemyHandler._NumKanDoraActive = 0;
		_PlayerHandler._RiichiTileCounter = -1;
		_PlayerHandler._WallTileCounter = _TileDrawCounter;
		_EnemyHandler._RiichiTileCounter = -1;
		_EnemyHandler._WallTileCounter = _TileDrawCounter;
		_PlayerHandler._RoundWind = _RoundWind;
		_DoraIndicatorLabel.Text = "DoraIndicator: " + _DoraIndicatorArr[0].ToString() + "\nDora: " + _DoraTileArr[0].ToString();
		
		//_PlayerHandler._BypassFuriten = true;
		TableWallModder oTableWallModder = new TableWallModder();
		//Furiten testing
		oTableWallModder.ModTable("23456788s34567p","23456788s34567p","8888p",_TableModel);
		//Kan testing
		//oTableWallModder.ModTable("6666z","6666z","66z",_TableModel);
		//call tiles debug
		//oTableWallModder.ModTable("23456s2345678p1m","23456s2345678p1m","44s444488p",_TableModel);
		//chiitoi debug
		//oTableWallModder.ModTable("11448899s1122z2s","11448899s1122z2s","2233s4488p",_TableModel);
		//Kan Riichi debug (Should not be allowed to kan due to waits will change)
		//oTableWallModder.ModTable("234567p5556789s","234567p5556789s","11z556699s",_TableModel);
		//Kan Riichi debug (Should be allowed to kan due to waits will not change)
		//oTableWallModder.ModTable("11144567p55567s","11144567p55567s","11z556699s",_TableModel);
		//two kans and riichi
		//oTableWallModder.ModTable("11144567p55567s","11144567p55567s","11p556699s",_TableModel);
		//chi shouldn't allow immediate kan
		//oTableWallModder.ModTable("11114567p55567s","11114567p55567s","88p556699s",_TableModel);
		
		bool bDealToPlayerFirst = true;
		if(_PlayerHandler._SeatWind != Enums.Wind.East)
		{
			bDealToPlayerFirst = false;
		}
		InitializeHands(bDealToPlayerFirst);
		UpdateDebugInfoLabel();
		UpdateHonbaUI();
		UpdateTilesLeftLabel();
		StartRound();
	}
	
	public void CleanUp()
	{
		_PlayerHandler.CleanUp();
		_EnemyHandler.CleanUp();
		_DeadWall.CleanUp();
	}
	
	//Will set _DoraIndicatorArr & _UraDoraIndicatorArr
	public void SetDeadWall()
	{
		_DoraIndicatorArr = new Mahjong.Model.Tile[5];
		_UraDoraIndicatorArr = new Mahjong.Model.Tile[5];
		_UraDoraIndicatorArr[0] = _TableModel.Wall[_TableModel.Wall.Count - 5];
		_DoraIndicatorArr[0] = _TableModel.Wall[_TableModel.Wall.Count - 6];
		_UraDoraIndicatorArr[1] = _TableModel.Wall[_TableModel.Wall.Count - 7];
		_DoraIndicatorArr[1] = _TableModel.Wall[_TableModel.Wall.Count - 8];
		_UraDoraIndicatorArr[2] = _TableModel.Wall[_TableModel.Wall.Count - 9];
		_DoraIndicatorArr[2] = _TableModel.Wall[_TableModel.Wall.Count - 10];
		_UraDoraIndicatorArr[3] = _TableModel.Wall[_TableModel.Wall.Count - 11];
		_DoraIndicatorArr[3] = _TableModel.Wall[_TableModel.Wall.Count - 12];
		_UraDoraIndicatorArr[4] = _TableModel.Wall[_TableModel.Wall.Count - 13];
		_DoraIndicatorArr[4] = _TableModel.Wall[_TableModel.Wall.Count - 14];
		
		_DeadWall.InitializeDeadWallUI(_DoraIndicatorArr);
	}
	
	public void InitializeHands(bool pbShouldAddTilesToPlayerFirst)
	{
		for(int i = 0; i< 6; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				if(pbShouldAddTilesToPlayerFirst)
				{
					_PlayerHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
					
				}else
				{
					_EnemyHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
				}
			}
			pbShouldAddTilesToPlayerFirst = !pbShouldAddTilesToPlayerFirst;
			//await ToSignal(GetTree().CreateTimer(.25), "timeout");
		}
		if(pbShouldAddTilesToPlayerFirst)
		{
			_PlayerHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
			_EnemyHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
		}else{
			_EnemyHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
			_PlayerHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
		}
		_PlayerHandler.SortTilesUI();
		_EnemyHandler.SortTilesUI();
	}
	
	public void SetDoraArrFromDoraIndicatorArr()
	{
		_DoraTileArr = new Mahjong.Model.Tile[5];
		for(int i = 0;i<_DoraIndicatorArr.Length;i++)
		{
			Mahjong.Model.Tile poDoraIndicator = _DoraIndicatorArr[i];
			_DoraTileArr[i] = GetNextTileFromTileIndicator(poDoraIndicator);
		}
	}
	
	public void SetUraDoraArrFromUraDoraIndicatorArr()
	{
		_UraDoraTileArr = new Mahjong.Model.Tile[5];
		for(int i = 0;i<_UraDoraIndicatorArr.Length;i++)
		{
			Mahjong.Model.Tile poUraDoraIndicator = _UraDoraIndicatorArr[i];
			_UraDoraTileArr[i] = GetNextTileFromTileIndicator(poUraDoraIndicator);
		}
	}
	
	private Mahjong.Model.Tile GetNextTileFromTileIndicator(Mahjong.Model.Tile poIndicator)
	{
		int num = 0;
		string suit = "";
		suit = poIndicator.suit;
		if(suit == "z")
		{
			//TODO: Becareful of the wall configurations
			if(poIndicator.num != 4 && poIndicator.num != 7)
			{
				num = poIndicator.num + 1;
			}else if(poIndicator.num == 4){
				num =  1;
			}else if(poIndicator.num == 7){
				num =  5;
			}
		}else{
			if(poIndicator.num == 9){
				num =  1;
			}else{
				num = poIndicator.num + 1;
			}
		}
		return new Mahjong.Model.Tile(num+suit);
	}
	
	public void StartRound()
	{
		_PlayerHandler.SortTilesUI();
		_EnemyHandler.SortTilesUI();
		if(_PlayerHandler._SeatWind == Enums.Wind.East)
		{
			_PlayerHandler.StartTurn();
		}
		else{
			_EnemyHandler.StartTurn();
		}	
	}
	
	public void OnDrawTileRequested(BaseHandler oBaseHandler)
	{
		if(!_TableManager.CanDrawFromWall(_TableModel) || _TileDrawCounter >= _TileDrawLimit)
		{
			Ryuukyoku();
			return;
		}else{
			Mahjong.Model.Tile DrawnTile = _TableManager.DrawNextTileFromWall(_TableModel);
			_TileDrawCounter++;
			_PlayerHandler._WallTileCounter = _TileDrawCounter;
			_EnemyHandler._WallTileCounter = _TileDrawCounter;
			if(oBaseHandler.GetType() == typeof(PlayerHandler))
			{
				((PlayerHandler) oBaseHandler).AddTileToHandTsumo(DrawnTile);
				
			}
			if(oBaseHandler.GetType() == typeof(EnemyHandler))
			{
				//TODO: change this later
				((EnemyHandler) oBaseHandler).AddTileToHandTsumo(DrawnTile);
			}
			UpdateTilesLeftLabel();
		}
	}
	
	//TODO: needs checks if the kan will abort the game
	//Currently this is for closed kan only.
	public void OnDrawKanTileRequested(BaseHandler oBaseHandler, bool pbFlipKandora)
	{
		if(!_TableManager.CanDrawFromWall(_TableModel) || _TileDrawCounter >= _TileDrawLimit)
		{
			return;
		}else{
			
			Mahjong.Model.Tile DrawnTile = _TableModel.Wall[_TableModel.Wall.Count - _NumKanDoraActive - 1];
			if(oBaseHandler.GetType() == typeof(PlayerHandler))
			{
				((PlayerHandler) oBaseHandler).AddTileToHandTsumo(DrawnTile);
				
			}
			if(oBaseHandler.GetType() == typeof(EnemyHandler))
			{
				//TODO: change this later
				((EnemyHandler) oBaseHandler).AddTileToHandTsumo(DrawnTile);
			}
			_TileDrawCounter++;
			if(pbFlipKandora){
				FlipKanDora();
			}
			UpdateTilesLeftLabel();
		}
	}
	
	//Remove async if you don't need the timer
	public async void Ryuukyoku()
	{
		int nPlayerShanten = _PlayerHandler.GetShanten();
		int nEnemyShanten = _EnemyHandler.GetShanten();
		if(nPlayerShanten == 0 && nEnemyShanten != 0)
		{
			_EnemyPoints -= 1000;
			_PlayerHandler._PlayerPoints += 1000;
		}else if(nPlayerShanten != 0 && nEnemyShanten == 0)
		{
			_EnemyPoints += 1000;
			_PlayerHandler._PlayerPoints -= 1000;
		}
		
		if((_PlayerHandler._SeatWind == Enums.Wind.East && nPlayerShanten != 0)
		|| (_EnemyHandler._SeatWind == Enums.Wind.East && nEnemyShanten != 0))
		{
			SwitchWinds();
		}
		_Honba += 1;
		UpdateDebugInfoLabel();
		UpdateHonbaUI();
		OnRoundEnded();
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		InitializeTable();
	}
	
	public void SwitchWinds()
	{
		if(_RoundWind == Enums.Wind.East && _RoundNumber == 2)
		{
			_RoundWind = Enums.Wind.South;
		}
		if(_PlayerHandler._SeatWind == Enums.Wind.East)
		{
			_PlayerHandler._SeatWind = Enums.Wind.South;
			_EnemyHandler._SeatWind = Enums.Wind.East;
		}else{
			_PlayerHandler._SeatWind = Enums.Wind.East;
			_EnemyHandler._SeatWind = Enums.Wind.South;
		}
		_RoundNumber++;
	}
	
	//TODO: Switch to EnemyHandler Turn when you implement EnemyHandler
	//TODO: Remove async when you don't need it anymore
	public async void OnPlayerTurnEnded(string psTile)
	{
		await ToSignal(GetTree().CreateTimer(.5), "timeout");
		_EnemyHandler.StartTurn(psTile);
	}
	
	//TODO: Switch to EnemyHandler Turn when you implement EnemyHandler
	//TODO: Remove async when you don't need it anymore
	public async void OnEnemyTurnEnded(string psTile)
	{
		await ToSignal(GetTree().CreateTimer(.5), "timeout");
		_PlayerHandler.StartTurn(psTile);
	}
	
	public async void OnRoundEnded()
	{
		CleanUp();
		if(_RoundNumber > 4)
		{
			EndGame oEndGame = (EndGame) ResourceLoader.Load<PackedScene>("res://Tanyao/scenes/EndGame.tscn").Instantiate();
			GetTree().Root.AddChild(oEndGame);
			oEndGame.SetEndScoresLabel(_PlayerHandler._PlayerPoints.ToString(), _EnemyPoints.ToString());
			DisconnectSignals();
			GetNode("/root/Table").QueueFree();
		}
		//Move this to OnHandScoreConfirmButtonPressed
		//await ToSignal(GetTree().CreateTimer(2), "timeout");
		//InitializeTable();
	}
	
	public void OnPlayerWinDeclared(int pnPayment, HandGodotWrapper poHand, ScoreGodotWrapper poScore)
	{
		CleanUp();
		_EnemyPoints -= pnPayment;
		_PlayerHandler._PlayerPoints += (pnPayment + _Pot);
		_Pot = 0;
		
		if(_PlayerHandler._SeatWind == Enums.Wind.East)
		{
			_Honba += 1;
		}else{
			SwitchWinds();
			_Honba = 0;
		}
		
		UpdateDebugInfoLabel();
		UpdateHonbaUI();
		_HonbaUI.Hide();
		_PlayerPortrait.Hide();
		
		//TODO: I had to adjust the size of the Hand HBoxContainer and placement of HandScore confirm button
		//because they were overlapping with each other. The Hand HBoxContainer was eating the mouse click input
		//so parts of the Confirm button area cannot be highlighted on hover and cannot be clicked.
		//I have no idea how to adjust input event handling order such that Confirm button takes precedence for mouse clicks
		HandScore oHandScore = (HandScore) _HandScoreScene.Instantiate();
		AddChild(oHandScore);
		oHandScore._Score = poScore._Score;
		oHandScore._Hand = poHand._Hand;
		oHandScore._DoraIndicatorArr = _DoraIndicatorArr;
		oHandScore._UraDoraIndicatorArr = _UraDoraIndicatorArr;
		oHandScore._NumKanDoraActive = _NumKanDoraActive;
		oHandScore.SetLabels();
	}
	
	public void UpdateDebugInfoLabel()
	{
		_EnemyPointsLabel.Text = "EnemyPoints: " + _EnemyPoints.ToString();
		_PlayerPointsLabel.Text = "PlayerPoints: " + _PlayerHandler._PlayerPoints.ToString();
		_PotLabel.Text = "Pot: " + _Pot.ToString();
		_HonbaLabel.Text = "Honba: " + _Honba.ToString();
		_RoundWindLabel.Text = "RoundWind: " + _RoundWind.ToString();
		_SeatWindLabel.Text = "SeatWind: " + _PlayerHandler._SeatWind.ToString();
		_RoundNumberLabel.Text = "RoundNumber: " + _RoundNumber.ToString();
	}
	
	public void OnHandScoreConfirmButtonPressed()
	{
		HandScore oHandScore = GetNode<HandScore>("HandScore");
		oHandScore.QueueFree();
		
		_HonbaUI.Show();
		_PlayerPortrait.Show();
		
		if(_RoundNumber > 4)
		{
			EndGame oEndGame = (EndGame) ResourceLoader.Load<PackedScene>("res://Tanyao/scenes/EndGame.tscn").Instantiate();
			GetTree().Root.AddChild(oEndGame);
			oEndGame.SetEndScoresLabel(_PlayerHandler._PlayerPoints.ToString(), _EnemyPoints.ToString());
			DisconnectSignals();
			GetNode("/root/Table").QueueFree();
		}else{
			//Begin new round
			InitializeTable();
		}
	}
	
	public void OnRiichiDeclared()
	{
		_Pot += 1000;
		_PlayerHandler._RiichiTileCounter = _TileDrawCounter;
		UpdateDebugInfoLabel();
		UpdateHonbaUI();
	}
	
	public void OnFlipKanDoraDeclared()
	{
		FlipKanDora();
	}
	
	public void FlipKanDora(){
		_DeadWall.FlipKanDora(_NumKanDoraActive + 1);
		_NumKanDoraActive++;
		_PlayerHandler._NumKanDoraActive++;
		_EnemyHandler._NumKanDoraActive++;
	}
	
	private void UpdateTilesLeftLabel()
	{
		int count = Math.Min((_TileDrawLimit - _TileDrawCounter),(_TableModel.Wall.Count - 14)) ;
		_TilesLeftLabel.Text = "Tiles Left: " + count;
	}
	
	private void DisconnectSignals()
	{
		_Events.DrawTileRequested -= OnDrawTileRequested;
		_Events.DrawKanTileRequested -= OnDrawKanTileRequested;
		_Events.PlayerTurnEnded -= OnPlayerTurnEnded;
		_Events.EnemyTurnEnded -= OnEnemyTurnEnded;
		_Events.RoundEnded -= OnRoundEnded;
		_Events.PlayerWinDeclared -= OnPlayerWinDeclared;
		_Events.RiichiDeclared -= OnRiichiDeclared;
		_Events.HandScoreConfirmButtonPressed -= OnHandScoreConfirmButtonPressed;
		_PlayerHandler.DisconnectSignals();
		_EnemyHandler.DisconnectSignals();
	}
	
	private void UpdateHonbaUI()
	{
		_HonbaUI.SetHonbaLabelText(_Honba);
		_HonbaUI.SetRiichiLabelText(_Pot/1000);
	}
}
