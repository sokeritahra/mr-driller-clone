using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockColor {
    Red,
    Blue,
    Green,
    Yellow,
    Grey
}
//väri: 5 eri väriä

public enum BlockState {
    Static,
    Hold,
    Falling,
    LevelEnd
}

public class BlockScript : MonoBehaviour {

    public float velocity;
    public int gridPos;
    // paikka taulukossa?
    //ja sit onko blokki perus, X vai AIR
    public float holdTimer = 2f;
    public BlockState bs;
    public BlockColor bc;
    BlockManager bm;
    public List<BlockScript> group;
    SpriteRenderer sr;
    BlockScript blockBelow;
    BlockScript blockAbove;
    Vector3 below;
    Collider2D col;
    Collider2D[] stuffBelow;

    private void Awake() {
        bm = FindObjectOfType<BlockManager>().GetComponent<BlockManager>();
        col = gameObject.GetComponent<Collider2D>();
        //int tempInt = Random.Range(0, 3); //TODO: generalize
        //bc = (BlockColor)tempInt;
        //sr = GetComponent<SpriteRenderer>();
        //sr.color = new Color(0, tempInt / 4f, 0);
    }

    public void AtLevelStart() {
        //kun on static tarvii vaan kerran kattoo mikä palikka on alla
        //sille täytyy kertoo mikä on sen yllä jotta se voi kutsua sitä
        below = transform.position + new Vector3(0, -1, 0);
        blockBelow = bm.FindBlock(below);
        if (blockBelow) {
            print("the block above " + blockBelow + " is " + this);
            blockBelow.SetBlockAbove(this);
            print("the block below " + blockBelow.blockAbove + " is " + blockBelow);
        }

    }

    public void SetBlockAbove(BlockScript above) {
        blockAbove = above;
    }

    void FixedUpdate() {
        // if block underneath destroyed, hold & wobble for 2 seconds, fall

        //TODO: make hold work!
        //wobble

        if (bs == BlockState.Hold) {
            holdTimer -= Time.deltaTime;
            //print(holdTimer);
        }

        if (holdTimer < 0) {
            bs = BlockState.Falling;
            holdTimer = 2f;
        }

        ///then stop on top of next block OR merge with a same color block
        ///

        if (bs == BlockState.Falling) {
            //print("AAAAAAAA " + this);
            Fall();
            //    ///then stop on top of next block OR merge with a same color block
            //    ///
        }

        ///when the block is falling:
        ///check if there's a block of the same color on either side
        ///
        ///if (LeftBlock.BlockColor == gameObject.BlockColor) {
        /// do something }
        /// 
        ///if (yes and) the block's center would pass the same colored block's center,
        ///snap the centers on the same hrzntl level
        /// if there are more than 3 blocks of the same color, Pop();
    }

    void Fall() {
        transform.Translate(0, -velocity * Time.deltaTime, 0);
        //luodaan overlap joka kattoo onko alapuolella blokki
        Vector2 centerPoint = new Vector2(transform.position.x, transform.position.y - 0.25f);
        stuffBelow = Physics2D.OverlapBoxAll(centerPoint, new Vector2(0.5f, 0.5f), 0);
        //var oneBelow = col.OverlapCollider(cf, stuffBelow);
        //below = transform.position + new Vector3(0, -1, 0);
        if (stuffBelow.Length > 1) {
            print("havaittiin " + stuffBelow.Length + " blokkia colliderissa");
            }
        foreach (Collider2D col in stuffBelow) {
            if (col != gameObject.GetComponent<Collider2D>()) {
                blockBelow = col.gameObject.GetComponent<BlockScript>();
            }
        }
        
        //print(blockBelow + " is below " + this);


        if (blockBelow && blockBelow.bs == BlockState.Static) {
            //check where we are an if we're going below some line then?
            Vector3 placeToSnap = blockBelow.transform.position + new Vector3(0, 1, 0);
            print(this + " should snap on top of " + blockBelow + " at " + placeToSnap);
            transform.position = placeToSnap;
            bs = BlockState.Static;
            if (blockAbove) {
                blockAbove.transform.position = placeToSnap + new Vector3(0, 1, 0);
                blockAbove.bs = BlockState.Static;
            }
            blockBelow.SetBlockAbove(this);
            bm.SetBlockInGrid(this);
            return;
        }
        //tell the block above this one to fall as well
        if (blockAbove) {
            blockAbove.bs = BlockState.Falling;
        }
        //sit jos on niin stop falling ja transform = +1y
        /// check if block's center would pass (by unity yksikkö?? how??)
        ///to the next ruutu where there already is a static block
        ///if would, snap to the previous ruutu
        ///if not, keep falling
    }

    public void SetGridPos(int posX, int posY, int columns) {
        gridPos = columns * posY + posX;
    }

    public void SetGroup(List<BlockScript> g) {
        group = g;
        //print("blokki nro " + gridPos + " on ryhmässä nro " + groupNumber);
    }

    public void Pop() {
        //kerro block managerille että poksahti
        //animaatio tms?
        if (blockAbove) {
            blockAbove.bs = BlockState.Hold;
        }

        Destroy(gameObject);
        print("Pop!");
    }
}
