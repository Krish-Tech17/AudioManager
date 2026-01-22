using UnityEngine;
using System.Diagnostics;

public static class MacTTS
{
    private static Process process;

    public static void Init()
    {

    }

    public static void Speak(string text)
    {
        Stop();

        process = new Process();
        process.StartInfo.FileName = "say";
        process.StartInfo.Arguments = text;
        process.StartInfo.UseShellExecute = false;
        process.Start();
    }

    public static void Stop()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
            process = null;
        }
    }

    public static bool IsSpeaking()
    {
        return process != null && !process.HasExited;
    }
}
