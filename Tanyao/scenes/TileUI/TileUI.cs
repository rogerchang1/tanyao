using Godot;
using System;
using Mahjong.Model;

public partial class TileUI : Control
{
	[Signal]
	public delegate void ReparentRequestedEventHandler(TileUI poTile, HBoxContainer poParent);
	
	[Signal]
	public delegate void TileDiscardedEventHandler(TileUI poTileUI);
	
	public ColorRect _Color;
	public Label _StateLabel;
	public Label _TileLabel;
	public TileStateMachine _TileStateMachine;
	public Area2D _DropPointDetector;
	public bool _CanBeDiscarded;
	public Node _ParentContainer;
	public Sprite2D _Sprite;
	
	[Export]
	public bool _IsInteractable = true;
	
	public bool _IsShown = true;
	
	[Export]
	public string _Tile;
	
	public Mahjong.Model.Tile _TileModel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Color = GetNode<ColorRect>("Color");
		_StateLabel = GetNode<Label>("StateLabel");
		_TileLabel = GetNode<Label>("TileLabel");
		_DropPointDetector = GetNode<Area2D>("DropPointDetector");
		_TileStateMachine = GetNode<TileStateMachine>("TileStateMachine");
		_TileStateMachine.init(this);
		_Sprite = GetNode<Sprite2D>("Sprite2D");
		_CanBeDiscarded = false;
		
		if(_Tile == null || _Tile == ""){
			_TileModel = new Mahjong.Model.Tile("1p");
		}else{
			_TileModel = new Mahjong.Model.Tile(_Tile);
		}
		
		if(_TileModel.suit == "z")
		{
			//_Sprite.SetRegionRect(new Rect2(80, 10, 63, 83));
			//AtlasTexture oAtlasTexture = new AtlasTexture();
			_Sprite.RegionRect = new Rect2(80, 10, 63, 83);
			//_Sprite.Texture = 
		}
		
		_TileLabel.Text = _TileModel.ToString();
		//Might not need this after experimentation.
		//Might be best to assign the Parent during AddTile to Hand.
		_ParentContainer = GetParent();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//TODO: this is a hack... I should figure out the right way to instantiate things...
		//if(_TileLabel.Text != _Tile)
		//{
			//_TileLabel.Text = _TileModel.ToString();
		//}
		
	}
	
	public void SetTile(Mahjong.Model.Tile poTile)
	{
		_TileModel = poTile;
		_Tile = poTile.ToString();
		_TileLabel.Text = _TileModel.ToString();
		SetSprite();
		//if(_TileModel.suit == "z")
		//{
			//_Sprite.RegionRect = new Rect2(80, 10, 63, 83);
		//}
	}
	
	public void SetTile(string psTile)
	{
		_TileModel = new Mahjong.Model.Tile(psTile);
		_Tile = psTile;
		_TileLabel.Text = _TileModel.ToString();
		SetSprite();
		//if(_TileModel.suit == "z")
		//{
			//_Sprite.RegionRect = new Rect2(80, 10, 63, 83);
		//}
	}
	
	public override void _Input(InputEvent @event)
	{
		if(!_IsInteractable)
		{
			return;
		}
		
		_TileStateMachine.OnInput(@event);
	}
	
	private void _on_gui_input(InputEvent @event)
	{
		if(!_IsInteractable)
		{
			return;
		}
		
		_TileStateMachine.OnGuiInput(@event);
	}
	
	private void _on_mouse_entered()
	{
		if(!_IsInteractable)
		{
			return;
		}
		
		_TileStateMachine.OnMouseEntered();
	}

	private void _on_mouse_exited()
	{
		if(!_IsInteractable)
		{
			return;
		}
		
		_TileStateMachine.OnMouseExited();
	}
	
	private void _on_drop_point_detector_area_entered(Area2D area)
	{
		_CanBeDiscarded = true;
	}

	private void _on_drop_point_detector_area_exited(Area2D area)
	{
		_CanBeDiscarded = false;
	}
	
	public void SetSprite()
	{
		int nTileSetX = 10;
		int nTileSetY = 10;
		if(_IsShown == false)
		{
			nTileSetX = 710;
		}else{
			if(_TileModel.suit == "p")
			{
				nTileSetY += 90;
			}
			if(_TileModel.suit == "s")
			{
				nTileSetY += 180;
			}
			if(_TileModel.suit == "m")
			{
				nTileSetY += 270;
			}
			nTileSetX += ((_TileModel.num - 1) * 70);
		}
		_Sprite.RegionRect = new Rect2(nTileSetX, nTileSetY, 63, 83);
	}
}



