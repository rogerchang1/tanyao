using Godot;
using System;

public partial class TileDraggingState : TileState
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void Enter()
	{
		var UILayer = GetTree().GetFirstNodeInGroup("UILayer");
		if(UILayer != null)
		{
			_TileUI.Reparent(UILayer);
		}
		_TileUI._StateLabel.Text = "DRAGGING";
		_TileUI._Color.Color = new Color(1, 0, 0, 1);
	}
	
	public override void OnInput(InputEvent @event)
	{
		bool MouseMotion = @event is InputEventMouseMotion motionEvent;
		bool cancel = @event.IsActionPressed("right_mouse");
		bool confirm = @event.IsActionPressed("left_mouse") || @event.IsActionReleased("left_mouse");
		
		if(MouseMotion)
		{
			_TileUI.GlobalPosition = _TileUI.GetGlobalMousePosition() - _TileUI.PivotOffset;
		}
		
		if(cancel)
		{
			EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.BASE);
		}else if(confirm)
		{
			GetViewport().SetInputAsHandled();
			EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.RELEASED);
		}
		
		
	}
}
