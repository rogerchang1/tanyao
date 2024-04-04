using Godot;
using System;

//TODO: not sure if inheritance is the right thing to do
public partial class PlayerHandler : BaseHandler
{	
	[Export]
	Hand _PlayerHand;
	
	[Export]
	Label _ShantenLabel;
	
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
		IsValidHand();
	}
	
	public bool IsValidHand()
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
		}
		
		return false;
	}
	
	public void OnPlayerTileDiscarded()
	{
		EndTurn();
	}

}
