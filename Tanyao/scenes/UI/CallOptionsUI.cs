using Godot;
using System;
using System.Collections.Generic;
using Mahjong.Model;

public partial class CallOptionsUI : Node2D
{
	public Button _Chi;
	public Button _Pon;
	public Button _Kan;
	public Button _DaiminKan;
	public Button _ShouminKan;
	public Button _Riichi;
	public Button _Tsumo;
	public Button _Ron;
	public Button _Cancel;
	
	[Export]
	public ChiTileOptions _ChiTileOptionsUI;
	
	[Export]
	public KanTileOptions _KanTileOptionsUI;
	
	public Events _Events;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Events = GetNode<Events>("/root/Events");
		_Chi = GetNode<Button>("ButtonContainer/Chi");
		_Pon = GetNode<Button>("ButtonContainer/Pon");
		_Kan = GetNode<Button>("ButtonContainer/Kan");
		_DaiminKan = GetNode<Button>("ButtonContainer/DaiminKan");
		_ShouminKan = GetNode<Button>("ButtonContainer/ShouminKan");
		_Riichi = GetNode<Button>("ButtonContainer/Riichi");
		_Tsumo = GetNode<Button>("ButtonContainer/Tsumo");
		_Ron = GetNode<Button>("ButtonContainer/Ron");
		HideAll();
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
	
	public void SetKanTileOptions(List<Mahjong.Model.Tile> poKanTileOptions)
	{
		_KanTileOptionsUI.ClearButtons();
		foreach(Mahjong.Model.Tile oKanTileOption in poKanTileOptions)
		{
			_KanTileOptionsUI.AddKanTileOption(oKanTileOption);
		}
	}
	
	private void _on_chi_pressed()
	{
		_ChiTileOptionsUI.Show();
	}
	
	private void _on_pon_pressed()
	{
		_Events.EmitSignal(Events.SignalName.PonButtonPressed);
	}
	
	private void _on_ron_pressed()
	{
		_Events.EmitSignal(Events.SignalName.RonButtonPressed);
	}
	
	
	private void _on_kan_pressed()
	{
		//_Events.EmitSignal(Events.SignalName.KanButtonPressed);
		_KanTileOptionsUI.Show();
	}
	
	private void _on_daiminkan_pressed()
	{
		_Events.EmitSignal(Events.SignalName.DaiminKanButtonPressed);
	}


	private void _on_shouminkan_pressed()
	{
		// Replace with function body.
	}

	private void _on_riichi_pressed()
	{
		_Events.EmitSignal(Events.SignalName.RiichiButtonPressed);
	}

	private void _on_tsumo_pressed()
	{
		_Events.EmitSignal(Events.SignalName.TsumoButtonPressed);
	}
	
	private void _on_cancel_pressed()
	{
		HideAll();
		_Events.EmitSignal(Events.SignalName.CallOptionsCancelPressed);
	}
	
	public void HideAll()
	{
		_ChiTileOptionsUI.Hide();
		_KanTileOptionsUI.Hide();
		_Chi.Hide();
		_Pon.Hide();
		_Kan.Hide();
		_DaiminKan.Hide();
		_ShouminKan.Hide();
		_Riichi.Hide();
		_Tsumo.Hide();
		_Ron.Hide();
		this.Hide();
	}
	
}
