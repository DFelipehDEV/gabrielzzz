using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class ControlOverlay : VBoxContainer
{
	[Export]
	public Texture2D LeftClick, RightClick, Number1, Number2, Number3, Number4, Number5, Number6, Number7, Number8, Layout1to8;

	public struct Control
	{
		public Control(string name, Texture2D texture)
		{
			this.Name = name;
			this.Texture = texture;
		}

		public string Name;
		public Texture2D Texture;
	}
	

	private List<Control> controls = new List<Control>(); 

	public void Add(Control control)
	{
		controls.Add(control);
		
		TextureRect icon = new TextureRect()
		{
			Texture = control.Texture,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			CustomMinimumSize = new Vector2(32.0f, 32.0f)
		};

		Label label = new Label
		{
			Text = control.Name,
			Position = new Vector2(32.0f, 2.0f)
		};
		icon.AddChild(label);

		AddChild(icon);
	}

	public void Clear()
	{
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}

		controls.Clear();
	}
}
