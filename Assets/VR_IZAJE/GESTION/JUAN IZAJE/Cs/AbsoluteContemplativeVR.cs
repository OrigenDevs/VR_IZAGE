using UnityEngine;
using UnityEngine.InputSystem;

public class AbsoluteContemplativeVR : MonoBehaviour
{
    [Header("XR Input Reference")]
    [Tooltip("Asigna aquí la acción de rotación del HMD (ej. XRI HMD/Rotation).")]
    [SerializeField] private InputActionProperty hmdRotationAction;

    [Header("Anchor Settings")]
    [Tooltip("El objeto vacío que representa los ojos del conductor dentro de la cabina.")]
    [SerializeField] private Transform driverSeatAnchor;

    private void OnEnable()
    {
        if (hmdRotationAction.action != null)
        {
            hmdRotationAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (hmdRotationAction.action != null)
        {
            hmdRotationAction.action.Disable();
        }
    }

    private void LateUpdate()
    {
        // 1. Forzar posición absoluta en el asiento (bloquea traslación del simulador o del tracking físico)
        if (driverSeatAnchor != null)
        {
            transform.position = driverSeatAnchor.position;
        }

        // 2. Aplicar únicamente la rotación de la cabeza
        if (hmdRotationAction.action != null)
        {
            Quaternion hmdRotation = hmdRotationAction.action.ReadValue<Quaternion>();
            transform.rotation = hmdRotation;
        }
    }
}