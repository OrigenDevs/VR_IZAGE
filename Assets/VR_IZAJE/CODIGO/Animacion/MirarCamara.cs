using UnityEngine;

public class MirarCamara : MonoBehaviour
{
    void Update()
    {
        Camera cam = Camera.main;
        if (cam != null)
            transform.LookAt(cam.transform);
    }
}
