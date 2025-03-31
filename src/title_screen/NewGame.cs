using Godot;
using System;
using System.Text.Json;

public partial class NewGame : Button
{
	public override void _Ready()
	{
		Pressed += NewNight;
	}
	 public void NewNight()
	 {
		FileAccess file = FileAccess.Open("user://nightdata.json", FileAccess.ModeFlags.Write);
		file.StoreString("res://Night1.tscn");
		file.Close();
		GetTree().ChangeSceneToFile("res://night_start/NightStart.tscn");
	 }

}
