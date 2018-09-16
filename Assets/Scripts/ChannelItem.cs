﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChannelItem : MonoBehaviour
{
    public TMP_Text channelNumber;
    public TMP_Text channelTitle;
    public Button   channelBtn;

    public Channel channelReference;

    private void Start()
    {
        channelBtn.onClick.AddListener(delegate 
        {
            OpenChatPainel();
        });
    }

    public void SetChannelInfos()
    {

    }

    private void OpenChatPainel()
    {

    }
}