using UnityEngine;

public class CondTiempo : MonoBehaviour
{
    public DialogPlayer dialogPlayer;
    public float tiempoSegundos = 2f;
    public bool iniciarAlInicio = true;

    void Start()
    {
        if (iniciarAlInicio && dialogPlayer != null)
            Invoke(nameof(Ejecutar), tiempoSegundos);
    }

    public void Iniciar()
    {
        if (dialogPlayer != null)
            Invoke(nameof(Ejecutar), tiempoSegundos);
    }

    void Ejecutar()
    {
        dialogPlayer.Avanzar();
    }
}
