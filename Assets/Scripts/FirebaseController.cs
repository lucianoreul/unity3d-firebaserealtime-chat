using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Linq;
using System;

[RequireComponent(typeof(PainelsController))]
public class FirebaseController : MonoBehaviour
{

    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    private string appUrl = "";
    private PainelsController painelsController;

    void Start()
    {
        painelsController = GetComponent<PainelsController>();
        //InitializeFirebaseOnEditor();
        InitializeFirebase();
    }

    // Inicia o firebase no unityeditor
    protected virtual void InitializeFirebaseOnEditor()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // caminho para a inicialização no editor.
                app.SetEditorDatabaseUrl(appUrl);
                if (app.Options.DatabaseUrl != null)
                {
                    app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
                }
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
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
                    newChannel.nameCreator = childSnapshot.Child("idCreator").Value.ToString();
                    newChannel.date = childSnapshot.Child("timestamp").Value.ToString();
                    newChannel.title = childSnapshot.Child("title").Value.ToString();
                    list.Add(newChannel);
                }
                painelsController.SpawnChannelsButtons();
            }
        });
    }

    public void CreaterChannelDataBase(string title)
    {
        var idChannel = FirebaseDatabase.DefaultInstance.GetReference("channels").Push().Key;
        Dictionary<string, object> ChannelUpdate = new Dictionary<string, object>();
        Dictionary<string, object> timestamp = new Dictionary<string, object>();
        timestamp[".sv"] = "timestamp";
        //ChannelUpdate["channels/" + idChannel + "/idCreator"] = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        ChannelUpdate["channels/" + idChannel + "/idCreator"] = "Testando";
        ChannelUpdate["channels/" + idChannel + "/title"] = title;
        ChannelUpdate["channels/" + idChannel + "/timestamp/"] = timestamp;
        FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(ChannelUpdate);
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

public struct Messages
{
    public string idMessage;
    public string idSender;
    public string text;
}
