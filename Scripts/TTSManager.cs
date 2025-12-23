using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTSManager : MonoBehaviour
{
    public static TTSManager Instance;

    [Header("Settings")]
    private bool overrideCurrentNarration = false;

    private Queue<string> ttsQueue = new Queue<string>();
    private bool isSpeaking = false;

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

#if UNITY_STANDALONE_WIN
        WindowsTTS.initSpeech();

#elif UNITY_ANDROID              
        // TODO: Initialize Android TTS

#elif UNITY_IOS          
        // TODO: Initialize iOS TTS

#elif UNITY_STANDALONE_OSX                     
        // TODO: Initialize macOS TTS
#endif
    }

    // ----------------------------------------------------------------------
    // SAME STYLE AS: AudioManager.PlayClipInQueue()
    // ----------------------------------------------------------------------
    public void SpeakInQueue(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        ttsQueue.Enqueue(text);

        if (!isSpeaking)
            TrySpeakNext();
    }

    // ----------------------------------------------------------------------
    // Stop everything instantly and speak this text now
    // ----------------------------------------------------------------------
    public void StopAndSpeak(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        Stop();                // Stop TTS immediately
        ttsQueue.Clear();      // Remove all queued text

        ttsQueue.Enqueue(text);
        TrySpeakNext();
    }

    // ----------------------------------------------------------------------
    // Like AudioManager.Stop()
    // ----------------------------------------------------------------------
    public void Stop()
    {
        StopAllCoroutines();

#if UNITY_STANDALONE_WIN
        WindowsTTS.stopCurrentSpeech();
        WindowsTTS.clearSpeechQueue();
#elif UNITY_ANDROID              
        // TODO: Stop Android speech

#elif UNITY_IOS          
        // TODO: Stop iOS speech

#elif UNITY_STANDALONE_OSX                     
        // TODO: Stop macOS speech

#endif

        isSpeaking = false;
    }

    // INTERNAL CORE LOGIC (equivalent to TryPlayNext)
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

        // override existing narration
        if (overrideCurrentNarration)
            Stop();

        isSpeaking = true;

#if UNITY_STANDALONE_WIN
        WindowsTTS.clearSpeechQueue();
        WindowsTTS.addToSpeechQueue(text);
        StartCoroutine(CheckWindowsEnd());
#elif UNITY_ANDROID              
        // TODO: Android Speak

#elif UNITY_IOS          
        // TODO: iOS Speak

#elif UNITY_STANDALONE_OSX                     
        // TODO: macOS Speak
#endif
    }

#if UNITY_STANDALONE_WIN
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

    private void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN
        WindowsTTS.destroySpeech();

#elif UNITY_ANDROID              
        // TODO: Android functionality

#elif UNITY_IOS          
        // TODO: iOS functionality


#elif UNITY_STANDALONE_OSX                     
        // TODO: macOS functionality

#endif
    }
}
