using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio")]
    public AudioSource narrationSource;
    public AudioClip fallbackClip;

    private bool isMuted = false;

    // Queue for pending audio clips
    private Queue<AudioClip> audioQueue = new Queue<AudioClip>();

    // Is anything currently playing?
    private bool isPlaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (narrationSource == null)
                narrationSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    /// <summary>
    /// Adds audio to queue. Plays immediately if nothing is playing.
    /// </summary>
    public void PlayClipInQueue(AudioClip clip)
    {
        if (clip == null || isMuted)
            return;

        audioQueue.Enqueue(clip);

        if (!isPlaying)
            TryPlayNext();
    }

    /// <summary>
    /// Immediately stops current audio and plays this clip.
    /// </summary>
    public void StopAndPlay(AudioClip clip)
    {
        if (clip == null || isMuted)
            return;

        narrationSource.Stop();
        isPlaying = false;

        // Clear all queued audio
        audioQueue.Clear();

        // Play this clip NOW
        audioQueue.Enqueue(clip);
        TryPlayNext();
    }

    /// <summary>
    /// Stop everything.
    /// </summary>
    public void Stop()
    {
        StopAllCoroutines();
        narrationSource.Stop();
        isPlaying = false;
        audioQueue.Clear();
    }

    /// <summary>
    /// Play fallback audio if needed.
    /// </summary>
    public void PlayFallback()
    {
        StopAndPlay(fallbackClip);
    }

    public void SetVolume(float value)
    {
        narrationSource.volume = value;
    }

    public void Mute(bool state)
    {
        isMuted = state;
        narrationSource.mute = state;
    }

    // ------------------------- INTERNAL LOGIC ---------------------------- //

    private void TryPlayNext()
    {
        if (audioQueue.Count == 0 || isMuted)
        {
            isPlaying = false;
            return;
        }

        AudioClip nextClip = audioQueue.Dequeue();

        narrationSource.clip = nextClip;
        narrationSource.Play();
        isPlaying = true;

        StopAllCoroutines();
        StartCoroutine(WaitForClipEnd(nextClip.length));
    }

    private IEnumerator WaitForClipEnd(float duration)
    {
        yield return new WaitForSeconds(duration);

        isPlaying = false;
        TryPlayNext();
    }
}
