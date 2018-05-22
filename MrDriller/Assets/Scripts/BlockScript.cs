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
    Collider2D[] stuffBelow;
    BlockSpriteChanger bsc;

    private void Awake() {
        bm = FindObjectOfType<BlockManager>().GetComponent<BlockManager>();
        bsc = gameObject.GetComponentInChildren<BlockSpriteChanger>();
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
            CheckBelow();
            if (bsc.leftBlock && bsc.leftBlock.bs == BlockState.Static && bsc.leftBlock.bc == bc) {
                bs = BlockState.Static;
            }
            else if (bsc.rightBlock && bsc.rightBlock.bs == BlockState.Static && bsc.rightBlock.bc == bc) {
                bs = BlockState.Static;
            }
            else {
                holdTimer -= Time.deltaTime;
            }
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


    }

    void Merge(BlockScript blockInDir) {        
        int direction = blockInDir == bsc.leftBlock ? 1 : -1;
        Vector3 placeToSnap = blockInDir.transform.position + new Vector3(direction, 0, 0);
        transform.position = placeToSnap;
        bm.SetBlockInGrid(this);
        bs = BlockState.Static;
        bsc.SpriteUpdate();
        if (this.group != blockInDir.group) {
            bm.MergeGroups(this, blockInDir);
        }
        //change group

    }

    //void MergeRight() {  bool sameAboveOrBelow = checkUp ? sameAbove : sameBelow;
    //    print("same color on the right");
    //    Vector3 placeToSnap = bsc.rightBlock.transform.position + new Vector3(-1, 0, 0);
    //    transform.position = placeToSnap;
    //    bm.SetBlockInGrid(this);
    //    bs = BlockState.Static;
    //    bsc.SpriteUpdate();
    //    //change group?

    //}

    void Fall() {

        if (bsc.leftBlock && bsc.leftBlock.bs == BlockState.Static && bsc.leftBlock.bc == bc) {
            Merge(bsc.leftBlock);
            if (bsc.rightBlock && bsc.rightBlock.bs == BlockState.Static && bsc.rightBlock.bc == bc) {
                Merge(bsc.rightBlock);
            }
            /// if there are more than 3 blocks of the same color, Pop();
        }
        else if (bsc.rightBlock && bsc.rightBlock.bs == BlockState.Static && bsc.rightBlock.bc == bc) {
            Merge(bsc.rightBlock);
            /// if there are more than 3 blocks of the same color, Pop();
        }
        else {
            transform.Translate(0, -velocity * Time.deltaTime, 0);
            bm.SetBlockInGrid(this);
            CheckBelow();
        }
    }

    void CheckBelow() {
        //luodaan overlap joka kattoo onko alapuolella blokki
        Vector2 centerPoint = new Vector2(transform.position.x, transform.position.y - 0.25f);
        stuffBelow = Physics2D.OverlapBoxAll(centerPoint, new Vector2(0.5f, 0.5f), 0);

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
            if (blockBelow.bc == bc) {
                if (this.group != blockBelow.group) {
                    bm.MergeGroups(this, blockBelow);
                }
            }
            print("the block has moved in the grid to " + gridPos);
        }
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
            bm.HoldBlocks(blockAbove); 
        }

        Destroy(gameObject);
        print("Pop!");
    }
}
