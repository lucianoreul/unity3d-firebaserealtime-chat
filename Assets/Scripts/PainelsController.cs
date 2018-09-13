using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PainelsController : MonoBehaviour
{
    public RectTransform displayPainel, channelPainel, chatPainel;
    public Vector2 screenResolution;
    public CanvasScaler canvasScaler;
    [Range(0.1f,1f)]
    public float timePainel;

    private void Start()
    {
        screenResolution = displayPainel.rect.size;
        channelPainel.localPosition = new Vector3(channelPainel.rect.size.x, 0, 0);
        chatPainel.localPosition = new Vector3(chatPainel.rect.size.x, 0, 0);
    }

    public void OpenPainel(RectTransform obj)
    {
        obj.DOAnchorPos(Vector2.zero, timePainel, false);
    }

    public void ClosePainel(RectTransform obj)
    {
        obj.DOAnchorPos(new Vector2(screenResolution.x, 0), timePainel, false);
    }

}
