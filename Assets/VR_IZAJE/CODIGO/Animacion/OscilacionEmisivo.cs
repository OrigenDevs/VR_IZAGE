using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OscilacionEmisivo : MonoBehaviour
{
    public Color colorEmisivo = Color.white;
    [Range(0f, 10f)]
    public float intensidadMinima;
    [Range(0f, 10f)]
    public float intensidadMaxima = 1f;
    public float velocidad = 1f;

    private Renderer rend;
    private Material materialInstancia;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        rend = GetComponent<Renderer>();
        materialInstancia = rend.material;
        materialInstancia.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * velocidad) + 1f) * 0.5f;
        float intensidad = Mathf.Lerp(intensidadMinima, intensidadMaxima, t);
        materialInstancia.SetColor(EmissionColor, colorEmisivo * intensidad);
    }

    void OnDestroy()
    {
        if (materialInstancia != null)
            Destroy(materialInstancia);
    }
}
