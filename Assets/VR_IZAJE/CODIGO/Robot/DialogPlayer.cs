using System.Collections;
using TMPro;
using UnityEngine;

public class DialogPlayer : MonoBehaviour
{
    public LipSyncController lipSync;
    public TextMeshPro text3D;
    public Animator robotAnimator;
    public GameObject[] dialogList;
    public int currentIndex = 0;

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

    void Start()
    {
        if (dialogList == null || dialogList.Length == 0) return;
        if (currentIndex < dialogList.Length && dialogList[currentIndex] != null)
        {
            Debug.Log($"DialogPlayer: Iniciando diálogo {currentIndex + 1}/{dialogList.Length} - {dialogList[currentIndex].name}");
            dialogList[currentIndex].SetActive(true);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Saltar();
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

    public void Saltar()
    {
        if (currentDialog == null) return;

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
        if (audioSource != null) audioSource.Stop();
        if (lipSync != null) lipSync.SetAudioSource(null);
        if (text3D != null) text3D.text = currentDialog.dialogText;

        FinalizarDialog(currentDialog);
    }

    void FinalizarDialog(DialogData data)
    {
        foreach (var obj in data.objectsToActivateOnEnd)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in data.objectsToDeactivateOnEnd)
            if (obj != null) obj.SetActive(false);

        if (data.onDialogEnd != null)
            data.onDialogEnd.Invoke();

        if (onEndCallback != null)
            onEndCallback.Invoke();

        var shouldAdvance = data.autoAvanzar;
        currentDialog = null;

        if (shouldAdvance)
            Avanzar();
    }

    public void Avanzar()
    {
        if (dialogList == null || dialogList.Length == 0) return;

        if (currentIndex >= 0 && currentIndex < dialogList.Length && dialogList[currentIndex] != null)
        {
            Debug.Log($"DialogPlayer: Apagando diálogo {currentIndex + 1} - {dialogList[currentIndex].name}");
            dialogList[currentIndex].SetActive(false);
        }

        currentIndex++;

        if (currentIndex >= dialogList.Length)
        {
            Debug.Log("DialogPlayer: Todos los diálogos completados");
            return;
        }

        if (dialogList[currentIndex] != null)
        {
            Debug.Log($"DialogPlayer: Activando diálogo {currentIndex + 1}/{dialogList.Length} - {dialogList[currentIndex].name}");
            dialogList[currentIndex].SetActive(true);
        }
    }

    public void IrADialogo(int index)
    {
        if (dialogList == null || dialogList.Length == 0) return;
        if (index < 0 || index >= dialogList.Length) return;
        Stop();

        if (currentIndex >= 0 && currentIndex < dialogList.Length && dialogList[currentIndex] != null)
        {
            Debug.Log($"DialogPlayer: Apagando diálogo {currentIndex + 1} - {dialogList[currentIndex].name}");
            dialogList[currentIndex].SetActive(false);
        }

        currentIndex = index;

        if (dialogList[currentIndex] != null)
        {
            Debug.Log($"DialogPlayer: Saltando a diálogo {currentIndex + 1}/{dialogList.Length} - {dialogList[currentIndex].name}");
            dialogList[currentIndex].SetActive(true);
        }
    }

    IEnumerator PlayRoutine(DialogData data)
    {
        if (lipSync != null) lipSync.SetAudioSource(audioSource);

        foreach (var obj in data.objectsToActivate)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in data.objectsToDeactivate)
            if (obj != null) obj.SetActive(false);

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

        playRoutine = null;
        FinalizarDialog(data);
    }
}
