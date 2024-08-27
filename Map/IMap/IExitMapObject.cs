using Godot;

public interface IExitMapObject
{
    void ExitMapObject(Node node, Vector3 exitVec, PhysicsDirectBodyState3D state);
}
