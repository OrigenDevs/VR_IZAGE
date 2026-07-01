using UnityEngine;

public class CondAgarreTiempo : MonoBehaviour
{
    public DialogPlayer dialogPlayer;
    public float tiempoRequerido = 3f;
    public GameObject barraDeTiempo;
    public AudioSource audioSourceLoop;
    public EjeEscala ejeEscala = EjeEscala.X;

    public enum EjeEscala { X, Y, Z }

    private SimpleGrab simpleGrab;
    private float tiempoAcumulado = 0f;
    private bool agarrado = false;
    private bool completado = false;

    void OnEnable()
    {
        simpleGrab = GetComponent<SimpleGrab>();
        if (simpleGrab != null)
        {
            simpleGrab.onGrab.AddListener(OnGrab);
            simpleGrab.onRelease.AddListener(OnRelease);
        }
        ResetearBarra();
    }

    void OnDisable()
    {
        if (simpleGrab != null)
        {
            simpleGrab.onGrab.RemoveListener(OnGrab);
            simpleGrab.onRelease.RemoveListener(OnRelease);
        }
        DetenerSonido();
    }

    void OnGrab()
    {
        agarrado = true;
        completado = false;
        tiempoAcumulado = 0f;
        ResetearBarra();
        if (audioSourceLoop != null)
        {
            audioSourceLoop.loop = true;
            audioSourceLoop.Play();
        }
    }

    void OnRelease()
    {
        agarrado = false;
        tiempoAcumulado = 0f;
        ResetearBarra();
        DetenerSonido();
    }

    void Update()
    {
        if (!agarrado || completado || dialogPlayer == null) return;

        tiempoAcumulado += Time.deltaTime;
        float progreso = Mathf.Clamp01(tiempoAcumulado / tiempoRequerido);

        if (barraDeTiempo != null)
        {
            Vector3 escala = barraDeTiempo.transform.localScale;
            switch (ejeEscala)
            {
                case EjeEscala.X: escala.x = progreso; break;
                case EjeEscala.Y: escala.y = progreso; break;
                case EjeEscala.Z: escala.z = progreso; break;
            }
            barraDeTiempo.transform.localScale = escala;
        }

        if (progreso >= 1f)
        {
            completado = true;
            agarrado = false;
            DetenerSonido();
            if (dialogPlayer != null)
                dialogPlayer.Avanzar();
        }
    }

    void ResetearBarra()
    {
        if (barraDeTiempo != null)
        {
            Vector3 escala = barraDeTiempo.transform.localScale;
            switch (ejeEscala)
            {
                case EjeEscala.X: escala.x = 0f; break;
                case EjeEscala.Y: escala.y = 0f; break;
                case EjeEscala.Z: escala.z = 0f; break;
            }
            barraDeTiempo.transform.localScale = escala;
        }
    }

    void DetenerSonido()
    {
        if (audioSourceLoop != null && audioSourceLoop.isPlaying)
        {
            audioSourceLoop.loop = false;
            audioSourceLoop.Stop();
        }
    }
}
