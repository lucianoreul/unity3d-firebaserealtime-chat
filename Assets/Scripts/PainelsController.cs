using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

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

    [Space(5)]
    [Header("content")]
    public GameObject content;

    [Space(5)]
    [Header("prefabs")]
    public GameObject itemChannelPrefab;

    [Space(5)]
    [Header("Lists")]
    public List<GameObject> channelItens = new List<GameObject>();
    public List<Channel> channels = new List<Channel>();

    private FirebaseController firebaseController;

    private void Start()
    {
        ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.VisibleOverContent;
        ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xff20B2AA;

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
            ClearAplication();
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

    public void UpdateChannelsFromDataBase()
    {
        firebaseController.GetChannelsDatabase(channels);
    }

    public void SpawnChannelsButtons()
    {
        for(int i = 0; i < channels.Count; i++)
        {
            var button = Instantiate(itemChannelPrefab, content.transform, false);
            var buttonComp = button.GetComponent<ChannelItem>();
            buttonComp.channelReference = channels[i];
            buttonComp.SetChannelInfos();
            buttonComp.channelNumber.text = i.ToString();
            channelItens.Add(button);
        }
    }

    public void ClearAplication()
    {
        foreach(var item in channelItens)
        {
            Destroy(item);
        }
        channelItens.Clear();
    }

}
