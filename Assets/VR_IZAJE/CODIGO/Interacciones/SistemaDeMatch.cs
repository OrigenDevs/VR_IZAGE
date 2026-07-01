using UnityEngine;

[System.Serializable]
public class ParActivable
{
    public GameObject objetoADesactivar;
    public GameObject objetoAActivar;
    public bool destruir;
}

public class SistemaDeMatch : MonoBehaviour
{
    public ParActivable[] lista;
    public AudioSource audioSource;
    public DialogPlayer dialogPlayer;

    private bool[] completados;

    void Start()
    {
        completados = new bool[lista.Length];
    }

    void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < lista.Length; i++)
        {
            var par = lista[i];
            if (completados[i]) continue;
            if (par.objetoADesactivar != other.gameObject) continue;

            if (par.destruir)
                Destroy(par.objetoADesactivar);
            else
                par.objetoADesactivar.SetActive(false);

            if (par.objetoAActivar != null)
                par.objetoAActivar.SetActive(true);

            if (audioSource != null)
                audioSource.Play();

            completados[i] = true;
            VerificarCompletado();
            break;
        }
    }

    void VerificarCompletado()
    {
        foreach (var c in completados)
            if (!c) return;

        enabled = false;
        if (dialogPlayer != null)
            dialogPlayer.Avanzar();
    }
}
