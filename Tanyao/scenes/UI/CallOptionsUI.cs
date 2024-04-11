using Godot;
using System;
using System.Collections.Generic;
using Mahjong.Model;

public partial class CallOptionsUI : Node2D
{
	public Button _Chi;
	public Button _Pon;
	public Button _Kan;
	public Button _Riichi;
	public Button _Tsumo;
	public Button _Ron;
	public Button _Cancel;
	
	[Export]
	public ChiTileOptions _ChiTileOptionsUI;
	
	public Events _Events;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		_Chi = GetNode<Button>("ButtonContainer/Chi");
		_Pon = GetNode<Button>("ButtonContainer/Pon");
		_Kan = GetNode<Button>("ButtonContainer/Kan");
		_Riichi = GetNode<Button>("ButtonContainer/Riichi");
		_Tsumo = GetNode<Button>("ButtonContainer/Tsumo");
		_Ron = GetNode<Button>("ButtonContainer/Ron");
		_Cancel = GetNode<Button>("ButtonContainer/Cancel");
		_Chi.Hide();
		_Pon.Hide();
		_Kan.Hide();
		_Riichi.Hide();
		_Tsumo.Hide();
		_Ron.Hide();
		_ChiTileOptionsUI.Hide();
	}
	
	public void SetChiTileOptions(List<List<Mahjong.Model.Tile>> poChiTileOptions)
	{
		_ChiTileOptionsUI.ClearButtons();
		foreach(List<Mahjong.Model.Tile> oChiTileOption in poChiTileOptions)
		{
			//Expect each oChiTileOption to have 2 tiles
			_ChiTileOptionsUI.AddChiTileOption(oChiTileOption[0],oChiTileOption[1]);
			//_ChiTileOptionsUI.Show();
		}
	}
	
	private void _on_chi_pressed()
	{
		// Replace with function body.
		GD.Print("hi");
		_ChiTileOptionsUI.Show();
	}
	
	private void _on_cancel_pressed()
	{
		HideAll();
		_Events.EmitSignal(Events.SignalName.CallOptionsCancelPressed);
	}
	
	public void HideAll()
	{
		_ChiTileOptionsUI.Hide();
		_Chi.Hide();
		_Pon.Hide();
		_Kan.Hide();
		_Riichi.Hide();
		_Tsumo.Hide();
		_Ron.Hide();
		this.Hide();
	}
	
}
