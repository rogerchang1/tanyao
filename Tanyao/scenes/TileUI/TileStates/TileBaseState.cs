using Godot;
using System;

public partial class TileBaseState : TileState
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public async override void Enter()
	{
		await ToSignal(GetParent(), "ready");

		_TileUI.StateLabel.Text = "BASE";
		EmitSignal(TileUI.SignalName.ReparentRequested, _TileUI);
		_TileUI.PivotOffset = Vector2.Zero;
	}
	
	public override void OnGuiInput(InputEvent @event)
	{
		if(@event.IsActionPressed("left_mouse"))
		{
			_TileUI.PivotOffset = _TileUI.GetGlobalMousePosition() - _TileUI.GlobalPosition;
			EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.CLICKED);
			//card_ui.pivot_offset = card_ui.get_global_mouse_position() - card_ui.global_position
			//transition_requested.emit(self, CardState.State.CLICKED)
		}
	}
}
