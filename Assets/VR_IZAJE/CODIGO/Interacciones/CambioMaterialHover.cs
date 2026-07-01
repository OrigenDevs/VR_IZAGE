using UnityEngine;

public class CambioMaterialHover : MonoBehaviour
{
    public Collider colliderReferencia;
    public Material materialSecundario;
    public bool cambioAHijos = true;

    private static System.Collections.Generic.List<CambioMaterialHover> instancias = new System.Collections.Generic.List<CambioMaterialHover>();

    private MeshRenderer[] renderers;
    private Material[] materialesOriginales;

    void OnEnable() { if (!instancias.Contains(this)) instancias.Add(this); }
    void OnDisable() { instancias.Remove(this); }

    public static CambioMaterialHover BuscarPorCollider(Collider c)
    {
        foreach (var item in instancias)
            if (item.colliderReferencia == c)
                return item;
        return null;
    }

    void Awake()
    {
        if (cambioAHijos)
        {
            renderers = GetComponentsInChildren<MeshRenderer>(true);
        }
        else
        {
            var mr = GetComponent<MeshRenderer>();
            renderers = mr != null ? new[] { mr } : new MeshRenderer[0];
        }

        materialesOriginales = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            materialesOriginales[i] = renderers[i].sharedMaterial;
    }

    public void OnHoverStart()
    {
        if (materialSecundario == null) return;
        foreach (var r in renderers)
            r.sharedMaterial = materialSecundario;
    }

    public void OnHoverEnd()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].sharedMaterial = materialesOriginales[i];
        }
    }
}
