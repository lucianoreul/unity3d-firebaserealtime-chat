using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(FirebaseController))]
public class PainelsController : MonoBehaviour
{
    public RectTransform displayPainel, channelPainel, chatPainel, createrChannelPainel;
    public Vector2 screenResolution;
    public CanvasScaler canvasScaler;
    [Range(0.1f, 1f)]
    public float timePainel;

    // control variable
    private Vector2 createrChannelPainelPos;
    private bool createrChannelPainelISOpen = false;

    [HideInInspector]
    public Channel channelReferenceSelected;

    [Space(5)]
    [Header("Texts")]
    public TMP_Text userName;
    public TMP_Text chatName;

    [Space(5)]
    [Header("Inputs")]
    public TMP_InputField displayInput;
    public TMP_InputField newChannelName;
    public TMP_InputField chatInput;

    [Space(5)]
    [Header("Buttons")]
    public Button getStartBtn;
    public Button signOutBtn;
    public Button openCreateChannelBtn;
    public Button createChannelBtn;
    public Button refreshChannelBtn;
    public Button sendMessage;
    public Button returnChat;

    [Space(5)]
    [Header("content")]
    public GameObject channelContent;
    public GameObject chatContent;

    [Space(5)]
    [Header("prefabs")]
    public GameObject itemChannelPrefab;

    [Space(5)]
    [Header("Lists")]
    [HideInInspector]
    public List<GameObject> channelItens = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> messageItens = new List<GameObject>();
    [HideInInspector]
    public List<string> messageItensTeste = new List<string>();
    [HideInInspector]
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
        createrChannelPainelPos = new Vector2(0, createrChannelPainel.rect.size.y);

        ConfigButtons();
    }

    private void OnEnable()
    {
        ChannelItem.OnClickOpen += OpenChatPainel;
    }

    public void ConfigButtons()
    {
        getStartBtn.onClick.AddListener(delegate
        {
            if (!string.IsNullOrEmpty(displayInput.text))
            {
                firebaseController.AuthenticateAnonymouslyUser();
                userName.text = displayInput.text;
                displayInput.text = "";
            }
        });

        signOutBtn.onClick.AddListener(delegate
        {
            firebaseController.SignOutAplication();
            ClosePainel(channelPainel, true);
            ClearAplication();
        });

        openCreateChannelBtn.onClick.AddListener(delegate 
        {
            if (!createrChannelPainelISOpen)
                createrChannelPainel.DOAnchorPos(createrChannelPainelPos, timePainel, false);
            else
                createrChannelPainel.DOAnchorPos(Vector2.zero, timePainel, false);
            createrChannelPainelISOpen = !createrChannelPainelISOpen;
        });

        createChannelBtn.onClick.AddListener(delegate
        {
            if (!string.IsNullOrEmpty(newChannelName.text))
            {
                firebaseController.CreaterChannelDataBase(newChannelName.text, userName.text);
                newChannelName.text = "";
                firebaseController.GetChannelsDatabase(channels);
                createrChannelPainel.DOAnchorPos(Vector2.zero, timePainel, false);
                createrChannelPainelISOpen = !createrChannelPainelISOpen;
            }
        });

        refreshChannelBtn.onClick.AddListener(delegate
        {
            firebaseController.GetChannelsDatabase(channels);
        });

        sendMessage.onClick.AddListener(delegate 
        {
            if (!string.IsNullOrEmpty(chatInput.text))
            {
                firebaseController.CreateMessage(channelReferenceSelected.idChannel, userName.text, chatInput.text);
                chatInput.text = "";
            }
        });

        returnChat.onClick.AddListener(delegate 
        {
            foreach (var item in messageItens) Destroy(item);
            messageItens.Clear();
            ClosePainel(chatPainel, true);
        });
    }

    public void OpenPainel(RectTransform obj)
    {
        obj.DOAnchorPos(Vector2.zero, timePainel, false);
    }

    public void ClosePainel(RectTransform obj , bool horizontal)
    {
        if(horizontal)
            obj.DOAnchorPos(new Vector2(screenResolution.x, 0), timePainel, false);
        else
            obj.DOAnchorPos(new Vector2(0, screenResolution.y), timePainel, false);
    }

    public void UpdateChannelsFromDataBase()
    {
        firebaseController.GetChannelsDatabase(channels);
    }

    public void SpawnChannelsButtons()
    {
        for(int i = 0; i < channels.Count; i++)
        {
            if (channelItens.Any(channelBtn => channelBtn.GetComponent<ChannelItem>().channelReference.idChannel == channels[i].idChannel)) continue;
            var button = Instantiate(itemChannelPrefab, channelContent.transform, false);
            var buttonComp = button.GetComponent<ChannelItem>();
            buttonComp.channelReference = channels[i];
            buttonComp.SetChannelInfos();
            buttonComp.channelNumber.text = i.ToString();
            channelItens.Add(button);
        }
    }

    public void SpawnChatMessages()
    {
        for (int i = 0; i < channelReferenceSelected.messages.Count; i++)
        {
            if (messageItensTeste.Any(messageItem => messageItem == channelReferenceSelected.messages[i].text)) continue;
            messageItensTeste.Add(channelReferenceSelected.messages[i].text);
            Debug.Log(channelReferenceSelected.messages[i].text);
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

    private void OpenChatPainel(Channel value)
    {
        OpenPainel(chatPainel);
        channelReferenceSelected = value;
        firebaseController.ObservingChatMessages(channelReferenceSelected);
        chatName.text = channelReferenceSelected.title;
    }

}
