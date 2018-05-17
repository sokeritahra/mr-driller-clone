using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerMode { Up, Down, Left, Right, Falling }; // Modes for the sprites and drill directions

public class PlayerCharacter : MonoBehaviour {
    Rigidbody2D rb;
    public PlayerMode pm;
    public PlayerMode previousPm;
    Animator anim;
    public float speed; // Player horizontal speed
    float horizontal; 
    float vertical;
    float drillTimer = 0f; // Drill cooldown timer
    //float level = 1; // Level counter
    //float depth = 0; // Drilling depth counter
    BlockScript bs;
    BlockManager bm;
    public float drillDepth = 0.75f;
    float rayLength = 0.1f;
    RaycastHit2D hit;
    RaycastHit2D hitLeft;
    Vector2 directionDown = (Vector2)Vector3.down;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pm = PlayerMode.Down;
        bm = FindObjectOfType<BlockManager>();
    }

    bool IsGrounded() {
        Vector2 tempLeft = new Vector2(transform.position.x, transform.position.y) + new Vector2(-0.25f, -0.6f);
        hitLeft = Physics2D.Raycast(tempLeft, directionDown, rayLength);
        if (hitLeft.collider != null) {
            Debug.DrawRay(tempLeft, directionDown * hitLeft.distance, Color.yellow);
        }
        else {
            Debug.DrawRay(tempLeft, directionDown * 1000, Color.white);
        }

        Vector2 tempVector = new Vector2(transform.position.x, transform.position.y) + new Vector2(0.25f, -0.6f);
        hit = Physics2D.Raycast(tempVector, directionDown, rayLength);
        if (hit.collider != null) {
            Debug.DrawRay(tempVector, directionDown * hit.distance, Color.yellow);
        }
        else {
            Debug.DrawRay(tempVector, directionDown * 1000, Color.white);
        }
        return (Physics2D.Raycast(tempVector, directionDown, rayLength) || Physics2D.Raycast(tempLeft, directionDown, rayLength));
     }
    
    void FixedUpdate() {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");  // Read input from controller/keyboard
        if (pm != PlayerMode.Falling) { //Shouldn't move when falling
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y); // Move player horizontally
        }
        else {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (!IsGrounded()) {
            if (pm != PlayerMode.Falling) {
                previousPm = pm;
            }
            pm = PlayerMode.Falling;
        }
        else {
            pm = previousPm;
            if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) { // Set player (drilling) mode
                if (vertical < 0) {
                    pm = PlayerMode.Down;
                    anim.Play("Aim_Down");
                }
                if (vertical > 0) {
                    pm = PlayerMode.Up;
                    anim.Play("Aim_Up");
                }
            }
            else {
                if (horizontal > 0) {
                    pm = PlayerMode.Right;
                    anim.Play("Aim_Right");
                }
                if (horizontal < 0) {
                    pm = PlayerMode.Left;
                    anim.Play("Aim_Left");
                }
            }
        
        }

        if (drillTimer > 0) { // Drill cooldown timer
            drillTimer -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && drillTimer <= 0) { 
            CheckBlock(pm);

            print("poranäppäintä painettu!");

            }
            //if (pm == PlayerMode.Up) {
            //    print("Jos minulla olisi porattavaa, niin poraisin yllä olevan blokin!");
            //    drillTimer = 0.5f;
            //    anim.Play("Drill_Up");
            //}
            //if (pm == PlayerMode.Left) {
            //    print("Jos minulla olisi porattavaa, niin saattaisin porata vasemmalle!");
            //    drillTimer = 0.5f;
            //    anim.Play("Drill_Left");
            //}
            //if (pm == PlayerMode.Right) {
            //    print("Jos minulla olisi porattavaa, niin kaikki olisi hyvin!");
            //    drillTimer = 0.5f;
            //    anim.Play("Drill_Right");
            //}
        }


    void CheckBlock(PlayerMode mode) {
        drillTimer = 0.5f;
        float x = transform.position.x;
        float y = transform.position.y * -1f;
        string animS = "";

        if (mode == PlayerMode.Down) {
            y += drillDepth;
            animS = "Drill_Down";
        }
        else if (mode == PlayerMode.Left) {
            x -= drillDepth;
            animS = "Drill_Left";
        }
        else if (mode == PlayerMode.Right) {
            x += drillDepth;
            animS = "Drill_Right";
        }
        else if (mode == PlayerMode.Up) {
            y -= drillDepth;
            animS = "Drill_Up";
        }
        else {
            return;
        }

        anim.Play(animS);

        print(Mathf.RoundToInt(x) + ", " + Mathf.RoundToInt(y) );
        bs = bm.blockGrid[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];

        print(bs);

        if (bs) {
            DrillBlock(bs);

        }

        print("porattiin " + bs + " paikassa " + transform.position);

    }

    void DrillBlock(BlockScript block) {
        bm.PopBlocks(block);
        print("joo oli");
        //tarviiks muuta
    }

    void ColdAndLonelyDeath() { // Name probably says it all
        print("Aarghh!");
    }
}
