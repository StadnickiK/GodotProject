using Godot;
using System;

public interface ICollider3D : INode3D
{
    Vector3 GetUnitSize();

    Aabb GetAabb();
}
