using Godot;

public interface IEnterMapObject
{
    void EnterMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState state);
}

