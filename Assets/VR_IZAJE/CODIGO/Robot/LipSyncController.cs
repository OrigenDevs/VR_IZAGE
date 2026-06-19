using UnityEngine;

public class LipSyncController : MonoBehaviour
{
    public SpriteRenderer mouthRenderer;
    public Sprite[] mouthSprites;
    public AnimationClip mouthAnimationClip;
    [Range(0f, 0.1f)]
    public float volumeThreshold = 0.005f;
    public float updateInterval = 0.05f;

    private AudioSource audioSource;
    private float[] spectrum = new float[64];
    private float timer = 0f;
    private int currentFrame = 0;

    public void SetAudioSource(AudioSource source)
    {
        audioSource = source;
    }

    public void PopulateSpritesFromClip()
    {
#if UNITY_EDITOR
        if (mouthAnimationClip == null) return;

        var so = new UnityEditor.SerializedObject(mouthAnimationClip);
        var curvesProp = so.FindProperty("m_ObjectReferenceCurves");
        if (curvesProp == null) return;

        for (int i = 0; i < curvesProp.arraySize; i++)
        {
            var curveProp = curvesProp.GetArrayElementAtIndex(i);
            var bindingProp = curveProp.FindPropertyRelative("m_Binding");
            if (bindingProp == null) continue;

            var propName = bindingProp.FindPropertyRelative("m_PropertyName");
            if (propName == null || propName.stringValue != "m_Sprite") continue;

            var keyframesProp = curveProp.FindPropertyRelative("m_Keyframes");
            if (keyframesProp == null || keyframesProp.arraySize == 0) continue;

            var list = new System.Collections.Generic.List<Sprite>();
            for (int j = 0; j < keyframesProp.arraySize; j++)
            {
                var kfProp = keyframesProp.GetArrayElementAtIndex(j);
                var valProp = kfProp.FindPropertyRelative("value");
                if (valProp?.objectReferenceValue is Sprite s)
                    list.Add(s);
            }

            if (list.Count > 0)
            {
                mouthSprites = list.ToArray();
                break;
            }
        }
#endif
    }

    void Update()
    {
        if (mouthRenderer == null || mouthSprites == null || mouthSprites.Length == 0)
            return;

        if (audioSource == null || !audioSource.isPlaying)
        {
            mouthRenderer.sprite = mouthSprites[0];
            currentFrame = 0;
            return;
        }

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        float sum = 0f;
        for (int i = 0; i < spectrum.Length; i++)
            sum += spectrum[i];

        if (sum > volumeThreshold)
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % mouthSprites.Length;
                mouthRenderer.sprite = mouthSprites[currentFrame];
            }
        }
        else
        {
            mouthRenderer.sprite = mouthSprites[0];
            currentFrame = 0;
            timer = 0f;
        }
    }
}
