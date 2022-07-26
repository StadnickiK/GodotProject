using Godot;
using System;
using System.IO;
using System.Text;

public class Counter : Node
{
    [Export]
    public int FrameCount { get; set; } = 6000;

    [Export]
    public int FramesToSkip { get; set; } = 20;

    [Export]
    public string Path { get; set; } = @"D:\Godot\";

    [Export]
    public string CountFileName { get; set; } = "test.txt";

    [Export]
    public string StaticMemory { get; set; } = "test.txt";

    [Export]
    public string DynamicMemory { get; set; } = "test.txt";

    [Export]
    public string VideoMem { get; set; } = "test.txt";

    [Export]
    public string TextureMem { get; set; } = "test.txt";

    // DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")

    StreamWriter  Stream;

    FileStream fs;

    int CurrentCount = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Stream = System.IO.File.CreateText(FrameCount +"-"+ DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + ".txt");
        // fs = System.IO.File.OpenWrite(CountFileName);
        CountFileName = "AvgFrameLength "+FrameCount +"-"+ DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm") + ".txt";
        using (StreamWriter sw = System.IO.File.CreateText(Path+CountFileName))
            {
                sw.Close();
            }	
        FrameCount += FramesToSkip;

        StaticMemory = "StaticMemory "+FrameCount +"-"+ DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm") + ".txt";
        using (StreamWriter sw = System.IO.File.CreateText(Path+StaticMemory))
            {
                sw.Close();
            }	

        DynamicMemory = "DynamicMemory "+FrameCount +"-"+ DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm") + ".txt";
        using (StreamWriter sw = System.IO.File.CreateText(Path+DynamicMemory))
            {
                sw.Close();
            }		
    }

    public override void _Process(float delta)
    {
        if(CurrentCount < FrameCount){

            using(StreamWriter stw = System.IO.File.AppendText(Path+CountFileName)){
                stw.WriteLine(delta);
                stw.Close();
            }

            using(StreamWriter stw = System.IO.File.AppendText(Path+StaticMemory)){
                stw.WriteLine(OS.GetStaticMemoryUsage());
                stw.Close();
            }

            using(StreamWriter stw = System.IO.File.AppendText(Path+DynamicMemory)){
                stw.WriteLine(OS.GetDynamicMemoryUsage());
                stw.Close();
            }
            CurrentCount++;
        }
    }
}
