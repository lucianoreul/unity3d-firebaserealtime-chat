using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class FirebaseConnect : MonoBehaviour 
{
	
    	private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    	private string appUrl = "";
	
	void Start ()
	{
		InitializeFirebase();
	}
	
    	protected virtual void InitializeFirebase()
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
			Debug.LogError("Não foi possivel resolver as dependêcias: " + dependencyStatus);
		    }
		});
    	}
}
