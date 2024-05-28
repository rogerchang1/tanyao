using System;

public class TileUIConfiguration
{
	public bool IsInteractable = true;
	public bool IsShown = true;
	
	//TODO: might want to make this an enum instead...
	// values are CLOSED, TSUMO, LOCKED?, Also adding this to the dead wall..
	public string InitialHandAreaToAppendTo = "";
}
