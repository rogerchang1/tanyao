using Godot;
using System;

public partial class TileState : Node
{
	
	public enum State{ BASE, CLICKED, DRAGGING, RELEASED };
	
	[Signal]
	public delegate void TransitionRequestedEventHandler(TileState fromState, State toState);
	
	[Export]
	public State _State;
	
	public TileUI _TileUI;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public virtual void Enter()
	{
		return;
	}
	
	public virtual void Exit()
	{
		return;
	}
	
	public virtual void OnInput(InputEvent @event)
	{
		return;
	}
	
	public virtual void OnGuiInput(InputEvent @event)
	{
		return;
	}
	
	public virtual void OnMouseEntered()
	{
		return;
	}

	public virtual void OnMouseExited()
	{
		return;
	}
	
}
