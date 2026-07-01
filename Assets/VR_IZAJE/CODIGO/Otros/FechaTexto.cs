using UnityEngine;
using TMPro;
using System;

public class FechaTexto : MonoBehaviour
{
    public TextMeshPro textoFechaActual;
    public TextMeshPro textoFechaVencimiento;
    public TextMeshPro textoFechaAnterior;

    void Start()
    {
        DateTime hoy = DateTime.Now;
        DateTime vencimiento = hoy.AddMonths(2);
        DateTime anterior = hoy.AddMonths(-2);

        if (textoFechaActual != null)
            textoFechaActual.text = hoy.ToString("dd/MM/yyyy");

        if (textoFechaVencimiento != null)
            textoFechaVencimiento.text = vencimiento.ToString("dd/MM/yyyy");

        if (textoFechaAnterior != null)
            textoFechaAnterior.text = anterior.ToString("dd/MM/yyyy");
    }
}
