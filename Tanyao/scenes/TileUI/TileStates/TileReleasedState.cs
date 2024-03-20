using Godot;
using System;

public partial class TileReleasedState : TileState
{
	bool played;
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
		played = false;
		_TileUI._StateLabel.Text = "RELEASED";
		_TileUI._Color.Color = new Color(0, 1, 0, 1);
		if(_TileUI._DropPointDetectorOn)
		{
			played = true;
		}
	}
	
	public override void OnInput(InputEvent @event)
	{
		if(played)
		{
			return;
		}
		EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.BASE);
	}
	
}
