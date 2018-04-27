using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {
    Rigidbody2D rb;
    public float speed;
    //enum playerModes[Up, Down, Left, Right, Falling]; // Future modes for the sprites
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate () {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }
}
