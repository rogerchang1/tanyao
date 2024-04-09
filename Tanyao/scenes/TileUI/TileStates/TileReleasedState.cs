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
		if(_TileUI._CanBeDiscarded)
		{
			played = true;
			var Discards = GetTree().GetFirstNodeInGroup("DiscardsGroup");
			if(Discards != null)
			{
				_TileUI.Reparent(Discards);
			}
			Events _Events = GetNode<Events>("/root/Events");
			_Events.EmitSignal(Events.SignalName.TileDiscarded, _TileUI);
			//_TileUI.EmitSignal(TileUI.SignalName.TileDiscarded, _TileUI);
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
