using Godot;
using System.Linq;

public partial class Nokia : StaticBody3D, Interactable
{
	[Export]
	public double AlarmIncreaseRate = 9.5;

	[Export]
	public double AlarmDecreaseRate = 3.5;

	[Export]
	private TextureProgressBar progress;

	[Export]
	private AudioStreamPlayer3D alarmFiredAudio;

	[Signal]
	public delegate void AlarmFiredEventHandler();

	private bool delayingAlarm = false;
	public bool DelayingAlarm
	{
		get => delayingAlarm;
		set
		{
			delayingAlarm = value;
		}
	}
	private double alarmTimer = 100.0;

	private bool alarmFired = false;
	public bool HasAlarmFired
	{
		get => alarmFired;
	}

	public bool IsInteractable => !alarmFired;

	public void StartInteract()
	{
		delayingAlarm = true;
	}

	public void StopInteract()
	{
		delayingAlarm = false;
	}

	public override void _Process(double delta)
	{
		if (!alarmFired)
		{
			if (delayingAlarm)
			{
				alarmTimer += AlarmIncreaseRate * delta;
			}
			else
			{
				alarmTimer -= AlarmDecreaseRate * delta;
			}
			if (alarmTimer <= 0)
			{
				alarmFired = true;
				alarmFiredAudio.Play();
				EmitSignal(SignalName.AlarmFired);
			}
		}
		alarmTimer = Mathf.Clamp(alarmTimer, 0, 100);

		progress.Value = alarmTimer;
	}

	public void Highlight()
	{
		foreach (MeshInstance3D mesh in GetMeshes())
		{
			var material = mesh.GetActiveMaterial(0);
			if (material != null)
			{
				var newMat = material.Duplicate() as StandardMaterial3D;
				newMat.AlbedoColor = new Color(4.0f, 4.0f, 4.0f);
				mesh.MaterialOverride = newMat;
			}
		}
	}

	public void Unhighlight()
	{
		foreach (MeshInstance3D mesh in GetMeshes())
		{
			mesh.MaterialOverride = null;
		}
	}

	private MeshInstance3D[] GetMeshes()
	{
		return FindChildren("*", "MeshInstance3D", true)
				.Cast<MeshInstance3D>()
				.ToArray();
	}
}
