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

    private bool desktopMode = false;
    private float cameraYaw = 0f;
    private float cameraPitch = 0f;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleMode();

        if (!desktopMode) return;

        if (Input.GetKey(KeyCode.RightArrow))
            cameraYaw += cameraRotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
            cameraYaw -= cameraRotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraPitch += cameraRotateSpeed * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraPitch -= cameraRotateSpeed * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
        }

        targetCamera.transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);

        Vector3 mouseOffset = Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        float handYaw = (mouseOffset.x / Screen.width) * 120f * mouseSensitivity;
        float handPitch = -(mouseOffset.y / Screen.height) * 120f * mouseSensitivity;
        handPitch = Mathf.Clamp(handPitch, -80f, 80f);

        Vector3 handPos = targetCamera.transform.position
            + targetCamera.transform.right * handOffset.x
            + targetCamera.transform.up * handOffset.y
            + targetCamera.transform.forward * handOffset.z;

        Quaternion handRot = targetCamera.transform.rotation * Quaternion.Euler(handPitch, handYaw, 0f);

        if (leftHand != null)
        {
            leftHand.desktopOverride = true;
            leftHand.desktopPosition = handPos;
            leftHand.desktopRotation = handRot;
            leftHand.desktopTrigger = Input.GetMouseButton(0);
        }
    }

    void ToggleMode()
    {
        desktopMode = !desktopMode;

        if (desktopMode)
        {
            cameraYaw = targetCamera.transform.eulerAngles.y;
            cameraPitch = targetCamera.transform.eulerAngles.x;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (leftHand != null) leftHand.desktopOverride = true;
            if (rightHand != null) rightHand.desktopOverride = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (leftHand != null) leftHand.desktopOverride = false;
            if (rightHand != null) rightHand.desktopOverride = false;
        }
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
            GUI.Label(new Rect(10, 30, 400, 30), "Flechas: rotar cámara | Mouse: mano | Click: gatillo", style);
        }
    }
}
