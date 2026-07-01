using UnityEngine;
using UnityEngine.Events;

public class SimpleGrab : MonoBehaviour
{
    public UnityEvent onGrab;
    public UnityEvent onRelease;
    public float grabSpeed = 8f;
    public float releaseSpeed = 4f;
    public bool faceCamera = true;
    public float faceCameraSpeed = 10f;
    public bool cambiarPosicionBase;
    public Transform nuevaPosicionBase;
    public AudioClip grabSound;
    public AudioClip releaseSound;

    private Transform grabTarget;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Rigidbody rb;
    private bool rbWasKinematic;
    private bool isGrabbed = false;
    private bool movingToHand = false;
    private bool movingToOriginal = false;
    private AudioSource audioSource;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rbWasKinematic = rb.isKinematic;
            rb.useGravity = false;
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Grab(Transform target)
    {
        if (target == null || isGrabbed) return;
        isGrabbed = true;
        movingToHand = true;
        movingToOriginal = false;
        grabTarget = target;
        transform.SetParent(null);
        if (cambiarPosicionBase && nuevaPosicionBase != null)
        {
            originalPosition = nuevaPosicionBase.position;
            originalRotation = nuevaPosicionBase.rotation;
            originalParent = nuevaPosicionBase.parent;
        }
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        if (grabSound != null)
            audioSource.PlayOneShot(grabSound);
        onGrab.Invoke();
    }

    public void Release()
    {
        if (!isGrabbed) return;
        isGrabbed = false;
        movingToOriginal = true;
        movingToHand = false;
        transform.SetParent(null);
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        if (releaseSound != null)
            audioSource.PlayOneShot(releaseSound);
        onRelease.Invoke();
    }

    void Update()
    {
        if (movingToHand && grabTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, grabTarget.position, Time.deltaTime * grabSpeed);
            if (faceCamera)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    Vector3 dir = cam.transform.position - transform.position;
                    if (dir != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(dir);
                }
            }
            if (Vector3.Distance(transform.position, grabTarget.position) < 0.005f)
            {
                movingToHand = false;
                Quaternion currentRot = transform.rotation;
                transform.SetParent(grabTarget);
                transform.localPosition = Vector3.zero;
                transform.rotation = currentRot;
                if (rb != null) rb.isKinematic = true;
            }
        }
        else if (movingToOriginal)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * releaseSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * releaseSpeed);
            if (Vector3.Distance(transform.position, originalPosition) < 0.005f)
            {
                movingToOriginal = false;
                transform.position = originalPosition;
                transform.rotation = originalRotation;
                transform.SetParent(originalParent);
                if (rb != null)
                {
                    rb.isKinematic = rbWasKinematic;
                    rb.useGravity = !rbWasKinematic;
                }
            }
        }

        if (isGrabbed && !movingToHand && !movingToOriginal && faceCamera)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 dir = cam.transform.position - transform.position;
                if (dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * faceCameraSpeed);
                }
            }
        }
    }
}
