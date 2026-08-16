using Godot;
using System;
using System.Linq;
using System.Text;

public partial class PerformanceDebug : RichTextLabel
{

	double avg = 0;

	double avg1 = 0;

	double avg01 = 0;

	double time = 0;

	const int MaxFrames = 10000;

	bool frameTimesReady = false;

	double[] frameTimes = new double[MaxFrames];
	int index = 0;
	int count = 0;

	StringBuilder text = new StringBuilder();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	void Update(double delta)
	{
		avg += ((delta/Engine.TimeScale) - avg) * 0.03;
		double avgFps = 1/avg;
		text.Clear();
		text.AppendLine("Performance Info");
		text.AppendLine("Engine FPS " + Engine.GetFramesPerSecond());
		text.AppendLine("Last second FPS " + Performance.GetMonitor(Performance.Monitor.TimeFps));
		text.AppendLine("Avg FPS " + avgFps);
		text.AppendLine("Engine 1% FPS " + avg1);
		text.AppendLine("Engine 0,1% FPS " + avg01);
		Text = text.ToString();
	}

	void UpdateAvgLows(int elementCount)
	{
		double[] sorted = new double[elementCount];
		Array.Copy(frameTimes, sorted, elementCount);

		Array.Sort(sorted);

		// Slowest 1% of 1000 frames = 10 frames
		var percent = elementCount/100;
		avg1 = 1/sorted[^percent..].Average();
		percent /= 10;
		avg01 = 1/sorted[^percent..].Average();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (time > 1)
		{
			if(frameTimesReady)
				UpdateAvgLows(MaxFrames);
			else if(count > 1000)
				UpdateAvgLows(count);
			time = 0;
		}else
			time += delta;
		Update(delta);

		frameTimes[index] = delta;

		index = (index + 1) % MaxFrames;

		if (count < MaxFrames){
			count++;
		}else{
			frameTimesReady = true;
			count = 0;
		}
	}
}
