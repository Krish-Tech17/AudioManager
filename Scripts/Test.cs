using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Test : MonoBehaviour
{
    [SerializeField] TMP_InputField words;

    public AudioClip[] audioClips;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayAudioClips();
    }

    public void PlayAudio()
    {
     
        TTSManager.Instance.SpeakInQueue(words.text);
        TTSManager.Instance.SpeakInQueue("How are you");
        //StartCoroutine(Next());
    }

    IEnumerator Next()
    {

        yield return new WaitForSeconds(2f);
        TTSManager.Instance.StopAndSpeak("Vijay");
    }

    public void PlayAudioClips()
    {
        
        AudioManager.Instance.PlayClipInQueue(audioClips[6]);
        AudioManager.Instance.PlayClipInQueue(audioClips[1]);
        AudioManager.Instance.PlayClipInQueue(audioClips[2]);
        StartCoroutine(PlayCurrentAudio());
    }

    IEnumerator PlayCurrentAudio()
    {

        yield return new WaitForSeconds(1f);
        AudioManager.Instance.StopAndPlay(audioClips[2]);
    }
}
