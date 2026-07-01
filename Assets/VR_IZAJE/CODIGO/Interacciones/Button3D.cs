using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    public float hoverScale = 1.2f;
    public float hoverSpeed = 5f;
    public float pressScale = 0.8f;
    public float pressSpeed = 8f;
    public float releaseSpeed = 5f;
    public AudioSource audioSource;
    public UnityEvent onClick;

    private Vector3 originalScale;
    private float currentMultiplier = 1f;
    private bool isHovered = false;
    private bool isPressed = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnHoverStart()
    {
        isHovered = true;
    }

    public void OnHoverEnd()
    {
        isHovered = false;
    }

    public void OnPress()
    {
        isPressed = true;
    }

    public void OnRelease()
    {
        isPressed = false;
        if (audioSource != null)
            audioSource.Play();
        onClick.Invoke();
    }

    void Update()
    {
        float target;
        float speed;

        if (isPressed)
        {
            target = pressScale;
            speed = pressSpeed;
        }
        else if (isHovered)
        {
            target = hoverScale;
            speed = hoverSpeed;
        }
        else
        {
            target = 1f;
            speed = releaseSpeed;
        }

        currentMultiplier = Mathf.Lerp(currentMultiplier, target, Time.deltaTime * speed);
        if (Mathf.Abs(currentMultiplier - target) < 0.001f)
            currentMultiplier = target;

        transform.localScale = originalScale * currentMultiplier;
    }
}
