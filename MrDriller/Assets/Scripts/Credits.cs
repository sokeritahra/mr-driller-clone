using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {
    
	void Update () {
        if (transform.position.y < 9.5f) {
            transform.position = transform.position + (Vector3.up * 0.03f);
        }
	}
    public void ResetCredPos() {
        transform.position = new Vector3(4, -15, 0);
    }
}
