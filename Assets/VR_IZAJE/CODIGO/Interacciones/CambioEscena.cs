using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public string nombreEscena;

    public void IrAEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
    }
}
