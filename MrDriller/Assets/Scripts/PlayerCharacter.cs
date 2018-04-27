using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum PlayerMode { Up, Down, Left, Right, Falling }; // Mmodes for the sprites

public class PlayerCharacter : MonoBehaviour {
    Rigidbody2D rb;
    SpriteRenderer sr;
    PlayerMode pm;
    public float speed;

    float vertical;
    float horizontal;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }


    void FixedUpdate () {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) {
            if (vertical < 0) {
                pm = PlayerMode.Down;
            }
            if (vertical > 0) {
                pm = PlayerMode.Up;
            }
        } else {
            if (horizontal < 0) {
                pm = PlayerMode.Right;
            }
            if (horizontal > 0) {
                pm = PlayerMode.Left;
            }
        }
        
        //if (pm == PlayerMode.Down) {
            
        //}
        //if (pm == PlayerMode.Up) {

        //}
        //if (pm == PlayerMode.Left) {

        //}
        //if (pm == PlayerMode.Right) {

        //}
    }
}
