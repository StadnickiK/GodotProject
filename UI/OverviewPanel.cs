using Godot;
using System;
using System.Collections.Generic;

public class OverviewPanel : VBoxContainer
{

    List<Button> buttons = new List<Button>();

    Node _options = null;

    bool _hasPanel = false;

    [Export]
    public List<string> Values { get; set; } = new List<string>();  

    PackedScene _listPanelScene = (PackedScene)ResourceLoader.Load("res://UI/ListPanel.tscn");

    public override void _Ready(){
        GetNodes();
        foreach(Node node in _options.GetChildren()){
            node.QueueFree();
        }
        InitButtons();
    }

    void GetNodes(){
        _options = GetNode("Options");
    }

    public void AddPanel(string ValueName){
        var button = new Button();
            var listPanel = (ListPanel)_listPanelScene.Instance();
            button.Text = ValueName;
            button.Name = ValueName;
            listPanel.Title = ValueName;    // ready function overwrites the node name give in this step
            _options.AddChild(button);
            AddChild(listPanel);
            if(!_hasPanel){
                button.Disabled = true;
                _hasPanel = true;
            }else{
                listPanel.Visible = false;
            }
            ConnectButton(button);
    }

    public void AddPanel(string name, CanvasItem node){
        var button = new Button();
            button.Text = name;
            button.Name = name;
            node.Name = name;
            _options.AddChild(button);
            AddChild(node);
            if(!_hasPanel){
                button.Disabled = true;
                _hasPanel = true;
            }else{
                node.Visible = false;
            }
            ConnectButton(button);
    }

    public void AddNodeToPanel(string name, Node node){
        var listPanel = GetNode<ListPanel>(name);
        var parent = node.GetParent();
        if(parent != null){
            parent.RemoveChild(node);
        }
        listPanel.AddListItem(node);
    }

    public void ClearPanel(string name){
        var listPanel = GetNode<ListPanel>(name);
        listPanel.ClearItems();
    }

    void InitButtons(){
        foreach(string s in Values){
            AddPanel(s);
        }
    }

    void ConnectButton(Button button){
        buttons.Add(button);
        Godot.Collections.Array array = new Godot.Collections.Array();
        array.Add(button);
        button.Connect("button_up",this, nameof(_on_Button_Up), array);
    }

    public void _on_Button_Up(Node node){
        if(node is Button){
            var button = (Button)node;
            foreach(Button b in buttons){
                if(b.Disabled){
                    b.Disabled = false;
                    GetNode<CanvasItem>(b.Name).Visible = false;
                }
            }
            button.Disabled = true;
            GetNode<CanvasItem>(button.Name).Visible = true;
        }
    }

    public void ConnectToGuiInputEvent(Node node, string nodeName, string methodName){
        var listPanel = GetNode<ListPanel>(nodeName);
        listPanel.ConnectToOnGuiInput(node, methodName);
    }

    public void DisconnectToGuiInputEvent(Node node, string nodeName, string methodName){
        var listPanel = GetNode<ListPanel>(nodeName);
        listPanel.DisconnectToOnGuiInput(node, methodName);
    }
}
