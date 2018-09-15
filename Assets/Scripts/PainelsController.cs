using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(FirebaseController))]
public class PainelsController : MonoBehaviour
{
    public RectTransform displayPainel, channelPainel, chatPainel;
    public Vector2 screenResolution;
    public CanvasScaler canvasScaler;
    [Range(0.1f, 1f)]
    public float timePainel;

    [Space(5)]
    [Header("Texts")]
    public TMP_Text username;

    [Space(5)]
    [Header("Inputs")]
    public TMP_InputField displayInput;

    [Space(5)]
    [Header("Buttons")]
    public Button getStartBtn;
    public Button signOutBtn;

    private FirebaseController firebaseController;

    private void Start()
    {
        Screen.fullScreen = false;
        firebaseController = GetComponent<FirebaseController>();
        screenResolution = displayPainel.rect.size;
        channelPainel.localPosition = new Vector3(channelPainel.rect.size.x, 0, 0);
        chatPainel.localPosition = new Vector3(chatPainel.rect.size.x, 0, 0);

        getStartBtn.onClick.AddListener(delegate 
        {
            if (!string.IsNullOrEmpty(displayInput.text))
            {
                firebaseController.AuthenticateAnonymouslyUser();
                username.text = displayInput.text;
                displayInput.text = "";
            }
        });

        signOutBtn.onClick.AddListener(delegate 
        {
            firebaseController.SignOutAplication();
            ClosePainel(channelPainel);
        });
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
