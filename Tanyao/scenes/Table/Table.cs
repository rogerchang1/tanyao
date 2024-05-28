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
	public Mahjong.Model.Tile _DoraIndicator;
	public Mahjong.Model.Tile _DoraTile;
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
		_Events.PlayerTurnEnded += OnPlayerTurnEnded;
		_Events.EnemyTurnEnded += OnEnemyTurnEnded;
		_Events.RoundEnded += OnRoundEnded;
		_Events.PlayerWinDeclared += OnPlayerWinDeclared;
		_Events.RiichiDeclared += OnRiichiDeclared;
		_Events.HandScoreConfirmButtonPressed += OnHandScoreConfirmButtonPressed;
		
		
		InitializeTableValues();
		UpdateDebugInfoLabel();
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
		SetDeadWall();
		_DoraTile = GetDoraFromDoraIndicator(_DoraIndicator);
		_PlayerHandler._DoraTile = _DoraTile;
		_DoraIndicatorLabel.Text = "DoraIndicator: " + _DoraIndicator.ToString() + "\nDora: " + _DoraTile.ToString();
		
		//call tiles debug
		//_TableModel.Wall[0] = new Mahjong.Model.Tile("6s");
		//_TableModel.Wall[1] = new Mahjong.Model.Tile("2p");
		//_TableModel.Wall[2] = new Mahjong.Model.Tile("3p");
		//_TableModel.Wall[3] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[4] = new Mahjong.Model.Tile("6s");
		//_TableModel.Wall[5] = new Mahjong.Model.Tile("2p");
		//_TableModel.Wall[6] = new Mahjong.Model.Tile("3p");
		//_TableModel.Wall[7] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[8] = new Mahjong.Model.Tile("5p");
		//_TableModel.Wall[9] = new Mahjong.Model.Tile("6p");
		//_TableModel.Wall[10] = new Mahjong.Model.Tile("7p");
		//_TableModel.Wall[11] = new Mahjong.Model.Tile("8p");
		//_TableModel.Wall[12] = new Mahjong.Model.Tile("5p");
		//_TableModel.Wall[13] = new Mahjong.Model.Tile("6p");
		//_TableModel.Wall[14] = new Mahjong.Model.Tile("7p");
		//_TableModel.Wall[15] = new Mahjong.Model.Tile("8p");
		//_TableModel.Wall[16] = new Mahjong.Model.Tile("2s");
		//_TableModel.Wall[17] = new Mahjong.Model.Tile("3s");
		//_TableModel.Wall[18] = new Mahjong.Model.Tile("4s");
		//_TableModel.Wall[19] = new Mahjong.Model.Tile("5s");
		//_TableModel.Wall[20] = new Mahjong.Model.Tile("2s");
		//_TableModel.Wall[21] = new Mahjong.Model.Tile("3s");
		//_TableModel.Wall[22] = new Mahjong.Model.Tile("4s");
		//_TableModel.Wall[23] = new Mahjong.Model.Tile("5s");
		//_TableModel.Wall[24] = new Mahjong.Model.Tile("1m");
		//_TableModel.Wall[25] = new Mahjong.Model.Tile("1m");
		//
		//_TableModel.Wall[26] = new Mahjong.Model.Tile("4s");
		//_TableModel.Wall[27] = new Mahjong.Model.Tile("4s");
		//_TableModel.Wall[28] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[29] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[30] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[31] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[32] = new Mahjong.Model.Tile("8p");
		//_TableModel.Wall[33] = new Mahjong.Model.Tile("8p");
		
		////chiitoi debug
		//_TableModel.Wall[0] = new Mahjong.Model.Tile("1s");
		//_TableModel.Wall[1] = new Mahjong.Model.Tile("1s");
		//_TableModel.Wall[2] = new Mahjong.Model.Tile("4s");
		//_TableModel.Wall[3] = new Mahjong.Model.Tile("4s");
		//_TableModel.Wall[4] = new Mahjong.Model.Tile("6s");
		//_TableModel.Wall[5] = new Mahjong.Model.Tile("2p");
		//_TableModel.Wall[6] = new Mahjong.Model.Tile("3p");
		//_TableModel.Wall[7] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[8] = new Mahjong.Model.Tile("8s");
		//_TableModel.Wall[9] = new Mahjong.Model.Tile("8s");
		//_TableModel.Wall[10] = new Mahjong.Model.Tile("9s");
		//_TableModel.Wall[11] = new Mahjong.Model.Tile("9s");
		//_TableModel.Wall[12] = new Mahjong.Model.Tile("5p");
		//_TableModel.Wall[13] = new Mahjong.Model.Tile("6p");
		//_TableModel.Wall[14] = new Mahjong.Model.Tile("7p");
		//_TableModel.Wall[15] = new Mahjong.Model.Tile("8p");
		//_TableModel.Wall[16] = new Mahjong.Model.Tile("1z");
		//_TableModel.Wall[17] = new Mahjong.Model.Tile("1z");
		//_TableModel.Wall[18] = new Mahjong.Model.Tile("2z");
		//_TableModel.Wall[19] = new Mahjong.Model.Tile("2z");
		//_TableModel.Wall[20] = new Mahjong.Model.Tile("2s");
		//_TableModel.Wall[21] = new Mahjong.Model.Tile("3s");
		//_TableModel.Wall[22] = new Mahjong.Model.Tile("4s");
		//_TableModel.Wall[23] = new Mahjong.Model.Tile("5s");
		//_TableModel.Wall[24] = new Mahjong.Model.Tile("2s");
		//_TableModel.Wall[25] = new Mahjong.Model.Tile("2s");
		//
		//_TableModel.Wall[26] = new Mahjong.Model.Tile("3s");
		//_TableModel.Wall[27] = new Mahjong.Model.Tile("3s");
		//_TableModel.Wall[28] = new Mahjong.Model.Tile("2s");
		//_TableModel.Wall[29] = new Mahjong.Model.Tile("2s");
		//_TableModel.Wall[30] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[31] = new Mahjong.Model.Tile("4p");
		//_TableModel.Wall[32] = new Mahjong.Model.Tile("8p");
		//_TableModel.Wall[33] = new Mahjong.Model.Tile("8p");
		
		bool bDealToPlayerFirst = true;
		if(_PlayerHandler._SeatWind != Enums.Wind.East)
		{
			bDealToPlayerFirst = false;
		}
		InitializeHands(bDealToPlayerFirst);
		UpdateDebugInfoLabel();
		UpdateTilesLeftLabel();
		StartRound();
	}
	
	public void CleanUp()
	{
		_PlayerHandler.CleanUp();
		_EnemyHandler.CleanUp();
		_DeadWall.CleanUp();
	}
	
	public void SetDeadWall()
	{
		Mahjong.Model.Tile oKanDora1 = _TableModel.Wall[_TableModel.Wall.Count - 8];
		Mahjong.Model.Tile oKanDora2 = _TableModel.Wall[_TableModel.Wall.Count - 10];
		Mahjong.Model.Tile oKanDora3 = _TableModel.Wall[_TableModel.Wall.Count - 12];
		Mahjong.Model.Tile oKanDora4 = _TableModel.Wall[_TableModel.Wall.Count - 14];
		_DoraIndicator = _TableModel.Wall[_TableModel.Wall.Count - 6];
		_DeadWall.InitializeDeadWallUI(_DoraIndicator, oKanDora1, oKanDora2, oKanDora3, oKanDora4);
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
		_PlayerHandler._RoundWind = _RoundWind;
		_PlayerHandler._DoraTile = _DoraTile;
		_EnemyHandler.SortTilesUI();
	}
	
	public Mahjong.Model.Tile GetDoraFromDoraIndicator(Mahjong.Model.Tile poDoraIndicator)
	{
		int num = 0;
		string suit = "";
		suit = poDoraIndicator.suit;
		if(suit == "z")
		{
			//TODO: Becareful of the wall configurations
			if(poDoraIndicator.num != 4 && poDoraIndicator.num != 7)
			{
				num = poDoraIndicator.num + 1;
			}else if(poDoraIndicator.num == 4){
				num =  1;
			}else if(poDoraIndicator.num == 7){
				num =  5;
			}
		}else{
			if(poDoraIndicator.num == 9){
				num =  1;
			}else{
				num = poDoraIndicator.num + 1;
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
		
		HandScore oHandScore = (HandScore) _HandScoreScene.Instantiate();
		AddChild(oHandScore);
		oHandScore._Score = poScore._Score;
		oHandScore._Hand = poHand._Hand;
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
		UpdateDebugInfoLabel();
	}
	
	private void UpdateTilesLeftLabel()
	{
		int count = Math.Min((_TileDrawLimit - _TileDrawCounter),(_TableModel.Wall.Count - 14)) ;
		_TilesLeftLabel.Text = "Tiles Left: " + count;
	}
	
	private void DisconnectSignals()
	{
		_Events.DrawTileRequested -= OnDrawTileRequested;
		_Events.PlayerTurnEnded -= OnPlayerTurnEnded;
		_Events.EnemyTurnEnded -= OnEnemyTurnEnded;
		_Events.RoundEnded -= OnRoundEnded;
		_Events.PlayerWinDeclared -= OnPlayerWinDeclared;
		_Events.RiichiDeclared -= OnRiichiDeclared;
		_Events.HandScoreConfirmButtonPressed -= OnHandScoreConfirmButtonPressed;
		_PlayerHandler.DisconnectSignals();
		_EnemyHandler.DisconnectSignals();
	}
}
