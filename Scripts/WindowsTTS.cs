using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class WindowsTTS
{
    [DllImport("WindowsVoice", CallingConvention = CallingConvention.Cdecl)]
    public static extern void initSpeech();

    [DllImport("WindowsVoice", CallingConvention = CallingConvention.Cdecl)]
    public static extern void destroySpeech();

    [DllImport("WindowsVoice", CallingConvention = CallingConvention.Cdecl)]
    public static extern void addToSpeechQueue([MarshalAs(UnmanagedType.LPStr)] string s);

    [DllImport("WindowsVoice", CallingConvention = CallingConvention.Cdecl)]
    public static extern void clearSpeechQueue();

    [DllImport("WindowsVoice", CallingConvention = CallingConvention.Cdecl)]
    public static extern void stopCurrentSpeech();

    [DllImport("WindowsVoice", CallingConvention = CallingConvention.Cdecl)]
    private static extern void statusMessage(StringBuilder sb, int length);

    public static string GetStatusMessage()
    {
        StringBuilder sb = new StringBuilder(128);
        try
        {
            statusMessage(sb, sb.Capacity);
            return sb.ToString();
        }
        catch (Exception e)
        {
            Debug.LogWarning("WindowsVoice.GetStatusMessage exception: " + e);
            return "Waiting";
        }
    }
}
