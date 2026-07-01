using UnityEngine;

public class DialogData : MonoBehaviour
{
    public AudioClip voiceClip;
    [TextArea(3, 10)]
    public string dialogText;
    public float textDelay;
    public bool autoAvanzar;
    public AnimationTimelineEntry[] animationTimeline;

    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;
    public GameObject[] objectsToActivateOnEnd;
    public GameObject[] objectsToDeactivateOnEnd;

    public System.Action onDialogEnd;

    void OnEnable()
    {
        var player = FindFirstObjectByType<DialogPlayer>();
        if (player != null)
            player.Play(this);
    }
}

[System.Serializable]
public class AnimationTimelineEntry
{
    [Range(0f, 1f)]
    public float normalizedTime;
    public AnimationClip clip;
}
