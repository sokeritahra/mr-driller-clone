using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < 9.5f) {
            transform.position = transform.position + (Vector3.up * 0.01f);
        }
	}
}
