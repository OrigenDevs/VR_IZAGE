using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationTimelineEntry))]
public class AnimationTimelineDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var timeProp = property.FindPropertyRelative("normalizedTime");
        var clipProp = property.FindPropertyRelative("clip");
        float time = timeProp.floatValue;

        float barLeft = position.x + 55;
        float barWidth = position.width - 55;
        float barHeight = 14;
        float barY = position.y + 2;

        Rect bgRect = new Rect(barLeft, barY, barWidth, barHeight);
        Rect labelRect = new Rect(position.x, barY, 50, barHeight);
        Rect markerRect = new Rect(barLeft + barWidth * time - 4, barY - 1, 8, barHeight + 2);
        Rect sliderRect = new Rect(barLeft, barY, barWidth, barHeight);
        Rect clipRect = new Rect(barLeft, barY + barHeight + 2, barWidth, 16);

        GUI.Label(labelRect, (time * 100f).ToString("F0") + "%");

        Color bg = EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.7f, 0.7f, 0.7f);
        Color track = EditorGUIUtility.isProSkin ? new Color(0.35f, 0.35f, 0.35f) : new Color(0.5f, 0.5f, 0.5f);
        Color marker = new Color(0.2f, 0.7f, 1f);

        EditorGUI.DrawRect(bgRect, bg);
        EditorGUI.DrawRect(new Rect(barLeft, barY + barHeight / 2 - 1, barWidth, 2), track);
        EditorGUI.DrawRect(markerRect, marker);

        EditorGUI.BeginChangeCheck();
        float newTime = GUI.HorizontalSlider(sliderRect, time, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            timeProp.floatValue = newTime;
        }

        EditorGUI.PropertyField(clipRect, clipProp, GUIContent.none);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 38;
    }
}
