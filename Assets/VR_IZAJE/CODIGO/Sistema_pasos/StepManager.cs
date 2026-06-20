using System.Collections;
using UnityEngine;

[System.Serializable]
public class Step
{
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;
    public float delayBetweenActivations;
}

public class StepManager : MonoBehaviour
{
    public Step[] steps;
    public int currentStepIndex = 0;

    private Step currentStep;
    private bool advancing;

    void Start()
    {
        if (steps == null || steps.Length == 0) return;
        LoadStep(0);
    }

    public void AdvanceStep()
    {
        if (advancing) return;
        LoadStep(currentStepIndex + 1);
    }

    public void GoToStep(int index)
    {
        if (advancing) return;
        StopAllCoroutines();
        LoadStep(index);
    }

    void LoadStep(int index)
    {
        if (index < 0 || index >= steps.Length)
        {
            Debug.Log("Todos los pasos completados");
            enabled = false;
            return;
        }

        currentStepIndex = index;
        currentStep = steps[index];
        advancing = true;
        StartCoroutine(ApplyStep());
    }

    IEnumerator ApplyStep()
    {
        if (currentStep == null) yield break;

        foreach (var obj in currentStep.objectsToDeactivate)
            if (obj != null) obj.SetActive(false);

        foreach (var obj in currentStep.objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                yield return new WaitForSeconds(currentStep.delayBetweenActivations);
            }
        }

        advancing = false;
    }
}
