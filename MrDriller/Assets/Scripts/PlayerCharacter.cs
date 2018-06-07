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
    public bool alive = true;
    float speed = 5; // Player movement speed
    float horizontal; 
    float vertical;
    float climbSpeed = 10f;
    public float climbTimer = 0.4f; // Time to wait before climbing
    float drillTimer = 0f; // Drill cooldown timer
    float staticTimer = 0f; // Time to recover after near death experience
    float fallTimer;
    //float level = 1; // Level counter
    //float depth = 0; // Drilling depth counter
    BlockScript bs;
    BlockManager bm;
    GameManager gm;
    //bool genBlocks = true;
    string animS = "";
    string animDefault = "Aim_Down";
    float drillDepth = 0.75f;
    float upperRayLength = 1f;
    float headRayLength = 0.4f;
    float handRayLength = 0.4f;
    float groundRayLength = 0.4f;
    LayerMask blockLayerMask = 1 << 9; // Layermask of blocks
    LayerMask candyLayerMask = 1 << 12; // Layermask of candy
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
    Collider2D[] candyCol;
    Vector2 indent = new Vector2(0.05f, 0);
    Vector2 playerCenter;
    Vector2 playerLeft;
    Vector2 playerRight;
    Vector3 climbUpTarget;
    Vector3 climbLeftTarget;
    Vector3 climbRightTarget;
    int candyTaken = 0;
    float reviveTimer = 0;
    bool alreadyPlayed = false;
    public string vomitAudioEvent;
    public string climbAudioEvent;
    public string fallingAudioEvent;
    public string flippedAudioEvent;
    public string squashedAudioEvent;
    public Vector3 startPos;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pm = PlayerMode.Down;
        bm = FindObjectOfType<BlockManager>();
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        c = GetComponent<CapsuleCollider2D>();
        startPos = rb.position;
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

        reviveTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && drillTimer <= 0) {
            CheckBlock(pm);
            Fabric.EventManager.Instance.PostEvent(vomitAudioEvent);
            //print("poranäppäintä painettu!");
        }
        if (Input.GetButtonDown("Fire2")) {
            gm.PauseGame();
        }
        if (gm.gameEnded && Input.anyKeyDown) {
            gm.ReturnToMenu();
        }
        if (Input.GetButtonDown("Cancel")) {
            gm.ReturnToMenu();
        }
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
        // Candy Antenna
        candyCol = Physics2D.OverlapBoxAll(playerCenter, new Vector2(0.5f, 0.5f), 0, candyLayerMask);

        if (candyCol.Length > 0) {
            TakeCandy(candyCol[0].gameObject);
        }


        //************************************ TestiKoodia*************************************

        //if (transform.position.y < -10 && genBlocks == true) {
        //    bm.GenerateBlocks(2);
        //    genBlocks = false;
        //}
        //************************************TestiKoodia *************************************

        // Read input from controller/keyboard
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //// What to do if block falls on the way if climbing
        //if (pm == PlayerMode.Climbing && leftHandAntenna || rightHandAntenna) {
        //    pm = previousPm;
        //}

        // Force reset static mode
        if (pm == PlayerMode.Static && staticTimer < 0) {
            pm = previousPm;
        }

        if (alive) {
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
                    // rb.AddForce(new Vector2(rb.velocity.x, -5), ForceMode2D.Force);
                    rb.velocity = new Vector2(rb.velocity.x, -5f);
                    animS = animDefault;
                    fallTimer -= Time.deltaTime;
                    if (fallTimer < 0) {
                        rb.velocity = new Vector2(0, -5f);
                        //rb.AddForce(new Vector2(0, -5), ForceMode2D.Force);
                        
                        animS = "Falling";
                        if (!alreadyPlayed) {
                            Fabric.EventManager.Instance.PostEvent(fallingAudioEvent);
                            alreadyPlayed = true;
                        }
                    }
                } else {
                    pm = PlayerMode.Static;

                }

            } else {
                rb.velocity = new Vector2(0, 0);
                pm = previousPm;
                alreadyPlayed = false;
                animS = animDefault;
                fallTimer = .2f;
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
                        if (pm == PlayerMode.Climbing && rightHandAntenna) {
                            pm = previousPm;
                        } else {
                            rb.velocity = new Vector2(speed, 0);
                        }

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
                        if (pm == PlayerMode.Climbing && leftHandAntenna) {
                            pm = previousPm;
                        } else {
                            rb.velocity = new Vector2(-speed, 0);
                        }
                    }
                    if (transform.position.x < climbLeftTarget.x) {
                        pm = previousPm;
                    }
                }
            } else {

                if (horizontal > 0 && !rightHandAntenna && rb.transform.position.x < bm.columns - 1) {
                    rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
                }
                if (horizontal < 0 && !leftHandAntenna && rb.transform.position.x > bm.firstBlock.x) {
                    rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
                }

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
                        } else if (!rightHandAntenna && !upperRightAntenna) {
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

                        climbTimer = 0.4f;
                    }
                }
                previousPm = pm;
                gm.Depth(-Mathf.RoundToInt(transform.position.y - 1));
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
                climbUpTarget = (transform.position + Vector3.up * 1.01f);
                climbLeftTarget = (transform.position + Vector3.left * 0.75f);
                pm = PlayerMode.Climbing;
                climbTimer = 0.4f;
                Fabric.EventManager.Instance.PostEvent(climbAudioEvent);
            }

            if (rightHandAntenna && !upperRightAntenna && climbTimer < 0 && pm != PlayerMode.Climbing) {
                climbUpTarget = (transform.position + Vector3.up * 1.01f);
                climbRightTarget = (transform.position + Vector3.right * 0.75f);
                pm = PlayerMode.Climbing;
                climbTimer = 0.4f;
                Fabric.EventManager.Instance.PostEvent(climbAudioEvent);
            }

            if (staticTimer > 0) { // Animation / Player static timer
                staticTimer -= Time.deltaTime;
            }
            // Drill timer deduction
            if (drillTimer > 0) { // Drill cooldown timer
                drillTimer -= Time.deltaTime;
            }
            // Drilling
            //if (Input.GetButtonDown("Fire1") && drillTimer <= 0) {
            //    CheckBlock(pm);
            //    Fabric.EventManager.Instance.PostEvent(vomitAudioEvent);
            //    //print("poranäppäintä painettu!");
            //}

            //jos ei ole reunassa nämä pätee mutta reunassa pitää kuolla
            if (centerHeadAntenna || leftHeadAntenna || rightHeadAntenna) {
                if (leftHeadAntenna && rightHeadAntenna 
                    //|| (leftHandAntenna && Mathf.RoundToInt(rb.transform.position.x) >= Mathf.RoundToInt(bm.columns - 1)) ||
                    //(rightHandAntenna && Mathf.RoundToInt(rb.transform.position.x) <= Mathf.RoundToInt(bm.firstBlock.x))
                    ) {
                    // Squash player
                    Fabric.EventManager.Instance.PostEvent(squashedAudioEvent);
                    ColdAndLonelyDeath(true);
                    c.enabled = false;
                    print(leftHandAntenna.collider);
                    print(rightHandAntenna.collider);

                } else if (leftHeadAntenna) {
                    // NDE bellied or assed towards right
                    if (pm == PlayerMode.Right) {
                        animS = "Bellied_Right";
                        transform.position = (Vector2)transform.position + (Vector2.right / 2);
                        staticTimer = 1.0f;
                        Fabric.EventManager.Instance.PostEvent(flippedAudioEvent);
                    } else {
                        animS = "Assed_Right";
                        transform.position = (Vector2)transform.position + (Vector2.right / 2);
                        staticTimer = 1.0f;
                        Fabric.EventManager.Instance.PostEvent(flippedAudioEvent);
                    }
                } else {
                    // NDE bellied or assed towards left
                    if (pm == PlayerMode.Left) {
                        animS = "Bellied_Left";
                        transform.position = (Vector2)transform.position + (Vector2.left / 2);
                        staticTimer = 1.0f;
                        Fabric.EventManager.Instance.PostEvent(flippedAudioEvent);
                    } else {
                        animS = "Assed_Left";
                        transform.position = (Vector2)transform.position + (Vector2.left / 2);
                        staticTimer = 1.0f;
                        Fabric.EventManager.Instance.PostEvent(flippedAudioEvent);
                    }
                }
            }
        }

        else if (reviveTimer < 0) {
            bm.PopThreeColumnsOnTop();
            Revive();
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

        //print("Poraus koordinaatit: " + Mathf.RoundToInt(x) + ", " + Mathf.RoundToInt(y) );
        bs = bm.blockGrid[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];

        if (bs) {
            DrillBlock(bs);
        }
    }

    // Check if grounded
    bool IsGrounded() {
        //return ((groundCheckCenter) || (groundCheckLeft) || (groundCheckRight));
        return (groundCheckCenter || groundCheckLeft || groundCheckRight);
    }

    // Pop the block (from BlockManager)
    void DrillBlock(BlockScript block) {
        if(block.bc == BlockColor.Candy) {
            TakeCandy(block.gameObject);
        }
        //jos blokki on grey, pitää poksauttaa vain se ja POISTAA RYHMÄSTÄÄN!
        if(block.bs == BlockState.Static || block.bs == BlockState.Hold) {
            if (block.bc == BlockColor.Grey) {
                block.didGreyGetDrilled = true;
                block.Pop(1, 0);
            } else {
                bm.PopBlocks(block.group, 1, 100);
            }
        }
    }

    void TakeCandy(GameObject candy) {
        candyTaken++;
        var candyScript = candy.GetComponent<BlockScript>();
        bm.PopBlocks(candyScript.group, 1, 100*candyTaken);
        gm.CandyGet();
    }

    void Revive() {
        c.enabled = true;
        alive = true;
        animS = animDefault;
    }

    // Death on arrival
    public void ColdAndLonelyDeath(bool selfCalled) { // Name probably says it all
        rb.velocity = new Vector2(0, 0);
        animS = "Death_Squashed";
        pm = PlayerMode.Static;
        anim.Play(animS);
        alive = false;
        if (selfCalled) {
            gm.DeadOnArrival();
        }
        reviveTimer = 2f;
    }

    public void StartNewLvl() {
        rb.position = new Vector3(rb.position.x, (startPos.y + 5f), 0);
    }
}
