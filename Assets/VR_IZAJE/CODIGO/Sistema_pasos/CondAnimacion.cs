using UnityEngine;
using System.Collections;

public class CondAnimacion : MonoBehaviour
{
    public Animator animatorObjetivo;
    public RuntimeAnimatorController controllerAnimacion;

    void OnEnable()
    {
        if (animatorObjetivo != null && controllerAnimacion != null)
            StartCoroutine(ReproducirYAvanzar());
    }

    IEnumerator ReproducirYAvanzar()
    {
        animatorObjetivo.runtimeAnimatorController = controllerAnimacion;
        animatorObjetivo.Play(0, 0, 0f);

        yield return null;

        AnimatorClipInfo[] clipInfo = animatorObjetivo.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
            yield return new WaitForSeconds(clipInfo[0].clip.length);

        TransicionVr.ParpadearConTransporte();
    }
}
