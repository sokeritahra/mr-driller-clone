using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    float cameraSpeed = 5f;
    public Transform player;
    
	void Update () {
        var p = transform.position;
        var pl = player.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(p.x ,pl.y-2, p.z), Time.deltaTime * cameraSpeed);
    }
}
