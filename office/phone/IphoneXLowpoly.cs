using Godot;
using System;

public partial class IphoneXLowpoly : Node3D
{
    private Node3D phoneHover;

    public override void _Ready()
    {
        phoneHover = GetNode<Node3D>("PhoneModelHover");
    }
}
