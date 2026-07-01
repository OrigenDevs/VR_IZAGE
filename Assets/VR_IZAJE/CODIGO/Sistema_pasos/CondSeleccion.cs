using UnityEngine;
using System.Collections.Generic;

public class CondSeleccion : MonoBehaviour
{
    public DialogPlayer dialogPlayer;
    public GameObject[] objetos;
    public int minObjetos = 1;

    private HashSet<GameObject> seleccionados = new HashSet<GameObject>();

    void OnEnable()
    {
        foreach (var obj in objetos)
        {
            if (obj == null) continue;
            var grab = obj.GetComponent<SimpleGrab>();
            if (grab != null)
                grab.onGrab.AddListener(() => OnObjetoSeleccionado(obj));
        }
    }

    void OnDisable()
    {
        foreach (var obj in objetos)
        {
            if (obj == null) continue;
            var grab = obj.GetComponent<SimpleGrab>();
            if (grab != null)
                grab.onGrab.RemoveAllListeners();
        }
        seleccionados.Clear();
    }

    void OnObjetoSeleccionado(GameObject obj)
    {
        if (dialogPlayer == null) return;

        seleccionados.Add(obj);

        if (seleccionados.Count >= minObjetos)
        {
            enabled = false;
            dialogPlayer.Avanzar();
        }
    }
}
