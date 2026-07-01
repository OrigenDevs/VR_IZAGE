using UnityEngine;
using System.Collections.Generic;

public class SistemaAnimacionCorto : MonoBehaviour
{
    public Animator animatorPersonaje;
    public List<GameObject> objetosAActivar = new List<GameObject>();
    public List<GameObject> objetosADesactivar = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (animatorPersonaje != null)
            animatorPersonaje.SetTrigger("Accidente");

        foreach (GameObject obj in objetosADesactivar)
            if (obj != null) obj.SetActive(false);

        foreach (GameObject obj in objetosAActivar)
            if (obj != null) obj.SetActive(true);
    }
}
