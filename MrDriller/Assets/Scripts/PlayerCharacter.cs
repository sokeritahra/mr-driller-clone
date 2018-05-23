using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerMode {Up, Down, Left, Right, Falling, Static}; // Modes for the sprites and drill directions

public class PlayerCharacter : MonoBehaviour {
    Rigidbody2D rb;
    Collider2D c;
    public PlayerMode pm;
    public PlayerMode previousPm;
    Animator anim;
    
    public float speed; // Player horizontal speed
    float horizontal; 
    float vertical;
    public float drillTimer = 0f; // Drill cooldown timer
    float climbTimer = 0.5f; // Time to wait before climbing
    public float animationTimer = 0f; // Time to recover after near death experience
    //float level = 1; // Level counter
    //float depth = 0; // Drilling depth counter
    BlockScript bs;
    BlockManager bm;
    string animS = "";
    string animDefault = "Down";
    public float drillDepth = 0.75f;
    float rayLength = 0.6f;
    public LayerMask blockLayerMask = 1 << 9; // Layermask of blocks
    Vector2 indent = new Vector2(0.1f,0);
    RaycastHit2D hitRight;
    RaycastHit2D hitLeft;
    RaycastHit2D centerAntenna;
    RaycastHit2D leftAntenna;
    RaycastHit2D rightAntenna;
    RaycastHit2D leftHandAntenna;
    RaycastHit2D rightHandAntenna;
    RaycastHit2D upperLeftAntenna;
    RaycastHit2D upperRightAntenna;
    Vector2 playerCenter;
    Vector2 playerLeft;
    Vector2 playerRight;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pm = PlayerMode.Down;
        bm = FindObjectOfType<BlockManager>();
        c = GetComponent<Collider2D>();
    }

    bool IsGrounded() {
        hitLeft = Physics2D.Raycast(playerLeft, Vector2.down, rayLength, blockLayerMask);
        hitRight = Physics2D.Raycast(playerRight, Vector2.down, rayLength, blockLayerMask);
        return (hitLeft || hitRight);
    }


    private void Update() {
        // Debug drawings of playercharacter head antennas and falling sensors
        Debug.DrawRay(playerCenter, Vector2.up * (rayLength * 0.75f), Color.red);
        Debug.DrawRay(playerLeft + indent, Vector2.up * (rayLength * 0.75f), Color.green);
        Debug.DrawRay(playerRight - indent, Vector2.up * (rayLength * 0.75f), Color.blue);
        Debug.DrawRay(playerCenter, Vector2.left / 2, Color.red);
        Debug.DrawRay(playerCenter, Vector2.right / 2, Color.red);
        Debug.DrawRay(playerCenter + Vector2.up, Vector2.left, Color.red);
        Debug.DrawRay(playerCenter + Vector2.up, Vector2.right, Color.red);

        if (hitLeft.collider != null) {
            Debug.DrawRay(playerLeft, Vector2.down * hitLeft.distance, Color.yellow);
        } else {
            Debug.DrawRay(playerLeft, Vector2.down * 1000, Color.white);
        }
        if (hitRight.collider != null) {
            Debug.DrawRay(playerRight, Vector2.down * hitRight.distance, Color.yellow);
        } else {
            Debug.DrawRay(playerRight, Vector2.down * 1000, Color.white);
        }

    }

    void FixedUpdate() {
        // Player collider points (Left, right, center)
        playerCenter = c.bounds.center;
        playerLeft = c.bounds.center - (c.bounds.size.x / 2 * Vector3.right);
        playerRight = c.bounds.center + (c.bounds.size.x / 2 * Vector3.right);

        // Read input from controller/keyboard
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (!IsGrounded() || animationTimer > 0) {
            if (!IsGrounded()) {
                pm = PlayerMode.Falling;
            } else if (animationTimer > 0) {
                pm = PlayerMode.Static;
            }
        } else {
            pm = previousPm;
            anim.Play(animDefault);
        }

        if (pm == PlayerMode.Falling || pm == PlayerMode.Static) { // Shouldn't move when falling or static
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

            if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) { // Set player (drilling) mode
                if (vertical < 0) {
                    pm = PlayerMode.Down;
                    previousPm = pm;
                    animDefault = "Aim_Down";
                }
                if (vertical > 0) {
                    pm = PlayerMode.Up;
                    previousPm = pm;
                    animDefault = "Aim_Up";
                }
            } else {
                if (horizontal > 0) {
                    pm = PlayerMode.Right;
                    previousPm = pm;
                    animDefault = "Aim_Right";
                    climbTimer -= Time.deltaTime;
                } else if (horizontal < 0) {
                    pm = PlayerMode.Left;
                    previousPm = pm;
                    animDefault = "Aim_Left";
                    climbTimer -= Time.deltaTime;
                } else {
                    climbTimer = 0.5f;
                }
            }
        }
        
        
        // Head antennas
        centerAntenna = Physics2D.Raycast(playerCenter, Vector2.up, rayLength * 0.75f, blockLayerMask);
        leftAntenna = Physics2D.Raycast(playerLeft + indent, Vector2.up, rayLength * 0.75f, blockLayerMask);
        rightAntenna = Physics2D.Raycast(playerRight - indent, Vector2.up, rayLength * 0.75f, blockLayerMask);

        leftHandAntenna = Physics2D.Raycast(playerCenter, Vector2.left, 0.5f, blockLayerMask);
        rightHandAntenna = Physics2D.Raycast(playerCenter, Vector2.right, 0.5f, blockLayerMask);
        upperLeftAntenna = Physics2D.Raycast(playerCenter + Vector2.up, Vector2.left, 1f, blockLayerMask);
        upperRightAntenna = Physics2D.Raycast(playerCenter + Vector2.up, Vector2.right, 1f, blockLayerMask);

        if (leftHandAntenna && !upperLeftAntenna && climbTimer < 0) {
            transform.position = (Vector2)transform.position + (Vector2.up);
            climbTimer = 0.5f;
        } 
        if (rightHandAntenna && !upperRightAntenna &&  climbTimer < 0) {
            transform.position = (Vector2)transform.position + (Vector2.up);
            climbTimer = 0.5f;
        }


        if (centerAntenna||leftAntenna||rightAntenna) {
            if (centerAntenna) {
                //Pelaajalyttyyyn
                animS = "Squashed";               
            } else if (leftAntenna) {
                //Pyllähdä tai mahastu oikealle
                if (pm == PlayerMode.Right) {
                    animS = "Bellied_Right";
                    transform.position = (Vector2)transform.position + (Vector2.right / 2);               
                    animationTimer = 1.5f;
                } else {
                    animS = "Assed_Right";
                    transform.position = (Vector2)transform.position + (Vector2.right / 2);         
                    animationTimer = 1.5f;
                }
            } else {
                //Pyllähdy tai mahastu vasemmalle
                if (pm == PlayerMode.Left) {
                    animS = "Bellied_Left";
                    transform.position = (Vector2)transform.position + (Vector2.left / 2);        
                    animationTimer = 1.5f;
                } else {
                    animS = "Assed_Left";
                    transform.position = (Vector2)transform.position + (Vector2.left / 2);                
                    animationTimer = 1.5f;
                }
            }
            anim.Play(animS);
        }

        if (animationTimer > 0) { // Animation / Player static timer
            animationTimer -= Time.deltaTime;
        }

        if (drillTimer > 0) { // Drill cooldown timer
            drillTimer -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && drillTimer <= 0) { 
            CheckBlock(pm);
            print("poranäppäintä painettu!");
        }
    }


        void CheckBlock(PlayerMode mode) {
            drillTimer = 0.2f;
        animationTimer = 0.2f;
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
        //tarviiks muuta
    }

    void ColdAndLonelyDeath() { // Name probably says it all
        print("Aarghh!");
    }
}
