using Godot;

public partial class Energy : Label3D
{
    private double currentEnergy = 100.0;
    private double wasteMultiplier = 1.0;

    public double CurrentEnergy
    {
        get => currentEnergy;
        set => currentEnergy = Mathf.Clamp(value, 0, 100);
    }

    public double WasteMultiplier
    {
        get => wasteMultiplier;
        set => wasteMultiplier = Mathf.Max(value, 0);
    }

    public override void _Process(double delta)
    {
        CurrentEnergy -= 0.2 * WasteMultiplier * delta;
        Text = ((int)CurrentEnergy).ToString();
    }
}
