using Godot;
using System;

public interface IUnit3D : IMapObjectController, ITargetable, IMovable3D, IStatManager, IInputController, ISavingNode, ICardIndex
{
    public ProjectileFactory ProjectileFactory { get; set; }

    void LoadUnit(IStatManager unit);
    void UpdateModel(Unit unit);

    public void Initialize(UnitFactory unitFactory, Node parent, Vector3 position, Player controller, Unit unit);

    //public void Initialize(Node parent, Vector3 position, Player controller, Unit unit);

    void UpdatePosition(Vector3 position, float yRotation = 0);

    //float GetVisionRange();

    void ChangeMovableState(IMovableState movableState);

    public event DrawingLines3DController.SpawnOrderMarkersHandler SpawnMarkers;

    public event DrawingLines3DController.SpawnOrderMarkersHandler ClearMarkers;

    // public event DrawingLines3DController.RedrawOrderMarkersHandler RedrawMarkers;

}
