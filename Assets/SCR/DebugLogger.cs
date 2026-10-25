using System.IO;
using UnityEngine;

public static class DebugLogger
{
    private static StreamWriter writer;

    public static void Init()
    {
        writer = new StreamWriter("debug_log.txt", false);
        writer.AutoFlush = true;
    }

    public static void Log(string message)
    {
        writer?.WriteLine($"[{Time.frameCount}] {message}");
        Debug.Log(message); // дублируем в консоль
    }
}