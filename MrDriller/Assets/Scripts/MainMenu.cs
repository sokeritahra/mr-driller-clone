using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public string menuAudioEvent;

    public void PlayGame()
    {
        SceneManager.LoadScene("ToothBug");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start() {
        Fabric.EventManager.Instance.PostEvent(menuAudioEvent);
    }
}
