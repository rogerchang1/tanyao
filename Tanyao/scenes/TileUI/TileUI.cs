using Godot;
using System;
using Mahjong.Model;

public partial class TileUI : Control
{
	[Signal]
	public delegate void ReparentRequestedEventHandler(TileUI poTile);
	
	[Signal]
	public delegate void TileDiscardedEventHandler();
	
	public ColorRect _Color;
	public Label _StateLabel;
	public Label _TileLabel;
	public TileStateMachine _TileStateMachine;
	public Area2D _DropPointDetector;
	public bool _CanBeDiscarded;
	
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
		_CanBeDiscarded = false;
		
		if(_Tile == null || _Tile == ""){
			_TileModel = new Mahjong.Model.Tile("1p");
		}else{
			_TileModel = new Mahjong.Model.Tile(_Tile);
		}
		_TileLabel.Text = _TileModel.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public override void _Input(InputEvent @event)
	{
		_TileStateMachine.OnInput(@event);
	}
	
	private void _on_gui_input(InputEvent @event)
	{
		_TileStateMachine.OnGuiInput(@event);
	}
	
	private void _on_mouse_entered()
	{
		_TileStateMachine.OnMouseEntered();
	}

	private void _on_mouse_exited()
	{
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
}



