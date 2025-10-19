using Godot;
using System;

public interface IChangeControllerListener : IMapObjectController
{
    public void ChangeContoller(Player player);
}

public partial class ControllerComponent : Node
{
    private Player controller;

    public Player Controller { get => controller; set { controller = value; ChangeController?.Invoke(value); } }

    public delegate void ChangeControllerEventHandler(Player player);

    public event ChangeControllerEventHandler ChangeController;

    public override void _Ready()
    {
        foreach (var item in GetParent().GetChildren())
        {
            if (item is IChangeControllerListener listener)
                ChangeController += listener.ChangeContoller;
        }
    }

}
