using Godot;
using System;

public partial class TileBaseState : TileState
{
	double nClickTimeHeld = 0;
	
	private float OriginalPositionY;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("left_mouse")){
			if(nClickTimeHeld < 1)
			{
				nClickTimeHeld += 0;
			}else{
				nClickTimeHeld = 0;
			}
		}
	}
	
	public async override void Enter()
	{
		if(!_TileUI.IsNodeReady()){
			await ToSignal(_TileUI, "ready");
		}

		_TileUI._StateLabel.Text = "BASE";
		_TileUI._Color.Color = new Color(0x387ddb);
		
		_TileUI.EmitSignal(TileUI.SignalName.ReparentRequested, _TileUI, _TileUI._ParentContainer);
		_TileUI.PivotOffset = Vector2.Zero;
	}
	
	public override void OnGuiInput(InputEvent @event)
	{
		if(@event.IsActionPressed("left_mouse"))
		{
			_TileUI.PivotOffset = _TileUI.GetGlobalMousePosition() - _TileUI.GlobalPosition;
			EmitSignal(TileState.SignalName.TransitionRequested, this, (int)TileState.State.CLICKED);
		}
	}
	
	public override void OnMouseEntered()
	{
		OriginalPositionY = _TileUI.Position.Y;
		_TileUI.SetPosition(new Vector2(_TileUI.Position.X, _TileUI.Position.Y - (_TileUI.Size.Y / 10)));
	}

	public override void OnMouseExited()
	{
		_TileUI.SetPosition(new Vector2(_TileUI.Position.X, OriginalPositionY)) ;
	}
	
}
