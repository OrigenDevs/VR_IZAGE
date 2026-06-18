using UnityEngine;

public class DialogData : MonoBehaviour
{
    public AudioClip voiceClip;
    [TextArea(3, 10)]
    public string dialogText;
    public AnimationTimelineEntry[] animationTimeline;

    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    public System.Action onDialogEnd;

    void OnEnable()
    {
        var player = FindObjectOfType<DialogPlayer>();
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
