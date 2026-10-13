using UnityEngine;
using System.Diagnostics;

public static class MacTTS
{
    private static Process process;
    private static string currentVoice = "Samantha";
    public static void Init()
    {

    }

    public static void Speak(string text)
    {
        Stop();

        process = new Process();
        process.StartInfo.FileName = "say";
        process.StartInfo.Arguments = $"-v \"{currentVoice}\" -r 180 \"{EscapeText(text)}\"";
        //process.StartInfo.Arguments = text;
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

    private static string EscapeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        // Escape quotes for shell-style argument handling
        return text.Replace("\\", "\\\\")
                   .Replace("\"", "\\\"");
    }

    public static bool IsSpeaking()
    {
        return process != null && !process.HasExited;
    }
}
