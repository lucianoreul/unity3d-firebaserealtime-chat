using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

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
        });
    }

    public void SignOutAplication()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

}
