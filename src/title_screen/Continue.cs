using Godot;

public partial class Continue : Button
{
	public static readonly string DefaultNight = "res://nights/Night1.tscn";
	private string savePath = "user://nightdata.json";
	private string night = DefaultNight; // Default night

	public override void _Ready()
	{
		Pressed += LoadNight;
	}

	private void LoadNight()
	{
		if (!FileAccess.FileExists(savePath))
		{
			GD.Print("No save file found, starting at Night 1.");
		}
		else
		{

			FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
			string json = file.GetAsText();
			file.Close();

			night = json;
			GD.Print($"Loaded Night: {night}");

		}

		if (GetTree().ChangeSceneToFile(night) != Error.Ok)
		{
			GD.Print($"Failed to load night scene: '{night}'. Redirecting to default: '{DefaultNight}'");
			GetTree().ChangeSceneToFile(DefaultNight);
		}
	}
}
