using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPaso", menuName = "VR Izaje/Paso", order = 1)]
public class StepData : ScriptableObject
{
    public int stepNumber;
    public AudioClip voiceClip;
    [TextArea(3, 10)]
    public string dialogText;
    public PasoCompletionType completionType;
    public ExtraSystem extraSystem;

    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    public float completionTimer;
    [TextArea(1, 3)]
    public string completionMessage;
    public GameObject completionTarget;

    [Range(-5f, 5f)]
    public float timeAdjustment;
}

public enum PasoCompletionType
{
    Automatic,
    ButtonConfirm,
    Timer,
    GrabObject,
    Measurement,
    Sequence
}

public enum ExtraSystem
{
    None,
    VirtualShelf,
    Diorama,
    WindMeter,
    HookInspection,
    LoadTest
}
