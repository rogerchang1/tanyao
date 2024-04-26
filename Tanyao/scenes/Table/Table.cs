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
	public Label _TilesLeftLabel;
	public Label _EnemyPointsLabel;
	public Label _PlayerPointsLabel;
	public Label _PotLabel;
	public Label _HonbaLabel;
	public Label _DoraIndicatorLabel;
	public Label _RoundWindLabel;
	public Label _SeatWindLabel;
	
	public int _PlayerPoints;
	public int _EnemyPoints;
	public int _Pot;
	public int _Honba;
	
	//TODO: Think about additional dora tiles when kan 
	public Mahjong.Model.Tile _DoraIndicator;
	public Mahjong.Model.Tile _DoraTile;
	public Enums.Wind _RoundWind;
	
	Events _Events;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_TableModel = new Mahjong.Model.Table();
		_TableManager = new Mahjong.CTableManager();
		
		_PlayerHand = GetNode<Hand>("TableUI/Hand");
		_PlayerHandler = GetNode<PlayerHandler>("PlayerHandler");
		_EnemyHandler = GetNode<EnemyHandler>("EnemyHandler");
		_TilesLeftLabel = GetNode<Label>("DebugInfoContainer/TilesLeftLabel");
		_EnemyPointsLabel = GetNode<Label>("DebugInfoContainer/EnemyPointsLabel");
		_PlayerPointsLabel = GetNode<Label>("DebugInfoContainer/PlayerPointsLabel");
		_PotLabel = GetNode<Label>("DebugInfoContainer/PotLabel");
		_HonbaLabel = GetNode<Label>("DebugInfoContainer/HonbaLabel");
		_DoraIndicatorLabel = GetNode<Label>("DebugInfoContainer/DoraIndicatorLabel");
		_RoundWindLabel = GetNode<Label>("DebugInfoContainer/RoundWindLabel");
		_SeatWindLabel = GetNode<Label>("DebugInfoContainer/SeatWindLabel");
		
		_Events = GetNode<Events>("/root/Events");
		_Events.DrawTileRequested += OnDrawTileRequested;
		_Events.PlayerTurnEnded += OnPlayerTurnEnded;
		_Events.EnemyTurnEnded += OnEnemyTurnEnded;
		_Events.RoundEnded += OnRoundEnded;
		_Events.PlayerWinDeclared += OnPlayerWinDeclared;
		_Events.RiichiDeclared += OnRiichiDeclared;
		
		_PlayerHandler._PlayerPoints = 25000;
		_EnemyPoints = 25000;
		_Honba = 0;
		_EnemyPointsLabel.Text = "EnemyPoints: " + _EnemyPoints.ToString();
		_PlayerPointsLabel.Text = "PlayerPoints: " + _PlayerHandler._PlayerPoints.ToString();
		_PotLabel.Text = "Pot: " + _Pot.ToString();
		_HonbaLabel.Text = "Honba: " + _Honba.ToString();
		InitializeTable();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void InitializeTable()
	{
		//_TableManager.InitializeTable(_TableModel);
		Mahjong.Model.WallConfiguration oWallConfig = new Mahjong.Model.WallConfiguration();
		oWallConfig.LoadManzu = false;
		oWallConfig.LoadWest = false;
		oWallConfig.LoadNorth = false;
		_TableManager.InitializeTableWithWallConfiguration(_TableModel,oWallConfig);
		_DoraIndicator = _TableModel.Wall[_TableModel.Wall.Count - 5];
		SetDora();
		_DoraIndicatorLabel.Text = "DoraIndicator: " + _DoraIndicator.ToString() + "\nDora: " + _DoraTile.ToString();
		_RoundWind = Enums.Wind.East;
		_PlayerHandler._SeatWind = Enums.Wind.East;
		_PlayerHandler._DoraTile = _DoraTile;
		_RoundWindLabel.Text = "RoundWind: " + _RoundWind.ToString();
		_SeatWindLabel.Text = "SeatWind: " + _PlayerHandler._SeatWind.ToString();
		
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
		
		
		//TODO: randomize who draws first
		_PlayerHandler._SeatWind = Enums.Wind.East;
		_EnemyHandler._SeatWind = Enums.Wind.South;
		InitializeHands(true);
		UpdateTilesLeftLabel();
		StartRound();
	}
	
	public void CleanUp()
	{
		_PlayerHandler.CleanUp();
		_EnemyHandler.CleanUp();
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
	
	public void SetDora()
	{
		int num = 0;
		string suit = "";
		suit = _DoraIndicator.suit;
		if(suit == "z")
		{
			//TODO: Becareful of the wall configurations
			if(_DoraIndicator.num != 4 && _DoraIndicator.num != 7)
			{
				num = _DoraIndicator.num + 1;
			}else if(_DoraIndicator.num == 4){
				num =  1;
			}else if(_DoraIndicator.num == 7){
				num =  5;
			}
		}else{
			if(_DoraIndicator.num == 9){
				num =  1;
			}else{
				num = _DoraIndicator.num + 1;
			}
		}
		_DoraTile = new Mahjong.Model.Tile(num+suit);
	}
	
	public void StartRound()
	{
		//TODO: Later when there's an enemy handler
		//Just do a rand to flip a coin on who starts
		
		_PlayerHandler.StartTurn();
	}
	
	public void OnDrawTileRequested(BaseHandler oBaseHandler)
	{
		if(!_TableManager.CanDrawFromWall(_TableModel))
		{
			return;
		}
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
		UpdateTilesLeftLabel();
	}
	
	public void OnPlayerTurnStarted(string psTile)
	{
		_PlayerHandler.StartTurn(psTile);
	}
	
	public void OnEnemyTurnStarted(string psTile)
	{
		_EnemyHandler.StartTurn(psTile);
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
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		InitializeTable();
	}
	
	public void OnPlayerWinDeclared(int pnPayment)
	{
		_EnemyPoints -= pnPayment;
		_PlayerHandler._PlayerPoints += (pnPayment + _Pot);
		_Pot = 0;
		
		if(_PlayerHandler._SeatWind == Enums.Wind.East)
		{
			_Honba += 1;
		}else{
			_Honba = 0;
		}
		
		_EnemyPointsLabel.Text = "EnemyPoints: " + _EnemyPoints.ToString();
		_PlayerPointsLabel.Text = "PlayerPoints: " + _PlayerHandler._PlayerPoints.ToString();
		_PotLabel.Text = "Pot: " + _Pot.ToString();
		_HonbaLabel.Text = "Honba: " + _Honba.ToString();
	}
	
	public void OnRiichiDeclared()
	{
		_Pot += 1000;
		_EnemyPointsLabel.Text = "EnemyPoints: " + _EnemyPoints.ToString();
		_PlayerPointsLabel.Text = "PlayerPoints: " + _PlayerHandler._PlayerPoints.ToString();
		_PotLabel.Text = "Pot: " + _Pot.ToString();
	}
	
	private void UpdateTilesLeftLabel()
	{
		 _TilesLeftLabel.Text = "Tiles Left: " + (_TableModel.Wall.Count - 14);
	}
}
