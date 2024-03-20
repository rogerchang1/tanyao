using Godot;
using System;

public partial class TileClickedState : TileState
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
		_TileUI._StateLabel.Text = "CLICKED";
		_TileUI._DropPointDetector.Monitoring = true;
	}
	
	public override void OnInput(InputEvent @event)
	{
		bool IsInMotion = @event is InputEventMouseMotion motionEvent;
		if(IsInMotion)
		{
			EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.DRAGGING);
		}
	}
	
}
