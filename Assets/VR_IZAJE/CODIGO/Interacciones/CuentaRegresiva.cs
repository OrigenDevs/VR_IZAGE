using UnityEngine;

public class CuentaRegresiva : MonoBehaviour
{
    public float tiempo = 5f;
    private float tiempoRestante;

    void OnEnable()
    {
        tiempoRestante = tiempo;
    }

    void Update()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0f)
            gameObject.SetActive(false);
    }
}
