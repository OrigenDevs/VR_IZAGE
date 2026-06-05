using System.Collections;
using TMPro;
using UnityEngine;

public class DialogSystemVR : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject dialogPanel;

    private AudioSource audioSource;
    private System.Action onComplete;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play(AudioClip clip, string text, float timeAdjust, System.Action callback)
    {
        onComplete = callback;
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(TypeText(clip, text, timeAdjust));
    }

    IEnumerator TypeText(AudioClip clip, string text, float timeAdjust)
    {
        textDisplay.text = "";

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();

            float duration = clip.length + timeAdjust;
            duration = Mathf.Max(duration, 0.1f);
            float timePerChar = duration / text.Length;

            foreach (char c in text)
            {
                textDisplay.text += c;
                yield return new WaitForSeconds(timePerChar);
            }

            while (audioSource.isPlaying)
                yield return null;
        }
        else
        {
            textDisplay.text = text;
        }

        if (onComplete != null)
            onComplete.Invoke();
    }

    public void Stop()
    {
        StopAllCoroutines();
        if (audioSource.isPlaying)
            audioSource.Stop();
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }
}
