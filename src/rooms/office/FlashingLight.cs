using Godot;

[GlobalClass]
public partial class FlashingLight : OmniLight3D
{
	private float targetStrength;
	[Export]
	private uint flashFrequency = 16;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		targetStrength = Mathf.Lerp(targetStrength, 5, 0.2f);
		if (Time.GetTicksMsec() % flashFrequency == 1)
			targetStrength -= 4;
			
		LightEnergy = Mathf.Lerp(LightEnergy, targetStrength, 0.2f);
	}
}
