using UnityEngine;

public class RotacionArrastre : MonoBehaviour
{
    public float sensibilidad = 1f;

    private SphereCollider esfera;

    private bool rotando;
    private Quaternion rotacionInicial;
    private Vector3 direccionInicial;

    void Awake()
    {
        esfera = GetComponent<SphereCollider>();
    }

    public void EmpezarRotacion(Vector3 hitPoint)
    {
        rotando = true;
        rotacionInicial = transform.rotation;
        direccionInicial = (hitPoint - esfera.bounds.center).normalized;
    }

    public void Rotar(Vector3 hitPointActual)
    {
        if (!rotando) return;

        Vector3 centro = esfera.bounds.center;
        Vector3 direccionActual = (hitPointActual - centro).normalized;
        Quaternion delta = Quaternion.FromToRotation(direccionInicial, direccionActual);
        delta.ToAngleAxis(out float angulo, out Vector3 eje);
        if (float.IsNaN(eje.x)) eje = Vector3.up;
        transform.rotation = Quaternion.AngleAxis(angulo * sensibilidad, eje) * rotacionInicial;
    }

    public void TerminarRotacion()
    {
        rotando = false;
    }
}
