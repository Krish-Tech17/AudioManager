using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TTSManager : MonoBehaviour
{
    public static TTSManager Instance;

    private enum PlatformTTS
    {
        Windows,
        Mac,
        Android,
        iOS,
        Unsupported
    }

    private PlatformTTS currentPlatform = PlatformTTS.Unsupported;

    [Header("Settings")]
    private bool overrideCurrentNarration = false;

    private Queue<string> ttsQueue = new Queue<string>();
    private bool isSpeaking = false;
    private string cleanedText = "";

    // ---------------------------------------------------------
    // INITIALIZATION
    // ---------------------------------------------------------
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DetectPlatform();
        InitializeTTS();
    }

    // ---------------------------------------------------------
    // PLATFORM DETECTION
    // ---------------------------------------------------------
    private void DetectPlatform()
    {
#if UNITY_EDITOR
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                currentPlatform = PlatformTTS.Windows;
                break;

            case RuntimePlatform.OSXEditor:
                currentPlatform = PlatformTTS.Mac;
                break;

            default:
                currentPlatform = PlatformTTS.Unsupported;
                break;
        }

#elif UNITY_STANDALONE_WIN
        currentPlatform = PlatformTTS.Windows;

#elif UNITY_STANDALONE_OSX
        currentPlatform = PlatformTTS.Mac;

#elif UNITY_ANDROID
        currentPlatform = PlatformTTS.Android;

#elif UNITY_IOS
        currentPlatform = PlatformTTS.iOS;

#else
        currentPlatform = PlatformTTS.Unsupported;
#endif

        Debug.Log("TTS Platform Detected: " + currentPlatform);
    }

    private void InitializeTTS()
    {
        switch (currentPlatform)
        {
            case PlatformTTS.Windows:
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                WindowsTTS.initSpeech();
#endif
                break;

            case PlatformTTS.Mac:
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
                MacTTS.Init();
#endif
                break;

            case PlatformTTS.Android:
                Debug.Log("Android TTS not yet implemented.");
                // TODO: Initialize Android TTS
                break;

            case PlatformTTS.iOS:
                Debug.Log("iOS TTS not yet implemented.");
                // TODO: Initialize iOS TTS
                break;

            case PlatformTTS.Unsupported:
                Debug.LogWarning("TTS not supported on this platform.");
                break;
        }
    }

    // ---------------------------------------------------------
    // PUBLIC API
    // ---------------------------------------------------------
    public void SpeakInQueue(string text)
    {
        cleanedText = CleanText(text);

        if (string.IsNullOrEmpty(cleanedText))
            return;

        if (currentPlatform == PlatformTTS.Unsupported)
        {
            Debug.LogWarning("TTS unsupported on this platform.");
            return;
        }

        ttsQueue.Enqueue(cleanedText);

        if (!isSpeaking)
            TrySpeakNext();
    }

    public void StopAndSpeak(string text)
    {
        cleanedText = CleanText(text);

        if (string.IsNullOrEmpty(cleanedText))
            return;

        Stop();
        ttsQueue.Clear();

        ttsQueue.Enqueue(cleanedText);
        TrySpeakNext();
    }

    public void WaitForCurrentSpeechToEnd(System.Action callback)
    {
        StartCoroutine(WaitForSpeechEndCoroutine(callback));
    }
    private IEnumerator WaitForSpeechEndCoroutine(System.Action callback)
    {
        while (isSpeaking)
            yield return null;

        callback?.Invoke();
    }

    public void Stop()
    {
        StopAllCoroutines();

        switch (currentPlatform)
        {
            case PlatformTTS.Windows:
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                WindowsTTS.stopCurrentSpeech();
                WindowsTTS.clearSpeechQueue();
#endif
                break;

            case PlatformTTS.Mac:
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
                MacTTS.Stop();
#endif
                break;

            case PlatformTTS.Android:
                // TODO: Stop Android TTS
                break;

            case PlatformTTS.iOS:
                // TODO: Stop iOS TTS
                break;
        }

        isSpeaking = false;
    }

    // ---------------------------------------------------------
    // INTERNAL SPEAK LOGIC
    // ---------------------------------------------------------
    private void TrySpeakNext()
    {
        if (ttsQueue.Count == 0)
        {
            isSpeaking = false;
            return;
        }

        string nextText = ttsQueue.Dequeue();
        InternalSpeak(nextText);
    }

    private void InternalSpeak(string text)
    {
        if (!overrideCurrentNarration && isSpeaking)
        {
            Debug.Log("TTS busy, ignoring: " + text);
            return;
        }

        if (overrideCurrentNarration)
            Stop();

        isSpeaking = true;

        switch (currentPlatform)
        {
            case PlatformTTS.Windows:
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                WindowsTTS.clearSpeechQueue();
                WindowsTTS.addToSpeechQueue(text);
                StartCoroutine(CheckWindowsEnd());
#endif
                break;

            case PlatformTTS.Mac:
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
                MacTTS.Speak(text);
                StartCoroutine(CheckMacEnd());
#endif
                break;

            case PlatformTTS.Android:
                // TODO: Android Speak
                isSpeaking = false;
                break;

            case PlatformTTS.iOS:
                // TODO: iOS Speak
                isSpeaking = false;
                break;

            case PlatformTTS.Unsupported:
                isSpeaking = false;
                break;
        }
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private IEnumerator CheckWindowsEnd()
    {
        while (true)
        {
            string status = WindowsTTS.GetStatusMessage();

            if (status.Contains("Waiting") || status.Contains("Speech stopped"))
                break;

            yield return null;
        }

        isSpeaking = false;
        TrySpeakNext();
    }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
    private IEnumerator CheckMacEnd()
    {
        while (MacTTS.IsSpeaking())
            yield return null;

        isSpeaking = false;
        TrySpeakNext();
    }
#endif

    private void OnApplicationQuit()
    {
        switch (currentPlatform)
        {
            case PlatformTTS.Windows:
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                WindowsTTS.destroySpeech();
#endif
                break;

            case PlatformTTS.Mac:
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
                MacTTS.Stop();
#endif
                break;
        }
    }

    private string CleanText(string text)
    {
        text = Regex.Replace(text, @"(\\n|\\r|\r\n|\n)+|\s*\([^)]*\)\s*", " ").Trim();

        return FormatEquipmentCodes(text);
    }


    private string FormatEquipmentCodes(string text)
    {
        // Matches:
        // MOV-857068
        // V406377
        // AB12CD34 etc.
        return Regex.Replace(text, @"\b[A-Z]+-?\d+\b", match =>
        {
            string value = match.Value.Replace("-", "");

            return string.Join(" ", value.ToCharArray());
        });
    }
}