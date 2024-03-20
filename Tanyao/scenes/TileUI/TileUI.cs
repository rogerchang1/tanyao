using Godot;
using System;

public partial class TileUI : Control
{
	[Signal]
	public delegate void ReparentRequestedEventHandler(TileUI poTile);
	
	public ColorRect _Color;
	public Label StateLabel;
	public TileStateMachine _TileStateMachine;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Color = GetNode<ColorRect>("Color");
		StateLabel = GetNode<Label>("StateLabel");
		_TileStateMachine = GetNode<TileStateMachine>("TileStateMachine");
		_TileStateMachine.init(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_gui_input(InputEvent @event)
	{
		_TileStateMachine.OnGuiInput(@event);
	}
	
	private void _on_mouse_entered()
	{
		_TileStateMachine.OnMouseEntered();
	}

	private void _on_mouse_exited()
	{
		// Replace with function body.
	}
	
}
