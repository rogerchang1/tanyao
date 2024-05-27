using Godot;
using System;

public partial class GameController : Node
{
	Table _Table;
	
	public void TransitionFromStartScreenToTable()
	{
		Table oTableScene = (Table) ResourceLoader.Load<PackedScene>("res://Tanyao/scenes/Table/Table.tscn").Instantiate();
		GetTree().Root.AddChild(oTableScene);
		GetNode("/root/StartGame").QueueFree();
		//await ToSignal(GetTree().CreateTimer(2), "timeout");
		//oTableScene.StartRound();
		
	}
	
	public void TransitionFromEndScreenToStartScreen()
	{
		Node oStartGameScene = ResourceLoader.Load<PackedScene>("res://Tanyao/scenes/StartGame.tscn").Instantiate();
		GetNode("/root/EndGame").QueueFree();
		GetTree().Root.AddChild(oStartGameScene);
	}
	
	
}
