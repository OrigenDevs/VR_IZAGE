using UnityEngine;
using System.Collections.Generic;

public class CondTrigger : MonoBehaviour
{
    public DialogPlayer dialogPlayer;
    public int minObjetos = 1;

    private HashSet<GameObject> objetosDentro = new HashSet<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (dialogPlayer == null) return;

        objetosDentro.Add(other.gameObject);

        if (objetosDentro.Count >= minObjetos)
        {
            enabled = false;
            dialogPlayer.Avanzar();
        }
    }
}
