using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum PlayerMode { Up, Down, Left, Right, Falling }; // Modes for the sprites and drill directions

public class PlayerCharacter : MonoBehaviour {
    Rigidbody2D rb;
    //SpriteRenderer sr;
    //PlayerMode pm;
    public float speed;
    float horizontal;
    //float vertical;
    //float drillTimer = 0.5f;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
       // sr = GetComponent<SpriteRenderer>();
    }


    void FixedUpdate () {
        horizontal = Input.GetAxis("Horizontal");
        //vertical = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        //if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) {
        //    if (vertical < 0) {
        //        pm = PlayerMode.Down;
        //    }
        //    if (vertical > 0) {
        //        pm = PlayerMode.Up;
        //    }
        //} else {
        //    if (horizontal < 0) {
        //        pm = PlayerMode.Right;
        //    }
        //    if (horizontal > 0) {
        //        pm = PlayerMode.Left;
        //        ColdAndLonelyDeath();
        //    }
        //}

        //if (Input.GetButton("Fire 1")) {
        //    if (pm == PlayerMode.Down) {
        //        // Pseudocode: Poraa alapuolella olevaan blokkiin
        //    }
        //    if (pm == PlayerMode.Up) {
        //        // Pseudocode: Poraa yläpuolella olevaan blokkiin
        //    }
        //    if (pm == PlayerMode.Left) {
        //        // Pseudocode: Poraa vasemmalla olevaan blokkiin
        //    }
        //    if (pm == PlayerMode.Right) {
        //        // Pseudocode: Poraa oikealla olevaan blokkiin
        //    }
        //}

    }

    void ColdAndLonelyDeath() {
        print("Aarghh!");
    }
}
