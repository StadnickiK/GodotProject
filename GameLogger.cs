using Godot;
using System;
using System.IO;

public partial class GameLogger
{
    [Export]
    private string logFilePath = "logs/app.log";
    private bool logToFile = true;

    private GameLogger()
    {
        if (logToFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            File.WriteAllText(logFilePath, string.Empty); // This ensures the file is clean
        }
    }

    public static GameLogger Instance { get; private set; } = new GameLogger();

    public void LogInfo(string message)
    {
        Log("INFO", message);
    }

    public void LogWarning(string message)
    {
        Log("WARNING", message);
    }

    public void LogError(string message)
    {
        Log("ERROR", message);
    }

    private void Log(string level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string formattedMessage = $"[{timestamp}] [{level}] {message}";

        Console.WriteLine(formattedMessage);

        if (logToFile)
        {
            try
            {
                File.AppendAllText(logFilePath, formattedMessage + System.Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGGER ERROR] Failed to write to log file: {ex.Message}");
            }
        }
    }
}
