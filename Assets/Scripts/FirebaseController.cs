using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using System;

[RequireComponent(typeof(PainelsController))]
public class FirebaseController : MonoBehaviour
{
    private PainelsController painelsController;

    void Start()
    {
        painelsController = GetComponent<PainelsController>();
        InitializeFirebase();
    }

    protected virtual void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Can use this app");
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void AuthenticateAnonymouslyUser()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            painelsController.OpenPainel(painelsController.channelPainel);
            painelsController.UpdateChannelsFromDataBase();
        });
    }

    public void GetChannelsDatabase(List<Channel> list)
    {
        FirebaseDatabase.DefaultInstance.GetReference("channels").GetValueAsync().ContinueWith(task => 
        {
            if (task.IsFaulted || !task.IsCompleted || task.IsCanceled)
            {
                Debug.Log("Database wrong!");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot != null && snapshot.ChildrenCount > 0)
            {
                foreach (var childSnapshot in snapshot.Children)
                {
                    if (list.Any(channelNew => channelNew.idChannel == childSnapshot.Key)) continue;
                    var newChannel = new Channel();
                    newChannel.idChannel = childSnapshot.Key;
                    newChannel.nameCreator = childSnapshot.Child("nameCreator").Value.ToString();
                    newChannel.date = childSnapshot.Child("timestamp").Value.ToString();
                    newChannel.title = childSnapshot.Child("title").Value.ToString();
                    newChannel.messages = new List<Messages>();
                    list.Add(newChannel);
                }
                painelsController.SpawnChannelsButtons();
            }
        });
    }

    public void CreaterChannelDataBase(string title, string nameCreator)
    {
        var idChannel = FirebaseDatabase.DefaultInstance.GetReference("channels").Push().Key;
        Dictionary<string, object> ChannelUpdate = new Dictionary<string, object>();
        Dictionary<string, object> timestamp = new Dictionary<string, object>();
        timestamp[".sv"] = "timestamp";
        ChannelUpdate["channels/" + idChannel + "/nameCreator"] = nameCreator;
        ChannelUpdate["channels/" + idChannel + "/title"] = title;
        ChannelUpdate["channels/" + idChannel + "/timestamp/"] = timestamp;
        FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(ChannelUpdate);
    }

    public void CreateMessage(string idChannel, string nameCreator, string text)
    {
        var idMessage = FirebaseDatabase.DefaultInstance.GetReference("channels").Child(idChannel).Child("messages").Push().Key;
        Dictionary<string, object> messageUpdate = new Dictionary<string, object>();
        Dictionary<string, object> timestamp = new Dictionary<string, object>();
        timestamp[".sv"] = "timestamp";
        messageUpdate["messages/" + idMessage + "/nameCreator"] = nameCreator;
        messageUpdate["messages/" + idMessage + "/message"] = text;
        messageUpdate["messages/" + idMessage + "/timestamp/"] = timestamp;
        FirebaseDatabase.DefaultInstance.RootReference.Child("channels").Child(idChannel).UpdateChildrenAsync(messageUpdate);

    }

    public void ObservingChatMessages(Channel currentChannel)
    {
        FirebaseDatabase.DefaultInstance.GetReference("channels").Child(currentChannel.idChannel).Child("messages").ValueChanged += (object sender2, ValueChangedEventArgs e2) => 
        {
            if (e2.DatabaseError != null)
            {
                Debug.LogError(e2.DatabaseError.Message);
                return;
            }
            if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
            {
                foreach (var childSnapshot in e2.Snapshot.Children)
                {
                    if (currentChannel.messages.Any(contentMessage => contentMessage.idMessage == childSnapshot.Key)) continue;
                    var newMessage = new Messages();
                    newMessage.idMessage = childSnapshot.Key;
                    newMessage.idSender = childSnapshot.Child("nameCreator").Value.ToString();
                    newMessage.text = childSnapshot.Child("message").Value.ToString();
                    currentChannel.messages.Add(newMessage);
                }
                painelsController.SpawnChatMessages();
            }
        };
    }

    public void ObservingChatWriting()
    {

    }

    public void SignOutAplication()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

}

[Serializable]
public struct Channel
{
    public string idChannel;
    public string nameCreator;
    public string title;
    public string date;
    public List<Messages> messages;
}

[Serializable]
public struct Messages
{
    public string idMessage;
    public string idSender;
    public string text;
}
