using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransicionVr : MonoBehaviour
{
    [SerializeField] private float duracionDefecto = 0.5f;
    [SerializeField] private Color colorFondo = Color.black;

    public GameObject objetoATransportar;
    public Transform destino;
    public AudioSource audioTransicion;

    private static TransicionVr instancia;
    private Image panelNegro;
    private Sequence secuenciaActual;

    void Awake()
    {
        instancia = this;
        CrearPanel();
    }

    void OnEnable()
    {
        ParpadearConTransporte(duracionDefecto, duracionDefecto);
    }

    void OnDisable()
    {
        secuenciaActual?.Kill();
        if (panelNegro != null)
            DOTween.Kill(panelNegro);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            ParpadearConTransporte(duracionDefecto, duracionDefecto);
    }

    void CrearPanel()
    {
        GameObject canvasObj = new GameObject("CanvasTransicion");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.transform.SetParent(transform);

        GameObject panelObj = new GameObject("PanelNegro");
        panelObj.transform.SetParent(canvasObj.transform);
        panelNegro = panelObj.AddComponent<Image>();
        panelNegro.color = new Color(colorFondo.r, colorFondo.g, colorFondo.b, 0f);
        panelNegro.raycastTarget = false;

        RectTransform rt = panelNegro.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public static void FundidoANegro(float duracion)
    {
        if (instancia == null || instancia.panelNegro == null) return;
        DOTween.Kill(instancia.panelNegro);
        instancia.panelNegro.DOFade(1f, duracion);
    }

    public static void AparecerDesdeNegro(float duracion)
    {
        if (instancia == null || instancia.panelNegro == null) return;
        DOTween.Kill(instancia.panelNegro);
        instancia.panelNegro.DOFade(0f, duracion);
    }

    public static void Parpadear(float duracionFundido, float espera)
    {
        if (instancia == null || instancia.panelNegro == null) return;
        Sequence seq = DOTween.Sequence();
        seq.Append(instancia.panelNegro.DOFade(1f, duracionFundido));
        seq.AppendInterval(espera);
        seq.Append(instancia.panelNegro.DOFade(0f, duracionFundido));
    }

    public static void ParpadearConTransporte()
    {
        ParpadearConTransporte(instancia.duracionDefecto, instancia.duracionDefecto);
    }

    public static void ParpadearConTransporte(float duracionFundido, float espera)
    {
        if (instancia == null || instancia.panelNegro == null) return;

        instancia.secuenciaActual?.Kill();
        DOTween.Kill(instancia.panelNegro);

        Sequence seq = DOTween.Sequence();
        instancia.secuenciaActual = seq;
        seq.Append(instancia.panelNegro.DOFade(1f, duracionFundido));
        seq.AppendCallback(() =>
        {
            if (instancia.objetoATransportar != null && instancia.destino != null)
                instancia.objetoATransportar.transform.SetPositionAndRotation(instancia.destino.position, instancia.destino.rotation);

            if (instancia.audioTransicion != null)
                instancia.audioTransicion.Play();
        });
        seq.AppendInterval(espera);
        seq.Append(instancia.panelNegro.DOFade(0f, duracionFundido));
        seq.OnComplete(() =>
        {
            if (instancia != null)
                instancia.gameObject.SetActive(false);
        });
    }
}
