using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {



	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
            transform.position += Vector3.right; 
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            transform.position += Vector3.left;
        }
    }
}
