using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerMode {Up, Down, Left, Right, Falling, Static, Climbing}; // Modes for the sprites and drill directions

public class PlayerCharacter : MonoBehaviour {
    Rigidbody2D rb;
    CapsuleCollider2D c;
    public PlayerMode pm;
    public PlayerMode previousPm;
    Animator anim;
    bool alive = true;
    float speed = 5; // Player movement speed
    float climbSpeed = 5f;
    float horizontal; 
    float vertical;
    float drillTimer = 0f; // Drill cooldown timer
    public float climbTimer = 0.5f; // Time to wait before climbing
    float staticTimer = 0f; // Time to recover after near death experience
    float fallTimer;
    //float level = 1; // Level counter
    //float depth = 0; // Drilling depth counter
    BlockScript bs;
    BlockManager bm;
    string animS = "";
    string animDefault = "Aim_Down";
    float drillDepth = 0.75f;
    float upperRayLength = 1f;
    float headRayLength = 0.4f;
    float handRayLength = 0.4f;
    float groundRayLength = 0.4f;
    LayerMask blockLayerMask = 1 << 9; // Layermask of blocks
    RaycastHit2D groundCheckRight;
    RaycastHit2D groundCheckLeft;
    RaycastHit2D groundCheckCenter;
    RaycastHit2D centerHeadAntenna;
    RaycastHit2D leftHeadAntenna;
    RaycastHit2D rightHeadAntenna;
    RaycastHit2D leftHandAntenna;
    RaycastHit2D rightHandAntenna;
    RaycastHit2D upperLeftAntenna;
    RaycastHit2D upperRightAntenna;
    Vector2 indent = new Vector2(0.05f, 0);
    Vector2 playerCenter;
    Vector2 playerLeft;
    Vector2 playerRight;
    Vector3 climbUpTarget;
    Vector3 climbLeftTarget;
    Vector3 climbRightTarget;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pm = PlayerMode.Down;
        bm = FindObjectOfType<BlockManager>();
        c = GetComponent<CapsuleCollider2D>();
    }

    private void Update() {


        // Debug drawings of playercharacter head antennas and falling sensors

        // Upper antennas
        Debug.DrawRay(playerCenter + Vector2.up, Vector2.left * upperRayLength,  Color.red);
        Debug.DrawRay(playerCenter + Vector2.up, Vector2.right * upperRayLength, Color.red);
        // Head Antennas
        Debug.DrawRay(playerCenter, Vector2.up * headRayLength, Color.green);
        Debug.DrawRay(playerLeft + indent, Vector2.up * headRayLength, Color.green);
        Debug.DrawRay(playerRight - indent, Vector2.up * headRayLength, Color.green);
        // Hand Antennas
        Debug.DrawRay(playerCenter, Vector2.left * handRayLength, Color.blue);
        Debug.DrawRay(playerCenter, Vector2.right * handRayLength, Color.blue);
        // Ground Antennas
        Debug.DrawRay(playerCenter, Vector2.down * groundRayLength, Color.yellow);
        Debug.DrawRay(playerLeft + indent * 2, Vector2.down * groundRayLength, Color.yellow);
        Debug.DrawRay(playerRight - indent * 2, Vector2.down * groundRayLength, Color.yellow);

        anim.Play(animS);
    }

    void FixedUpdate() {
        // Player Left, right and center collider points (in relation to collider)
        playerCenter = c.bounds.center;
        playerLeft = c.bounds.center - (c.bounds.size.x / 2 * Vector3.right);
        playerRight = c.bounds.center + (c.bounds.size.x / 2 * Vector3.right);

        // Playercharacter head antennas and falling sensors

        // Upper Antennas
        upperLeftAntenna = Physics2D.Raycast(playerCenter + Vector2.up, Vector2.left, upperRayLength, blockLayerMask);
        upperRightAntenna = Physics2D.Raycast(playerCenter + Vector2.up, Vector2.right, upperRayLength, blockLayerMask);
        // Head Antennas
        centerHeadAntenna = Physics2D.Raycast(playerCenter, Vector2.up, headRayLength, blockLayerMask);
        leftHeadAntenna = Physics2D.Raycast(playerLeft + indent, Vector2.up, headRayLength, blockLayerMask);
        rightHeadAntenna = Physics2D.Raycast(playerRight - indent, Vector2.up, headRayLength, blockLayerMask);
        // Hand Antennas
        leftHandAntenna = Physics2D.Raycast(playerCenter, Vector2.left, handRayLength, blockLayerMask);
        rightHandAntenna = Physics2D.Raycast(playerCenter, Vector2.right, handRayLength, blockLayerMask);
        // Ground Antennas
        groundCheckCenter = Physics2D.Raycast(playerCenter, Vector2.down, groundRayLength, blockLayerMask);
        groundCheckLeft = Physics2D.Raycast(playerLeft + indent, Vector2.down, groundRayLength, blockLayerMask);
        groundCheckRight = Physics2D.Raycast(playerRight - indent, Vector2.down, groundRayLength, blockLayerMask);




        // Read input from controller/keyboard
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // What to do if player is not grounded or set staticMode
        if (!IsGrounded() || staticTimer > 0 || pm == PlayerMode.Climbing) {
            if (pm == PlayerMode.Climbing) {
                if (previousPm == PlayerMode.Right) {
                    animS = "Climb_Right";
                } else {
                    animS = "Climb_Left";
                }

            } else if (!IsGrounded()) {
                pm = PlayerMode.Falling;

                rb.AddForce(new Vector2(0, -5), ForceMode2D.Force);
                fallTimer -= Time.deltaTime;
                if (fallTimer < 0) {
                    animS = "Falling";
                }
            } else {
                pm = PlayerMode.Static;

            }

        } else {
            if (alive) {
                rb.velocity = new Vector2(0, 0);
                pm = previousPm;
            animS = animDefault;
                fallTimer = .1f;
            }
        }

        if (pm == PlayerMode.Falling || pm == PlayerMode.Static) { // Shouldn't move when falling or static
            rb.velocity = new Vector2(0, rb.velocity.y);

        }
        else if (pm == PlayerMode.Climbing) {
            if (previousPm == PlayerMode.Right) {
                if (transform.position.y < climbUpTarget.y) {
                    rb.velocity = new Vector2(0, climbSpeed);
                }
                if (transform.position.y > climbUpTarget.y) {
                    animS = animDefault;
                    rb.velocity = new Vector2(speed, 0);
                }
                if (transform.position.x > climbRightTarget.x) {
                    pm = previousPm;
                }
            } else {
                if (transform.position.y < climbUpTarget.y) {
                    rb.velocity = new Vector2(0, climbSpeed);
                }
                if (transform.position.y > climbUpTarget.y) {
                    animS = animDefault;
                    rb.velocity = new Vector2(-speed, 0);
                }
                if (transform.position.x < climbLeftTarget.x) {
                    pm = previousPm;
                }
            }
        } else {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) { // Set player (drilling) mode
                if (vertical < 0) {
                    pm = PlayerMode.Down;
                    animDefault = "Aim_Down";
                }
                if (vertical > 0) {
                    pm = PlayerMode.Up;
                    animDefault = "Aim_Up";
                }
            } else {
                if (horizontal > 0) {
                    if (rightHandAntenna && !upperRightAntenna) {
                        pm = PlayerMode.Right;
                        animS = "Push_Right";
                        animDefault = "Aim_Right";
                        climbTimer -= Time.deltaTime;
                    } else if (!rightHandAntenna && !upperRightAntenna){
                        pm = PlayerMode.Right;
                        animS = "Walk_Right";
                        animDefault = "Aim_Right";
                    } else {
                        pm = PlayerMode.Right;
                        animDefault = "Aim_Right";
                    }

                } else if (horizontal < 0) {
                    if (leftHandAntenna && !upperLeftAntenna) {
                        pm = PlayerMode.Left;
                        animS = "Push_Left";
                        animDefault = "Aim_Left";
                        climbTimer -= Time.deltaTime;
                    } else if (!leftHandAntenna && !upperLeftAntenna) {
                        pm = PlayerMode.Left;
                        animS = "Walk_Left";
                        animDefault = "Aim_Left";
                    } else {
                        pm = PlayerMode.Left;
                        animDefault = "Aim_Left";
                    }
                } else {

                    climbTimer = 0.5f;
                }
            }
            previousPm = pm;
        }


        // Slipping
        //**************** If You remove the following part from comments, CHECK GROUNDED BOOL*********************
        if (!groundCheckCenter && !groundCheckLeft || !groundCheckRight) {
            if (!groundCheckCenter && !groundCheckLeft && !groundCheckRight) {

            } else if (!groundCheckLeft) {
                rb.velocity = new Vector2(-speed, 0);
            } else {
                rb.velocity = new Vector2(speed, 0);
            }
        }
        //*********************************************************************************************************

        // Climbing
        if (leftHandAntenna && !upperLeftAntenna && climbTimer < 0 && pm != PlayerMode.Climbing) {
            climbUpTarget = (transform.position + Vector3.up * 1.1f);
            climbLeftTarget = (transform.position + Vector3.left * 0.75f);
            pm = PlayerMode.Climbing;
        }

        if (rightHandAntenna && !upperRightAntenna && climbTimer < 0 && pm != PlayerMode.Climbing) {
            climbUpTarget = (transform.position + Vector3.up * 1.1f);
            climbRightTarget = (transform.position + Vector3.right * 0.75f);
            pm = PlayerMode.Climbing;
        }
        
        if (staticTimer > 0) { // Animation / Player static timer
            staticTimer -= Time.deltaTime;
        }
        // Drill timer deduction
        if (drillTimer > 0) { // Drill cooldown timer
            drillTimer -= Time.deltaTime;
        }
        // Drilling
        if (Input.GetButton("Fire1") && drillTimer <= 0) { 
            CheckBlock(pm);
            print("poranäppäintä painettu!");
        }
        if (centerHeadAntenna || leftHeadAntenna || rightHeadAntenna) {
            if (leftHeadAntenna && rightHeadAntenna) {
                // Squash player
                animS = "Death_Squashed";
                pm = PlayerMode.Static;
                anim.Play(animS);
                alive = false;
                ColdAndLonelyDeath();
                Time.timeScale = 0;
            } else if (leftHeadAntenna) {
                // NDE bellied or assed towards right
                if (pm == PlayerMode.Right) {
                    animS = "Bellied_Right";
                    transform.position = (Vector2)transform.position + (Vector2.right / 2);
                    staticTimer = 1.5f;
                } else {
                    animS = "Assed_Right";
                    transform.position = (Vector2)transform.position + (Vector2.right / 2);
                    staticTimer = 1.5f;
                }
            } else {
                // NDE bellied or assed towards left
                if (pm == PlayerMode.Left) {
                    animS = "Bellied_Left";
                    transform.position = (Vector2)transform.position + (Vector2.left / 2);
                    staticTimer = 1.5f;
                } else {
                    animS = "Assed_Left";
                    transform.position = (Vector2)transform.position + (Vector2.left / 2);
                    staticTimer = 1.5f;
                }
            }
        }
    }

    // Check playermode and if there is a block to drill in that direction 
    void CheckBlock(PlayerMode mode) {
        drillTimer = 0.2f;
    staticTimer = 0.2f;
        float x = transform.position.x;
        float y = transform.position.y * -1f;

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

        print("Poraus koordinaatit: " + Mathf.RoundToInt(x) + ", " + Mathf.RoundToInt(y) );
        bs = bm.blockGrid[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
            
        print(bs);
            
        if (bs) {
            DrillBlock(bs);
        }

        print("porattiin " + bs + " paikassa " + transform.position);
    }
    // Check if grounded
    bool IsGrounded() {
        return (groundCheckCenter || groundCheckLeft || groundCheckRight);
    }
    // Pop the block (from BlockManager)
    void DrillBlock(BlockScript block) {
        bm.PopBlocks(block);
    }
    // Death on arrival
    void ColdAndLonelyDeath() { // Name probably says it all
        print("Aarghh!");
    }
}
