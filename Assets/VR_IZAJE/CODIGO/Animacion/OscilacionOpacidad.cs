using UnityEngine;

public class OscilacionOpacidad : MonoBehaviour
{
    [Range(0f, 1f)]
    public float opacidadMinima;
    [Range(0f, 1f)]
    public float opacidadMaxima = 1f;
    public float velocidad = 1f;

    private Renderer rend;
    private Material materialInstancia;
    private Color colorOriginal;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            enabled = false;
            return;
        }
        materialInstancia = rend.material;
        colorOriginal = materialInstancia.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * velocidad) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(opacidadMinima, opacidadMaxima, t);
        Color c = colorOriginal;
        c.a = alpha;
        materialInstancia.color = c;
    }

    void OnDestroy()
    {
        if (materialInstancia != null)
            Destroy(materialInstancia);
    }
}
