using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public string menuAudioEvent;

    public void PlayGame()
    {
        SceneManager.LoadScene("ToothBug");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Start() {
        Fabric.EventManager.Instance.PostEvent(menuAudioEvent);
    }
}
