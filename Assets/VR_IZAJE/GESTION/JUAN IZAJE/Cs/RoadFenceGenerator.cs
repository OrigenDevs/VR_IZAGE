using UnityEngine;
using System.Collections.Generic;

public class CustomCurveFenceGenerator : MonoBehaviour
{
    public enum ModelAxis { X_Positive, X_Negative, Z_Positive, Z_Negative }

    [Header("Nodos de la Curva")]
    public List<Transform> curveNodes = new List<Transform>();

    [Header("Configuración de la Valla")]
    public GameObject fencePrefab;
    [Tooltip("Longitud real en metros de la baranda de un solo tramo.")]
    public float fenceLength = 3.0f;
    [Tooltip("Desplazamiento lateral respecto a la línea de los nodos.")]
    public float lateralOffset = 0f;

    [Header("Alineación del Eje del Modelo")]
    [Tooltip("¿Qué eje de tu modelo sigue el largo de la baranda? Para tu modelo actual, usa X_Positive.")]
    public ModelAxis directionAxis = ModelAxis.X_Positive;

    [Header("Ajustes de Transformación")]
    public Vector3 fenceRotationOffset = Vector3.zero;
    public Vector3 fenceScaleMultiplier = Vector3.one;

    [Header("Visualización del Spline")]
    public Color gizmoColor = Color.cyan;
    [Range(20, 200)]
    public int previewSegments = 100;

    private const string HOLDER_NAME = "Fences_Holder";

    [ContextMenu("Generar Vallas")]
    public void GenerateFences()
    {
        ClearFences();
        curveNodes.RemoveAll(node => node == null);

        if (curveNodes.Count < 2)
        {
            Debug.LogWarning("Necesitas al menos 2 nodos válidos.");
            return;
        }
        if (fencePrefab == null)
        {
            Debug.LogError("Asigna un prefab de valla primero.");
            return;
        }

        Transform holder = transform.Find(HOLDER_NAME);
        if (holder == null)
        {
            holder = new GameObject(HOLDER_NAME).transform;
            holder.SetParent(transform);
            holder.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        List<Vector3> smoothPath = GenerateBezierPath();
        if (smoothPath.Count < 2) return;

        float targetDistance = fenceLength;
        float currentDistance = 0f;
        Vector3 currentStartPoint = smoothPath[0];

        for (int i = 0; i < smoothPath.Count - 1; i++)
        {
            Vector3 pA = smoothPath[i];
            Vector3 pB = smoothPath[i + 1];
            float segmentLength = Vector3.Distance(pA, pB);

            while (currentDistance + segmentLength >= targetDistance)
            {
                float remainingDistance = targetDistance - currentDistance;
                float t = remainingDistance / segmentLength;
                Vector3 currentEndPoint = Vector3.Lerp(pA, pB, t);

                Vector3 fenceDirection = (currentEndPoint - currentStartPoint).normalized;

                // El pivote de tu modelo está centrado en X, Y, Z, por lo que spawneamos en el punto medio exacto
                Vector3 spawnPosition = (currentStartPoint + currentEndPoint) * 0.5f;

                // Aplicamos el offset lateral perpendicular
                Vector3 rightDir = Quaternion.Euler(0, 90, 0) * fenceDirection;
                spawnPosition += rightDir * lateralOffset;

                SpawnFence(spawnPosition, fenceDirection, holder);

                currentStartPoint = currentEndPoint;
                pA = currentEndPoint;
                segmentLength = Vector3.Distance(pA, pB);
                currentDistance = 0f;
            }

            currentDistance += segmentLength;
        }

        Debug.Log("¡Vallas alineadas milimétricamente usando el eje correcto!");
    }

    private void SpawnFence(Vector3 position, Vector3 direction, Transform holderParent)
    {
        // Calculamos la rotación base para que el eje correcto del modelo apunte hacia la calle
        Quaternion baseLookRotation = Quaternion.identity;

        switch (directionAxis)
        {
            case ModelAxis.X_Positive:
                baseLookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);
                break;
            case ModelAxis.X_Negative:
                baseLookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
                break;
            case ModelAxis.Z_Positive:
                baseLookRotation = Quaternion.LookRotation(direction);
                break;
            case ModelAxis.Z_Negative:
                baseLookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
                break;
        }

        Quaternion offsetRotation = Quaternion.Euler(fenceRotationOffset);
        Quaternion finalRotation = baseLookRotation * offsetRotation;

        GameObject newFence = Instantiate(fencePrefab, position, finalRotation, holderParent);
        newFence.transform.localScale = Vector3.Scale(newFence.transform.localScale, fenceScaleMultiplier);
    }

    private List<Vector3> GenerateBezierPath()
    {
        List<Vector3> points = new List<Vector3>();
        int nodeCount = curveNodes.Count;

        if (nodeCount == 2)
        {
            for (int i = 0; i <= previewSegments; i++)
            {
                float t = (float)i / previewSegments;
                points.Add(Vector3.Lerp(curveNodes[0].position, curveNodes[1].position, t));
            }
            return points;
        }

        for (int i = 0; i < nodeCount - 1; i++)
        {
            Vector3 p0 = curveNodes[Mathf.Max(i - 1, 0)].position;
            Vector3 p1 = curveNodes[i].position;
            Vector3 p2 = curveNodes[i + 1].position;
            Vector3 p3 = curveNodes[Mathf.Min(i + 2, nodeCount - 1)].position;

            int subdivisions = previewSegments / (nodeCount - 1);
            for (int j = 0; j <= subdivisions; j++)
            {
                float t = (float)j / subdivisions;
                Vector3 interpolatedPoint = GetCatmullRomPosition(t, p0, p1, p2, p3);

                if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], interpolatedPoint) > 0.01f)
                {
                    points.Add(interpolatedPoint);
                }
            }
        }
        return points;
    }

    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    [ContextMenu("Limpiar Vallas")]
    public void ClearFences()
    {
        Transform holder = transform.Find(HOLDER_NAME);
        if (holder != null) DestroyImmediate(holder.gameObject);
    }

    private void OnDrawGizmos()
    {
        curveNodes.RemoveAll(node => node == null);
        if (curveNodes.Count < 2) return;

        Gizmos.color = gizmoColor;
        List<Vector3> path = GenerateBezierPath();

        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }

        Gizmos.color = Color.red;
        foreach (var node in curveNodes)
        {
            Gizmos.DrawSphere(node.position, 0.2f);
        }
    }
}