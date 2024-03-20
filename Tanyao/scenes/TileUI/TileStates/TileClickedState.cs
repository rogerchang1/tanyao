using Godot;
using System;

public partial class TileClickedState : TileState
{
	double nClickTimeHeld = 0;
	bool bIsActive = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("left_mouse") && bIsActive)
		{
			if(nClickTimeHeld >= .15){
				nClickTimeHeld = 0;
				bIsActive = false;
				EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.DRAGGING);
			}else if(nClickTimeHeld < .15)
			{
				nClickTimeHeld += delta;
			}
		}
		if(Input.IsActionJustReleased("left_mouse") && bIsActive)
		{
			nClickTimeHeld = 0;
			bIsActive = false;
			_TileUI._CanBeDiscarded = true;
			EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.RELEASED);
		}
	}
	
	public override void Enter()
	{
		_TileUI._StateLabel.Text = "CLICKED";
		_TileUI._DropPointDetector.Monitoring = true;
		bIsActive = true;
	}
	
	public override void OnInput(InputEvent @event)
	{
		//if(@event is InputEventMouseMotion motionEvent)
		//{
			//EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.DRAGGING);
		//}
	}
	
}
