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
        var bindings = UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(mouthAnimationClip);
        foreach (var binding in bindings)
        {
            if (binding.propertyName == "m_Sprite")
            {
                var keyframes = UnityEditor.AnimationUtility.GetObjectReferenceCurve(mouthAnimationClip, binding);
                mouthSprites = new Sprite[keyframes.Length];
                for (int i = 0; i < keyframes.Length; i++)
                    mouthSprites[i] = keyframes[i].value as Sprite;
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
