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
    private bool triggerPressed = false;
    private bool hitObject = false;

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
        laser.material = new Material(Shader.Find("Unlit/Color"));
        laser.enabled = false;

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
        }

        UpdateHandColor();
    }

    void UpdateLaser()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit hit;
        Vector3 endPoint = origin + direction * laserMaxDistance;
        hitObject = Physics.Raycast(origin, direction, out hit, laserMaxDistance);
        if (hitObject)
            endPoint = hit.point;

        laser.SetPosition(0, origin);
        laser.SetPosition(1, endPoint);
        laser.enabled = triggerPressed;

        Color laserColor = triggerPressed
            ? (hitObject ? selectColor : triggerPressedColor)
            : defaultColor;
        laser.startColor = laserColor;
        laser.endColor = laserColor;
        laser.material.color = laserColor;
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
    }
}
