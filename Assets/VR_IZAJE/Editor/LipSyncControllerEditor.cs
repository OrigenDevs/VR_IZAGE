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
                serializedObject.Update();
                Repaint();
            }
        }

        if (lipSync.mouthSprites != null)
        {
            if (lipSync.mouthSprites.Length > 0)
            {
                int valid = 0;
                foreach (var s in lipSync.mouthSprites) { if (s != null) valid++; }
                EditorGUILayout.LabelField($"Sprites: {lipSync.mouthSprites.Length} ({valid} válidos)", EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.LabelField("No se encontraron sprites. Verifica que el clip o la carpeta BOCA LOOP existan.", EditorStyles.helpBox);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Haz clic en 'Extraer Sprites del Clip' para cargar.", EditorStyles.helpBox);
        }
    }
}
