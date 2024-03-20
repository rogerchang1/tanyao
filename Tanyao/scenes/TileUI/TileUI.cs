using Godot;
using System;

public partial class TileUI : Control
{
	[Signal]
	public delegate void ReparentRequestedEventHandler(TileUI poTile);
	
	public ColorRect _Color;
	public Label _StateLabel;
	public TileStateMachine _TileStateMachine;
	public Area2D _DropPointDetector;
	public bool _DropPointDetectorOn;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Color = GetNode<ColorRect>("Color");
		_StateLabel = GetNode<Label>("StateLabel");
		_DropPointDetector = GetNode<Area2D>("DropPointDetector");
		_TileStateMachine = GetNode<TileStateMachine>("TileStateMachine");
		_TileStateMachine.init(this);
		_DropPointDetectorOn = false;
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
		_DropPointDetectorOn = true;
		GD.Print("area entered");
	}

	private void _on_drop_point_detector_area_exited(Area2D area)
	{
		_DropPointDetectorOn = false;
		GD.Print("area exited");
	}
}



