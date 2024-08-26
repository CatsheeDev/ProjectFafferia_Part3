using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorHandler : MonoBehaviour
{
    public enum ErrorType
    {
        Warning, 
        Error, 
        Mwa
    }

    public static void LoadErrorScene()
    {
        SceneManager.LoadScene("ERROR");
    }

    public static void LoadErrorScene(string errorNote, ErrorType errType = ErrorType.Error) //TODO: ERROR NOTE IN SCENE
    {
        switch (errType)
        {
            case ErrorType.Warning:
                Debug.LogWarning(errorNote); break;
            case ErrorType.Mwa:
            case ErrorType.Error: Debug.LogError(errorNote); break;
        }   

        SceneManager.LoadScene("ERROR");
    }

    public void OpenLogFolder()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
        Application.Quit();
    }
}
