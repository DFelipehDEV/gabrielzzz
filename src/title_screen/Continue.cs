using Godot;
using System;
using System.Text.Json;

public partial class Continue : Button
{
	private string savePath = "user://nightdata.json";
	private string night = "res://Night1.tscn"; // Default night

	public override void _Ready()
	{
		Pressed += LoadNight;
	}

	private void LoadNight()
	{
		if (!FileAccess.FileExists(savePath))
		{
			GD.Print("No save file found, starting at Night 1.");
		}else
		{

			FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
			string json = file.GetAsText();
			file.Close();

			night = json;
			GD.Print($"Loaded Night: {night}");

		}

		GetTree().ChangeSceneToFile(night);
	}
}
