using Godot;
using System;
using System.Collections.Generic;

public partial class CallOptionsUI : Node2D
{
	public Button _Chi;
	public Button _Pon;
	public Button _Kan;
	public Button _Riichi;
	public Button _Tsumo;
	public Button _Ron;
	public Button _Cancel;
	public List<List<Mahjong.Model.Tile>> _ChiTileOptions = new List<List<Mahjong.Model.Tile>>();
	
	[Export]
	public ChiTileOptions _ChiTileOptionsUI;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Chi = GetNode<Button>("CallOptions/ButtonContainer/Chi");
		_Pon = GetNode<Button>("CallOptions/ButtonContainer/Pon");
		_Kan = GetNode<Button>("CallOptions/ButtonContainer/Kan");
		_Riichi = GetNode<Button>("CallOptions/ButtonContainer/Riichi");
		_Tsumo = GetNode<Button>("CallOptions/ButtonContainer/Tsumo");
		_Ron = GetNode<Button>("CallOptions/ButtonContainer/Ron");
		_Cancel = GetNode<Button>("CallOptions/ButtonContainer/Cancel");
		_Chi.Hide();
		_Pon.Hide();
		_Kan.Hide();
		_Riichi.Hide();
		_Tsumo.Hide();
		_Ron.Hide();
	}
	
	private void _on_chi_pressed()
	{
		// Replace with function body.
	}
	
	private void _on_cancel_pressed()
	{
		this.Hide();
		_Chi.Hide();
		_Pon.Hide();
		_Kan.Hide();
		_Riichi.Hide();
		_Tsumo.Hide();
		_Ron.Hide();
	}
	
}
