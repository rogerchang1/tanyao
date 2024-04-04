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
	
	Events _Events;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_TableModel = new Mahjong.Model.Table();
		_TableManager = new Mahjong.CTableManager();
		
		_PlayerHand = GetNode<Hand>("TableUI/Hand");
		_PlayerHandler = GetNode<PlayerHandler>("PlayerHandler");
		_EnemyHandler = GetNode<EnemyHandler>("EnemyHandler");
		_TilesLeftLabel = GetNode<Label>("TilesLeftLabel");
		
		_Events = GetNode<Events>("/root/Events");
		_Events.DrawTileRequested += OnDrawTileRequested;
		_Events.PlayerTurnEnded += OnPlayerTurnEnded;
		_Events.EnemyTurnEnded += OnEnemyTurnEnded;
		
		InitializeTable();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void InitializeTable()
	{
		_TableManager.InitializeTable(_TableModel);
		//TODO: randomize who draws first
		InitializeHands(true);
		UpdateTilesLeftLabel();
		StartRound();
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
		}
		if(pbShouldAddTilesToPlayerFirst)
		{
			_PlayerHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
			_EnemyHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
		}else{
			_EnemyHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
			_PlayerHandler.AddTileToHandClosed(_TableManager.DrawNextTileFromWall(_TableModel));
		}
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
			((PlayerHandler)oBaseHandler).AddTileToHandTsumo(DrawnTile);
			
		}
		if(oBaseHandler.GetType() == typeof(EnemyHandler))
		{
			//TODO: change this later
			((EnemyHandler)oBaseHandler).AddTileToHandTsumo(DrawnTile);
		}
		UpdateTilesLeftLabel();
	}
	
	public void OnPlayerTurnStarted()
	{
		_PlayerHandler.StartTurn();
	}
	
	public void OnEnemyTurnStarted()
	{
		_EnemyHandler.StartTurn();
	}
	
	//TODO: Switch to EnemyHandler Turn when you implement EnemyHandler
	//TODO: Remove async when you don't need it anymore
	public async void OnPlayerTurnEnded()
	{
		await ToSignal(GetTree().CreateTimer(.5), "timeout");
		_EnemyHandler.StartTurn();
	}
	
	//TODO: Switch to EnemyHandler Turn when you implement EnemyHandler
	//TODO: Remove async when you don't need it anymore
	public async void OnEnemyTurnEnded()
	{
		await ToSignal(GetTree().CreateTimer(.5), "timeout");
		_PlayerHandler.StartTurn();
	}
	
	private void UpdateTilesLeftLabel()
	{
		 _TilesLeftLabel.Text = "Tiles Left: " + (_TableModel.Wall.Count - 14);
	}
}
