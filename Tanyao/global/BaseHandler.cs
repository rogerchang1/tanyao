using Godot;
using System;

public partial class BaseHandler : Node
{	
	
	public Events _Events;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public virtual void StartTurn()
	{
		_Events.EmitSignal(Events.SignalName.DrawTileRequested, this);
	}

}
