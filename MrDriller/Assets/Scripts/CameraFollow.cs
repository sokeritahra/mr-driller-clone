using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    float cameraSpeed = 5f;
    public Transform player;
    
	void Update () {
        var p = transform.position;
        var pl = player.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(p.x ,Mathf.RoundToInt(pl.y) - 0.5f, p.z), Time.deltaTime * cameraSpeed);
    }
    public void ResetCamPos() {
        transform.position = new Vector3(9, 1, 0);
    }
}
