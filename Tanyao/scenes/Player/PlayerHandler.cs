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
	//[Export]
	//public PackedScene PonScene;
	//[Export]
	//public PackedScene KanScene;
	//[Export]
	//public PackedScene OpenKanScene;
	
	//Events _Events;
	
	Mahjong.Model.Hand _Hand;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		//_Events.PlayerTileDiscarded += OnPlayerTileDiscarded;
		_Events.TileDiscarded += OnTileDiscarded;
		_Events.ChiButtonPressed += OnChiButtonPressed;
		_Events.CallOptionsCancelPressed += OnCallCancelButtonPressed;
		_Hand = new Mahjong.Model.Hand();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void StartTurn(string psDiscardedTile = "")
	{
		if(psDiscardedTile != "")
		{
			GD.Print("Enemy discarded " + psDiscardedTile);
			List<List<Mahjong.Model.Tile>> oChiAbleTiles = IsChi(new Mahjong.Model.Tile(psDiscardedTile));
			if(oChiAbleTiles.Count > 0)
			{
				_CallOptionsUI.Show();
				_CallOptionsUI._Chi.Show();
				_CallOptionsUI.SetChiTileOptions(oChiAbleTiles);
			}
			else
			{
				_PlayerHand.EnableAllTilesInteractability();
				_Events.EmitSignal(Events.SignalName.DrawTileRequested, this);
			}
			
		}
		else
		{
			_PlayerHand.EnableAllTilesInteractability();
			_Events.EmitSignal(Events.SignalName.DrawTileRequested, this);
		}
		
	}
	
	public void EndTurn(Mahjong.Model.Tile oTile)
	{
		_PlayerHand.DisableAllTilesInteractability();
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
		TileUIConfiguration oTileUIConfiguration = new TileUIConfiguration();
		oTileUIConfiguration.InitialHandAreaToAppendTo = "TSUMO";
		_PlayerHand.AddTileToHand(poNewTileModel, oTileUIConfiguration);
		_Hand.Tiles.Add(poNewTileModel);
		IsValidHand(poNewTileModel);
	}
	
	public bool IsValidHand(Mahjong.Model.Tile poWinTile)
	{
		Mahjong.CShantenEvaluator oShantenEvaluator = new Mahjong.CShantenEvaluator();
		int nShanten = oShantenEvaluator.EvaluateShanten(_Hand);
		_ShantenLabel.Text = "Shanten: " + nShanten;
		if(nShanten == -1)
		{
			//TODO evaluate the type of hand here.
			_Hand.Agari = Enums.Agari.Tsumo;
			_Hand.SeatWind = Enums.Wind.East;
			_Hand.RoundWind = Enums.Wind.East;
			_Hand.WinTile = poWinTile;
			
			Mahjong.CScoreEvaluator oScoreEvaluator = new Mahjong.CScoreEvaluator();
			Mahjong.Model.Score oScore = oScoreEvaluator.EvaluateScore(_Hand);
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
	
	public void OnTileDiscarded(TileUI oTileUI)
	{
		Mahjong.CTilesManager oTilesManager = new Mahjong.CTilesManager();
		oTilesManager.RemoveSingleTileOf(_Hand.Tiles, oTileUI._TileModel);
		_Hand.DiscardedTiles.Add(oTileUI._TileModel);
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
		
		bool bTile1Found = false;
		bool bTile2Found = false;
		TileUI oTileUI1 = null;
		TileUI oTileUI2 = null;
		
		LockedBlock ChiBlock = (LockedBlock) ChiScene.Instantiate();
		
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
		
		_CalledHand.AddChild(ChiBlock);
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
	
	public void OnCallCancelButtonPressed()
	{
		_PlayerHand.EnableAllTilesInteractability();
		_Events.EmitSignal(Events.SignalName.DrawTileRequested, this);
	}
}
