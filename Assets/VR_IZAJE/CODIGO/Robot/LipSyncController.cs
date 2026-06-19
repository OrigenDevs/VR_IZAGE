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
        var curvesProp = so.FindProperty("m_PPtrCurves");
        if (curvesProp != null)
        {
            for (int i = 0; i < curvesProp.arraySize; i++)
            {
                var elem = curvesProp.GetArrayElementAtIndex(i);
                var attrProp = elem.FindPropertyRelative("attribute");
                if (attrProp == null || attrProp.stringValue != "m_Sprite") continue;

                var curveProp = elem.FindPropertyRelative("curve");
                if (curveProp == null || curveProp.arraySize == 0) continue;

                var list = new System.Collections.Generic.List<Sprite>();
                for (int j = 0; j < curveProp.arraySize; j++)
                {
                    var valProp = curveProp.GetArrayElementAtIndex(j).FindPropertyRelative("value");
                    if (valProp?.objectReferenceValue is Sprite s)
                        list.Add(s);
                }

                if (list.Count > 0)
                {
                    mouthSprites = list.ToArray();
                    return;
                }
            }
        }

        string clipPath = UnityEditor.AssetDatabase.GetAssetPath(mouthAnimationClip);
        string clipDir = System.IO.Path.GetDirectoryName(clipPath);
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new[] { clipDir + "/BOCA LOOP" });
        if (guids.Length > 0)
        {
            var loaded = new System.Collections.Generic.List<Sprite>();
            foreach (string guid in guids)
            {
                var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                if (sprite != null)
                    loaded.Add(sprite);
            }
            loaded.Sort((a, b) => UnityEditor.EditorUtility.NaturalCompare(a.name, b.name));
            if (loaded.Count > 0)
                mouthSprites = loaded.ToArray();
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
