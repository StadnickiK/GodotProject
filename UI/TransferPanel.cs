using Godot;
using System;

public class TransferPanel : ScrollContainer
{
    Label LocalName;
    Label GuestName;
    Node Container;

    PackedScene _TransferLabelScene = (PackedScene)ResourceLoader.Load("res://UI/TransferLabel.tscn");

    void GetNodes(){
        LocalName = GetNode<Label>("Container/Top/Local");
        GuestName = GetNode<Label>("Container/Top/Guest");
        Container = GetNode("Container");
    }

    public override void _Ready()
    {
        GetNodes();
    }


}
