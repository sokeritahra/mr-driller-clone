﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    float cameraSpeed = 5f;
    public GameObject player;
	void Update () {
        var p = transform.position;
        p.y = player.transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, p, Time.deltaTime * cameraSpeed);
    }
}