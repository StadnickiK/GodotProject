using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class TabsOrganizer : VBoxContainer
{
    protected Dictionary<Node, bool> Tabs { get; set; } = new Dictionary<Node, bool>();

    protected Node TabBin;

    public TabContainer TabContainer { get; set; }

    public override void _Ready()
    {
        TabContainer = GetNode<TabContainer>("Tabs");
        foreach (var Node in TabContainer.GetChildren())
        {
            Tabs.Add(Node, true);
        }
        TabBin = GetNode("TabBin");
        
    }

    public void HideTab(int index)
    {
        var node = Tabs.Keys.ElementAt(index);
        if (node.GetParent() != TabBin)
        {
            node.Reparent(TabBin);
            Tabs[node] = false;
        }
    }

    public void ShowTab(int index)
    {
        var node = Tabs.Keys.ElementAt(index);
        if (node.GetParent() != TabContainer)
        {
            foreach (var item in TabContainer.GetChildren())
                TabContainer.RemoveChild(item);
            Tabs[node] = true;
            TabBin.RemoveChild(node);
            foreach (var item in Tabs.Where(x => x.Value == true))
                TabContainer.AddChild(item.Key);
            
        }
    }
}
