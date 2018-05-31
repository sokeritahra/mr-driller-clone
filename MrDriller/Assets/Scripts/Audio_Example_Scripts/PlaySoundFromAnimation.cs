using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundFromAnimation : MonoBehaviour {

	
	public AudioSource sfx;

	
	// Update is called once per frame
	void PlaySound () {
		Fabric.EventManager.Instance.PostEvent("FS");
	}
}
