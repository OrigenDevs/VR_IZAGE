using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LipSyncController))]
public class LipSyncControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LipSyncController lipSync = (LipSyncController)target;

        if (lipSync.mouthAnimationClip != null)
        {
            if (GUILayout.Button("Extraer Sprites del Clip"))
            {
                lipSync.PopulateSpritesFromClip();
                EditorUtility.SetDirty(lipSync);
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        if (lipSync.mouthSprites != null && lipSync.mouthSprites.Length > 0)
        {
            EditorGUILayout.LabelField("Sprites extraídos: " + lipSync.mouthSprites.Length, EditorStyles.helpBox);
        }
    }
}
