using System.Collections;
using TMPro;
using UnityEngine;

public class DialogPlayer : MonoBehaviour
{
    public LipSyncController lipSync;
    public TextMeshPro text3D;
    public Animator robotAnimator;

    private DialogData currentDialog;
    private AudioSource audioSource;
    private Coroutine playRoutine;
    private System.Action onEndCallback;
    private AnimatorOverrideController overrideController;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (robotAnimator != null && robotAnimator.runtimeAnimatorController != null)
        {
            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = robotAnimator.runtimeAnimatorController;
            robotAnimator.runtimeAnimatorController = overrideController;
        }
    }

    public void Play(DialogData data)
    {
        Play(data, null);
    }

    public void Play(DialogData data, System.Action onEnd)
    {
        if (data == null) return;
        Stop();
        currentDialog = data;
        onEndCallback = onEnd;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
        playRoutine = StartCoroutine(PlayRoutine(data));
    }

    public void Stop()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
        if (audioSource != null) audioSource.Stop();
        if (lipSync != null) lipSync.SetAudioSource(null);
        if (text3D != null) text3D.text = "";
        currentDialog = null;
    }

    IEnumerator PlayRoutine(DialogData data)
    {
        if (lipSync != null) lipSync.SetAudioSource(audioSource);

        audioSource.clip = data.voiceClip;
        if (data.voiceClip != null)
            audioSource.Play();

        float duration = data.voiceClip != null ? data.voiceClip.length : 1f;
        float textDuration = Mathf.Max(duration - data.textDelay, 0.5f);
        float elapsed = 0f;
        int animIndex = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalized = elapsed / duration;

            while (animIndex < data.animationTimeline.Length &&
                   data.animationTimeline[animIndex].normalizedTime <= normalized)
            {
                var entry = data.animationTimeline[animIndex];
                if (entry.clip != null && overrideController != null)
                {
                    overrideController[entry.clip.name] = entry.clip;
                    robotAnimator.Play(entry.clip.name);
                }
                animIndex++;
            }

            if (text3D != null && !string.IsNullOrEmpty(data.dialogText))
            {
                float textNormalized = Mathf.Clamp01(elapsed / textDuration);
                int charCount = Mathf.FloorToInt(textNormalized * data.dialogText.Length);
                text3D.text = data.dialogText.Substring(0, Mathf.Min(charCount, data.dialogText.Length));
            }

            yield return null;
        }

        if (text3D != null) text3D.text = data.dialogText;

        foreach (var obj in data.objectsToActivate)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in data.objectsToDeactivate)
            if (obj != null) obj.SetActive(false);

        if (data.onDialogEnd != null)
            data.onDialogEnd.Invoke();

        if (onEndCallback != null)
            onEndCallback.Invoke();

        playRoutine = null;
    }
}
