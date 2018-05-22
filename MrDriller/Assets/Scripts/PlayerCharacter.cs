using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerMode { Up, Down, Left, Right, Falling}; // Modes for the sprites and drill directions

public class PlayerCharacter : MonoBehaviour {
    Rigidbody2D rb;
    Collider2D c;
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
    string animS = "";
    public float drillDepth = 0.75f;
    float rayLength = 0.6f;
    int layerMask = 1 << 9;
    Vector2 indent = new Vector2(0.1f,0);
    RaycastHit2D hitRight;
    RaycastHit2D hitLeft;
    RaycastHit2D centerAntenna;
    RaycastHit2D leftAntenna;
    RaycastHit2D rightAntenna;
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
        hitLeft = Physics2D.Raycast(playerLeft, Vector2.down, rayLength, layerMask);
        hitRight = Physics2D.Raycast(playerRight, Vector2.down, rayLength, layerMask);
        return (hitLeft || hitRight);
    }

    private void Update() {
        // Debug drawings of playercharacter head antennas and falling sensors
        Debug.DrawRay(playerCenter, Vector2.up * (rayLength * 0.75f), Color.red);
        Debug.DrawRay(playerLeft + indent, Vector2.up * (rayLength * 0.75f), Color.green);
        Debug.DrawRay(playerRight - indent, Vector2.up * (rayLength * 0.75f), Color.blue);
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
        }else {
            pm = previousPm;
            if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) { // Set player (drilling) mode
                if (vertical < 0) {
                    pm = PlayerMode.Down;
                    previousPm = pm;
                    anim.Play("Aim_Down");
                }
                if (vertical > 0) {
                    pm = PlayerMode.Up;
                    previousPm = pm;
                    anim.Play("Aim_Up");
                }
            }
            else {
                if (horizontal > 0) {
                    pm = PlayerMode.Right;
                    previousPm = pm;
                    anim.Play("Aim_Right");
                }
                if (horizontal < 0) {
                    pm = PlayerMode.Left;
                    previousPm = pm;
                    anim.Play("Aim_Left");
                }
            }
            //anim.Play(animS);
        
        }

        
        // Head antennas
        centerAntenna = Physics2D.Raycast(playerCenter, Vector2.up, rayLength * 0.75f, layerMask);
        leftAntenna = Physics2D.Raycast(playerLeft + indent, Vector2.up, rayLength * 0.75f, layerMask);
        rightAntenna = Physics2D.Raycast(playerRight - indent, Vector2.up, rayLength * 0.75f, layerMask);

        // What happens when head antennas collide with a block
        if (centerAntenna||leftAntenna||rightAntenna) {
            if (centerAntenna) {
                //Pelaajalyttyyyn
                animS = "Squashed";
                print("Lyttyyn meni");
                
            } else if (leftAntenna) {
                //Pyllähdä tai mahastu oikealle
                if (pm == PlayerMode.Right) {
                    animS = "Bellied_Right";
                    
                    print("Pitäs mahastua oikealle");
                } else {
                    animS = "Assed_Right";
                    
                    print("Pitäs pyllähtää oikealle");
                }
                print("Pitäs pyllähtää oikealle");
            } else {
                //Pyllähdy tai mahastu vasemmalle
                if (pm == PlayerMode.Left) {
                    animS = "Bellied_Left";
                    
                    print("Pitäs mahastua vasemmalle");
                } else {
                    animS = "Assed_Left";
                   
                    print("Pitäs pyllähtää vasemmalle");
                }
            }
            anim.Play(animS);
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
            drillTimer = 0.5f;
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
