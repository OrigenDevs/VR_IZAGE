using UnityEditor;
using UnityEngine;

public class SimpleVRSetup
{
    [MenuItem("VR Tools/Setup Rapido %#v")]
    static void SetupRapido()
    {
        var origin = Object.FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
        if (origin == null)
        {
            EditorUtility.DisplayDialog("Error", "No hay XR Origin. Crea uno desde GameObject > XR > XR Origin.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(origin.gameObject, "Setup VR Rapido");
        var camOffset = origin.CameraFloorOffsetObject;

        ConfigurarControl(camOffset.transform, "Left Controller", UnityEngine.XR.XRNode.LeftHand);
        ConfigurarControl(camOffset.transform, "Right Controller", UnityEngine.XR.XRNode.RightHand);

        if (Object.FindFirstObjectByType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>() == null)
        {
            var mgr = new GameObject("XR Interaction Manager");
            Undo.RegisterCreatedObjectUndo(mgr, "Crear Manager");
            mgr.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
        }

        Debug.Log("Setup VR Rapido completado!");
    }

    static void ConfigurarControl(Transform parent, string nombre, UnityEngine.XR.XRNode node)
    {
        var t = parent.Find(nombre);
        if (t == null)
        {
            var go = new GameObject(nombre);
            Undo.RegisterCreatedObjectUndo(go, "Crear " + nombre);
            t = go.transform;
            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }

        var g = t.gameObject;
        var hand = g.GetComponent<VRHandController>();
        if (hand == null) hand = g.AddComponent<VRHandController>();
        hand.node = node;

        var di = g.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (di == null)
        {
            di = g.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
            var sc = g.GetComponent<SphereCollider>();
            if (sc == null) sc = g.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.15f;
            var rb = g.GetComponent<Rigidbody>();
            if (rb == null) rb = g.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (t.childCount == 0 || t.GetChild(0).name != "HandModel")
        {
            var model = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            model.name = "HandModel";
            Undo.RegisterCreatedObjectUndo(model, "Crear HandModel");
            model.transform.SetParent(t);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.Euler(90, 0, 0);
            model.transform.localScale = new Vector3(0.03f, 0.08f, 0.03f);
        }
    }
}