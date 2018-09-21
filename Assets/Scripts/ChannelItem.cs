using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChannelItem : MonoBehaviour
{
    public TMP_Text channelNumber;
    public TMP_Text channelTitle;
    public TMP_Text channelCreatorName;
    public Button   channelBtn;

    // Event for open chat painel
    public delegate void ClickAction(Channel value);
    public static event ClickAction OnClickOpen;

    public Channel channelReference;

    private void Start()
    {
        channelBtn.onClick.AddListener(delegate 
        {
            // click cjannel button for call event
            OnClickOpen(channelReference);
        });
    }

    public void SetChannelInfos()
    {
        channelTitle.text = channelReference.title;
        channelCreatorName.text = channelReference.nameCreator;
    }
}
