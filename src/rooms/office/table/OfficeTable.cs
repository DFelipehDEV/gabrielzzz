using Godot;
using System.Linq;

public partial class OfficeTable : StaticBody3D, Interactable
{
	[Export]
	private Sprite3D hideOverlay;

	private Player player;
	public bool IsInteractable => player.State == Player.States.Default;

	public override void _Ready()
	{
		base._Ready();
		player = (Player)GetTree().GetFirstNodeInGroup("player");
	}
	
	public void StartInteract()
	{
		if (player.AnimationPlayer.CurrentAnimation != "UnhideUnderTable")
		{
			player.AnimationPlayer.Play("HideUnderTable");
			player.State = Player.States.Hidden;
			player.Phone.Flash = false;
		}
	}

	public void StopInteract()
	{
		
	}

	public void Highlight()
	{
		hideOverlay.Transparency = 0.25f;
	}

	public void Unhighlight()
	{
		hideOverlay.Transparency = 0.99f;
	}
}
