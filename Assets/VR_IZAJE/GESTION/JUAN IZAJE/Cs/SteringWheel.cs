using UnityEngine;

public class SteeringWheelLimiterVec : MonoBehaviour
{
    public enum SteeringAxis { X, Y, Z }

    [Header("References")]
    [Tooltip("El objeto padre que se mueve con la Spline.")]
    [SerializeField] private Transform truckTransform;

    [Header("Steering Settings")]
    [Tooltip("¿Qué eje local debe girar para simular la dirección del volante?")]
    [SerializeField] private SteeringAxis steeringAxis = SteeringAxis.Z;

    [Tooltip("Factor de división. Más alto = el volante gira menos en las curvas.")]
    [SerializeField] private float rotationDivider = 2f;

    [Tooltip("Ángulo máximo que puede girar el volante a cada lado (grados).")]
    [SerializeField] private float maxSteeringAngle = 60f;

    [Tooltip("Suavizado de retorno y movimiento.")]
    [SerializeField] private float smoothSpeed = 5f;

    private float currentWheelRotation = 0f;
    private Vector3 lastTruckForward;
    private Quaternion initialLocalRotation;

    private void Start()
    {
        if (truckTransform != null)
        {
            lastTruckForward = truckTransform.forward;
        }

        // ¡ESTA ES LA CLAVE! Guardamos la inclinación exacta que le diste en el Inspector
        initialLocalRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        if (truckTransform == null) return;

        // 1. Calcular el Delta de giro del camión usando su propio eje UP
        Vector3 currentForward = truckTransform.forward;
        float deltaRotation = Vector3.SignedAngle(lastTruckForward, currentForward, truckTransform.up);
        lastTruckForward = currentForward;

        // 2. Acumular el giro dividido
        if (Mathf.Abs(deltaRotation) > 0.001f)
        {
            currentWheelRotation += (deltaRotation / rotationDivider);
        }
        else
        {
            // Retorno al centro suave si va recto
            currentWheelRotation = Mathf.MoveTowards(currentWheelRotation, 0f, Time.deltaTime * smoothSpeed * 8f);
        }

        // Limitar para proteger los brazos del IK
        currentWheelRotation = Mathf.Clamp(currentWheelRotation, -maxSteeringAngle, maxSteeringAngle);

        // 3. Crear el Quaternion de giro SOLO para el eje seleccionado
        Vector3 steeringVector = Vector3.zero;
        switch (steeringAxis)
        {
            case SteeringAxis.X: steeringVector.x = currentWheelRotation; break;
            case SteeringAxis.Y: steeringVector.y = currentWheelRotation; break;
            case SteeringAxis.Z: steeringVector.z = currentWheelRotation; break;
        }
        Quaternion extraRotation = Quaternion.Euler(steeringVector);

        // 4. COMBINAR: Mantener la inclinación inicial y multiplicarle el giro local
        // Esto evita que los otros ejes se alteren o se reseteen a 0
        Quaternion targetLocalRotation = initialLocalRotation * extraRotation;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRotation, Time.deltaTime * smoothSpeed);
    }
}