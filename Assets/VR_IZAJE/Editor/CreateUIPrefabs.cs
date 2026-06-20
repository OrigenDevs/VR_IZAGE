using UnityEngine;
using UnityEditor;
using TMPro;

public class CreateUIPrefabs
{
    [MenuItem("VR Izaje/Crear Prefab Panel Confirmacion 3D")]
    static void CreateConfirmPanelPrefab()
    {
        string path = "Assets/VR_IZAJE/PREFABS/UI/UI_ConfirmPanel3D.prefab";
        string dir = System.IO.Path.GetDirectoryName(path);
        System.IO.Directory.CreateDirectory(dir);

        GameObject root = new GameObject("ConfirmPanel3D");

        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bg.name = "Background";
        bg.transform.SetParent(root.transform);
        bg.transform.localPosition = new Vector3(0, 0, 0.5f);
        bg.transform.localScale = new Vector3(0.6f, 0.3f, 1);
        bg.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        bg.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 0, 0, 0.8f);

        GameObject textGO = new GameObject("MessageText");
        textGO.transform.SetParent(root.transform);
        textGO.transform.localPosition = new Vector3(0, 0.04f, 0.52f);
        textGO.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        var tmp = textGO.AddComponent<TextMeshPro>();
        tmp.text = "Presiona para continuar";
        tmp.fontSize = 2;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.rectTransform.sizeDelta = new Vector2(200, 50);

        GameObject btnGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        btnGO.name = "ConfirmButton";
        btnGO.transform.SetParent(root.transform);
        btnGO.transform.localPosition = new Vector3(0, -0.06f, 0.52f);
        btnGO.transform.localScale = new Vector3(0.1f, 0.04f, 0.02f);
        Object.DestroyImmediate(btnGO.GetComponent<Collider>());
        var boxCollider = btnGO.AddComponent<BoxCollider>();
        boxCollider.size = Vector3.one;

        GameObject btnLabel = GameObject.CreatePrimitive(PrimitiveType.Quad);
        btnLabel.name = "Label";
        btnLabel.transform.SetParent(btnGO.transform);
        btnLabel.transform.localPosition = Vector3.zero;
        btnLabel.transform.localScale = new Vector3(0.08f, 0.03f, 1);
        btnLabel.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        btnLabel.GetComponent<Renderer>().sharedMaterial.color = Color.green;

        var btn3D = btnGO.AddComponent<Button3D>();
        btn3D.hoverScale = 1.2f;
        btn3D.pressScale = 0.8f;

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);

        Debug.Log("Prefab creado en: " + path);
    }
}
