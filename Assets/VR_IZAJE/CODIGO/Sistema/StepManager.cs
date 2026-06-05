using System.Collections;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public StepData[] steps;
    public DialogSystemVR dialogSystem;
    public LipSyncController lipSync;
    public UI_ConfirmPopup confirmPopup;
    public int currentStepIndex = 0;

    private StepData currentStep;
    private bool waitingForCompletion = false;

    void Start()
    {
        if (steps == null || steps.Length == 0)
        {
            Debug.LogWarning("No hay pasos asignados al StepManager");
            return;
        }
        LoadStep(0);
    }

    public void LoadStep(int index)
    {
        if (index < 0 || index >= steps.Length)
        {
            Debug.Log("Todos los pasos completados");
            return;
        }

        waitingForCompletion = false;

        if (currentStep != null)
            foreach (var obj in currentStep.objectsToActivate)
                if (obj != null) obj.SetActive(false);

        currentStepIndex = index;
        currentStep = steps[index];

        foreach (var obj in currentStep.objectsToActivate)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in currentStep.objectsToDeactivate)
            if (obj != null) obj.SetActive(false);

        if (dialogSystem != null)
        {
            dialogSystem.Play(currentStep.voiceClip, currentStep.dialogText, currentStep.timeAdjustment, OnDialogEnd);
        }

        if (lipSync != null && dialogSystem != null)
        {
            lipSync.SetAudioSource(dialogSystem.GetAudioSource());
        }
    }

    void OnDialogEnd()
    {
        switch (currentStep.completionType)
        {
            case PasoCompletionType.Automatic:
                StartCoroutine(AutoAdvance());
                break;

            case PasoCompletionType.ButtonConfirm:
                if (confirmPopup != null)
                    confirmPopup.Show(currentStep.completionMessage, CompleteStep);
                else
                    StartCoroutine(AutoAdvance());
                break;

            case PasoCompletionType.Timer:
                waitingForCompletion = true;
                StartCoroutine(TimerAdvance(currentStep.completionTimer));
                break;

            default:
                waitingForCompletion = true;
                break;
        }
    }

    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(1f);
        CompleteStep();
    }

    IEnumerator TimerAdvance(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (waitingForCompletion)
            CompleteStep();
    }

    public void NotifyComplete()
    {
        if (waitingForCompletion)
            CompleteStep();
    }

    void CompleteStep()
    {
        waitingForCompletion = false;
        LoadStep(currentStepIndex + 1);
    }

    public void GoToStep(int index)
    {
        StopAllCoroutines();
        if (dialogSystem != null)
            dialogSystem.Stop();
        LoadStep(index);
    }
}
