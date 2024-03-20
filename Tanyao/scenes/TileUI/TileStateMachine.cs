using Godot;
using System;

public partial class TileStateMachine : Node
{
	[Export]
	public TileState InitialState;
	
	public TileState CurrentState;
	Godot.Collections.Dictionary states = new Godot.Collections.Dictionary();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void init(TileUI poTile)
	{
		foreach(TileState oChildState in GetChildren())
		{
			//GD.Print("childtest");
			//states[Variant.From(oChildState._State)] = oChildState;
			states[(int)oChildState._State] = oChildState;
			oChildState.TransitionRequested += OnTransitionRequested;
			oChildState._TileUI = poTile;
		}
		if(InitialState != null)
		{
			InitialState.Enter();
			CurrentState = InitialState;
		}
	}
	
	public void OnInput(InputEvent @event)
	{
		if(CurrentState != null)
		{
			CurrentState.OnInput(@event);
		}
	}
	
	public void OnGuiInput(InputEvent @event)
	{
		if(CurrentState != null)
		{
			CurrentState.OnGuiInput(@event);
		}
	}
	
	public void OnMouseEntered()
	{
		if(CurrentState != null)
		{
			CurrentState.OnMouseEntered();
		}
	}
	
	public void OnMouseExited()
	{
		if(CurrentState != null)
		{
			CurrentState.OnMouseExited();
		}
	}
	
	public void OnTransitionRequested(TileState poFrom, TileState.State poTo){
		if(poFrom != CurrentState)
		{
			return;
		}
		//TileState NewState = (TileState) states[Variant.From(poTo)];
		TileState NewState = (TileState) states[(int)poTo];
		
		if(NewState == null)
		{
			return;
		}
		
		if(CurrentState != null)
		{
			CurrentState.Exit();
		}
		
		NewState.Enter();
		CurrentState = NewState;
	}
	
}
