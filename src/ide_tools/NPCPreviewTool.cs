#if TOOLS
using Godot;

[Tool]
public partial class NPCPreviewTool : Node
{
    [Export] public PackedScene npcScene;

	private Node3D npc;

	public Node3D NPC {
		get => npc;
		set {
			if (npc != value) {
				if (npc != null) {
					RemoveChild(npc);
				}
				npc = value;
				if (npc != null) {
					AddChild(npc);
				}
			}
		}
	}

    private Node3D lastSelectedNode;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            CreateOrUpdateSprite();
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Engine.IsEditorHint())
        {
            CheckSelectionAndUpdatePreview();
			EditorInterface.Singleton.GetInspector().PropertyEdited += OnInspectorEditedObjectChanged;
        }
    }


    private void CheckSelectionAndUpdatePreview()
    {
        var selectedNodes = EditorInterface.Singleton.GetSelection().GetSelectedNodes();
        if (selectedNodes.Count > 0 && selectedNodes[0] is Node3D selectedNode)
        {
            if (selectedNode != lastSelectedNode)
            {
                lastSelectedNode = selectedNode;
                CreateOrUpdateSprite(selectedNode);
            }
        }
        else
        {
            lastSelectedNode = null;
        }
    }

    public void CreateOrUpdateSprite(Node3D selectedNode = null)
    {
        if (npcScene == null)
        {
            GD.Print("No NPC scene assigned.");
            return;
        }

        if (NPC == null)
        {
            var instance = npcScene.Instantiate<Node3D>();
            AddChild(instance);
            NPC = instance;
        }

        if (selectedNode != null)
        {
            NPC.GlobalTransform = selectedNode.GlobalTransform;
        }
    }

	private void OnInspectorEditedObjectChanged(string property)
    {
		NPC = null;

        CreateOrUpdateSprite(null);
    }
}
#endif