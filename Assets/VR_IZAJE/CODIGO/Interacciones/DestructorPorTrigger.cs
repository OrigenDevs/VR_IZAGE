using UnityEngine;

public class DestructorPorTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject[] objetosAActivar;

    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);

        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();

        foreach (var obj in objetosAActivar)
            if (obj != null) obj.SetActive(true);
    }
}
