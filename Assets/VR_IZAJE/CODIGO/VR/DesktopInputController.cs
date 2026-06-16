using UnityEngine;

public class DesktopInputController : MonoBehaviour
{
    public Camera targetCamera;
    public VRHandController leftHand;
    public VRHandController rightHand;

    public float mouseSensitivity = 0.8f;
    public float cameraRotateSpeed = 60f;
    public Vector3 handOffset = new Vector3(0.3f, -0.2f, 0.5f);

    public KeyCode toggleKey = KeyCode.P;
    public bool desktopMode = false;

    private bool wasDesktopMode = false;
    private float cameraYaw = 0f;
    private float cameraPitch = 0f;
    private float handYaw = 0f;
    private float handPitch = 0f;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            desktopMode = !desktopMode;

        if (desktopMode != wasDesktopMode)
        {
            wasDesktopMode = desktopMode;
            if (desktopMode) ActivateDesktopMode();
            else DeactivateDesktopMode();
        }

        if (!desktopMode) return;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            cameraYaw += cameraRotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            cameraYaw -= cameraRotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            cameraPitch -= cameraRotateSpeed * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            cameraPitch += cameraRotateSpeed * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
        }

        targetCamera.transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        handYaw += mouseX * 120f * mouseSensitivity * Time.deltaTime;
        handPitch -= mouseY * 120f * mouseSensitivity * Time.deltaTime;
        handPitch = Mathf.Clamp(handPitch, -80f, 80f);

        Vector3 handPos = targetCamera.transform.position
            + targetCamera.transform.right * handOffset.x
            + targetCamera.transform.up * handOffset.y
            + targetCamera.transform.forward * handOffset.z;

        Quaternion handRot = targetCamera.transform.rotation * Quaternion.Euler(handPitch, handYaw, 0f);

        if (rightHand != null)
        {
            rightHand.desktopOverride = true;
            rightHand.desktopPosition = handPos;
            rightHand.desktopRotation = handRot;
            rightHand.desktopTrigger = Input.GetMouseButton(0);
        }
    }

    void ActivateDesktopMode()
    {
        cameraYaw = targetCamera.transform.eulerAngles.y;
        float rawPitch = targetCamera.transform.eulerAngles.x;
        cameraPitch = rawPitch > 180f ? rawPitch - 360f : rawPitch;

        handYaw = 0f;
        handPitch = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (rightHand != null) rightHand.desktopOverride = true;
        if (leftHand != null) leftHand.desktopOverride = false;
    }

    void DeactivateDesktopMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (leftHand != null) leftHand.desktopOverride = false;
        if (rightHand != null) rightHand.desktopOverride = false;
    }

    void OnGUI()
    {
        if (!desktopMode)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(10, 10, 400, 30), "Presiona P para activar modo escritorio", style);
        }
        else
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.green;
            GUI.Label(new Rect(10, 10, 400, 30), "MODO ESCRITORIO | P: volver a VR", style);
            GUI.Label(new Rect(10, 30, 400, 30), "Flechas/W/S: cámara | Mouse: mano | Click: gatillo", style);
        }
    }
}
