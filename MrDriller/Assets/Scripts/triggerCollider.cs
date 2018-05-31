using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class triggerCollider : MonoBehaviour {

	public AudioMixerSnapshot mixOn;
	public AudioMixerSnapshot mixOff;


	void OnTriggerEnter(Collider other) {
		Debug.Log ("Trigger");

		//mixOn.TransitionTo (0.3f);
	}

	void OnTriggerExit(Collider other){

		//mixOff.TransitionTo (0.3f);
	}
}
