using Godot;
using System;

public interface IStat : INode
{
    public enum StatVisibility
    {
        Hidden,
        Visible
    }

    public int Index { get; set; }

    public delegate void StatChangedEventHandler(IStat stat);
    public event StatChangedEventHandler StatChanged;

    Type ValueType { get; }

    object GetValue();

    IStat CloneStat();

    void Copy(IStat source);

    string ToString();
    void AddModifier(IStatModifier modifier);
    void RemoveModifier(IStatModifier modifier);
}
