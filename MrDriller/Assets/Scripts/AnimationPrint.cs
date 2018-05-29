using UnityEngine;
using System.Collections;

public class AnimationPrint : MonoBehaviour {

	public void PrintFloat (float theValue) {
		//Fabric.EventManager.Instance.PostEvent("FS");
		Debug.Log ("PrintFloat is called with a value of " + theValue);
	}
}
