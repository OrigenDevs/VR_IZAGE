using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRHandController : MonoBehaviour
{
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color selectColor = Color.green;
    public Color triggerPressedColor = Color.cyan;
    public XRNode node = XRNode.LeftHand;

    public Color noDeviceColor = Color.red;
    public float laserMaxDistance = 10f;
    public float laserWidth = 0.01f;

    public Color laserColor = Color.red;
    public Color laserTriggerColor = Color.green;
    [Range(0f, 1f)]
    public float laserOpacity = 0.5f;

    public Transform reticle;
    public float reticleSize = 0.03f;
    public float reticleDistanceOffset = 0.02f;
    public bool reticleUseSphere = true;

    public Transform grabAttachPoint;

    public bool desktopOverride = false;
    [HideInInspector] public Vector3 desktopPosition;
    [HideInInspector] public Quaternion desktopRotation;
    [HideInInspector] public bool desktopTrigger;

    private Renderer handRenderer;
    private XRDirectInteractor interactor;
    private InputDevice device;
    private List<InputDevice> deviceBuffer = new List<InputDevice>();
    private int noDeviceFrameCount = 0;
    private LineRenderer laser;
    private Material laserMaterial;
    private Transform reticleInstance;
    private Renderer reticleRenderer;
    private bool triggerPressed = false;
    private bool triggerWasPressed = false;
    private bool hitObject = false;
    private RaycastHit lastHit;
    private SimpleGrab grabbedObject;
    private Button3D hoveredButton;

    void Start()
    {
        handRenderer = GetComponentInChildren<Renderer>();
        interactor = GetComponent<XRDirectInteractor>();
        if (interactor != null)
            interactor.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;

        laser = GetComponent<LineRenderer>();
        if (laser == null)
            laser = gameObject.AddComponent<LineRenderer>();
        laser.positionCount = 2;
        laser.startWidth = laserWidth;
        laser.endWidth = laserWidth * 0.5f;
        laserMaterial = new Material(Shader.Find("Unlit/Color"));
        laser.material = laserMaterial;
        laser.enabled = true;

        if (reticle == null)
        {
            PrimitiveType type = reticleUseSphere ? PrimitiveType.Sphere : PrimitiveType.Quad;
            GameObject rt = GameObject.CreatePrimitive(type);
            rt.name = "Reticle (" + node + ")";
            DestroyImmediate(rt.GetComponent<Collider>());
            rt.transform.localScale = Vector3.one * reticleSize;
            reticleInstance = rt.transform;
            reticleRenderer = rt.GetComponent<Renderer>();
            reticleRenderer.material = new Material(Shader.Find("Unlit/Color"));
        }
        else
        {
            reticleInstance = reticle;
            reticleRenderer = reticle.GetComponent<Renderer>();
        }
        reticleInstance.gameObject.SetActive(true);

        FindDevice();
        InputDevices.deviceConnected += OnDeviceConnected;
    }

    void OnDeviceConnected(InputDevice _)
    {
        if (!device.isValid) FindDevice();
    }

    void FindDevice()
    {
        deviceBuffer.Clear();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, deviceBuffer);
        foreach (var d in deviceBuffer)
        {
            if (node == XRNode.LeftHand && d.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            { device = d; return; }
            if (node == XRNode.RightHand && d.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            { device = d; return; }
        }
        if (deviceBuffer.Count > 0 && !device.isValid)
            device = deviceBuffer[0];
    }

    void Update()
    {
        if (desktopOverride)
        {
            noDeviceFrameCount = 0;
            transform.position = desktopPosition;
            transform.rotation = desktopRotation;
            triggerPressed = desktopTrigger;
            if (interactor != null)
                interactor.selectInput.manualPerformed = desktopTrigger;
            UpdateLaser();
            HandleGrab();
            UpdateHandColor();
            return;
        }

        if (!device.isValid) FindDevice();

        triggerPressed = false;
        hitObject = false;

        if (device.isValid)
        {
            noDeviceFrameCount = 0;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out var rot))
                transform.localRotation = rot;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out var pos))
                transform.localPosition = pos;

            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out var pressed))
            {
                triggerPressed = pressed;
                if (interactor != null)
                    interactor.selectInput.manualPerformed = pressed;
            }

            UpdateLaser();
        }
        else
        {
            noDeviceFrameCount++;
            laser.enabled = false;
            if (reticleInstance != null)
                reticleInstance.gameObject.SetActive(false);
            if (grabbedObject != null)
            {
                grabbedObject.Release();
                grabbedObject = null;
            }
        }

        HandleGrab();
        UpdateHandColor();
    }

    void UpdateLaser()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Vector3 endPoint = origin + direction * laserMaxDistance;
        hitObject = Physics.Raycast(origin, direction, out lastHit, laserMaxDistance);
        if (hitObject)
            endPoint = lastHit.point;

        laser.SetPosition(0, origin);
        laser.SetPosition(1, endPoint);
        laser.enabled = true;

        Color col = triggerPressed ? laserTriggerColor : laserColor;
        col.a = laserOpacity;
        laser.startColor = col;
        laser.endColor = col;
        laserMaterial.color = col;

        if (reticleInstance != null)
        {
            reticleInstance.gameObject.SetActive(true);
            reticleInstance.position = endPoint + direction * reticleDistanceOffset;
            if (reticleUseSphere)
                reticleInstance.rotation = Quaternion.identity;
            else
                reticleInstance.rotation = Quaternion.LookRotation(-direction);
            if (reticleRenderer != null)
            {
                Color rc = col;
                rc.a = 1f;
                reticleRenderer.material.color = rc;
            }
        }
    }

    void HandleGrab()
    {
        bool triggerJustPressed = triggerPressed && !triggerWasPressed;
        bool triggerJustReleased = !triggerPressed && triggerWasPressed;
        triggerWasPressed = triggerPressed;

        SimpleGrab sg = null;
        Button3D btn = null;

        if (hitObject && lastHit.collider != null)
        {
            sg = lastHit.collider.GetComponentInParent<SimpleGrab>();
            if (sg == null)
                btn = lastHit.collider.GetComponentInParent<Button3D>();
        }

        Button3D newHovered = (sg != null || grabbedObject != null) ? null : btn;
        if (newHovered != hoveredButton)
        {
            if (hoveredButton != null) hoveredButton.OnHoverEnd();
            if (newHovered != null) newHovered.OnHoverStart();
            hoveredButton = newHovered;
        }

        if (triggerJustPressed)
        {
            if (sg != null && grabAttachPoint != null)
            {
                if (grabbedObject != null && grabbedObject != sg)
                    grabbedObject.Release();
                grabbedObject = sg;
                grabbedObject.Grab(grabAttachPoint);
            }
            else if (hoveredButton != null)
            {
                hoveredButton.OnPress();
            }
        }
        else if (triggerJustReleased)
        {
            if (grabbedObject != null)
            {
                grabbedObject.Release();
                grabbedObject = null;
            }
            if (hoveredButton != null)
            {
                hoveredButton.OnRelease();
            }
        }
    }

    void UpdateHandColor()
    {
        if (handRenderer == null) return;

        Color color;
        if (!device.isValid && noDeviceFrameCount > 30)
            color = noDeviceColor;
        else if (triggerPressed && hitObject && interactor != null && interactor.interactablesSelected.Count > 0)
            color = selectColor;
        else if (triggerPressed)
            color = triggerPressedColor;
        else if (interactor != null && interactor.interactablesSelected.Count > 0)
            color = selectColor;
        else if (interactor != null && interactor.hasHover)
            color = hoverColor;
        else
            color = defaultColor;

        handRenderer.material.color = color;
    }

    void OnDestroy()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
        if (grabbedObject != null)
        {
            grabbedObject.Release();
            grabbedObject = null;
        }
        if (hoveredButton != null)
        {
            hoveredButton.OnHoverEnd();
            hoveredButton = null;
        }
    }
}
