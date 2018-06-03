using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockColor {
    Red,
    Blue,
    Green,
    Yellow,
    Grey,
    Candy,
    LevelEnd
}
//väri: 5 eri väriä + sokeri

public enum BlockState {
    Static,
    Hold,
    Falling
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
    GameManager gm;
    public List<BlockScript> group;
    SpriteRenderer sr;
    public BlockScript blockBelow;
    public BlockScript blockAbove;
    public BlockScript blockLeft;
    public BlockScript blockRight;
    Vector3 below;
    Collider2D[] stuffBelow;
    Collider2D[] stuffLeft;
    Collider2D[] stuffRight;
    BlockSpriteChanger bsc;
    public bool toBeDestroyed;
    public PlayerCharacter player;
    public bool levelEnd;
    int hitsLeft = 1;
    Collider2D c2d;

    private void Awake() {
        bm = FindObjectOfType<BlockManager>().GetComponent<BlockManager>();
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        bsc = gameObject.GetComponentInChildren<BlockSpriteChanger>();
        player = FindObjectOfType<PlayerCharacter>();
        if(bc == BlockColor.Candy) {
            c2d = gameObject.GetComponent<Collider2D>();
            c2d.enabled = false;
        }
        if (bc == BlockColor.Grey) {
            hitsLeft = 5;
        }
    }

    public void AtLevelStart() {
        //kun on static tarvii vaan kerran kattoo mikä palikka on alla
        //sille täytyy kertoo mikä on sen yllä jotta se voi kutsua sitä
        below = transform.position + new Vector3(0, -1, 0);
        blockBelow = bm.FindBlock(below);
        if (blockBelow) {
            blockBelow.SetBlockAbove(this);
        }
        CheckLeft();
        CheckRight();
    }

    public void SetBlockAbove(BlockScript above) {
        blockAbove = above;
    }

    void Update() {
        if (levelEnd) {
            bs = BlockState.Static;
        }
        // if block underneath destroyed, hold & wobble for 2 seconds, fall

        //TODO: make hold work!
        //wobble

        if (bs == BlockState.Hold && holdTimer <= 0) {
            bs = BlockState.Falling;
            //holdTimer = 2f;
        }

        if (bs == BlockState.Static) {
            ////print(this + " STATE is STATIC");
            Vector3 placeToSnap = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            if (transform.position != placeToSnap) {
                transform.position = placeToSnap;
                print("snapped " + this +  " when static");
            }
            //holdTimer = 2f;
        }

    }

    void Merge(BlockScript blockInDir) {
        //print("merging " + this + " to " + blockInDir.group);
        int direction = blockInDir == blockLeft ? 1 : -1;
        if (blockInDir.bs == BlockState.Static) {
        //if (blockInDir.bs == BlockState.Static || blockInDir.bs == BlockState.Hold) {
            Vector3 placeToSnap = blockInDir.transform.position + new Vector3(direction, 0, 0);
            placeToSnap = new Vector3(Mathf.Round(placeToSnap.x), Mathf.Round(placeToSnap.y), Mathf.Round(placeToSnap.z));
            transform.position = placeToSnap;
            bs = BlockState.Static;
        }
        var tempTime = blockBelow.holdTimer;
        foreach (BlockScript block in group) { //tästä oma funktio?
            if (block.blockLeft) {
                    tempTime = tempTime < block.blockBelow.holdTimer ? tempTime : block.blockLeft.holdTimer;
                }
            }
        foreach (BlockScript block in group) {
                block.holdTimer = tempTime;
        }

        bm.SetBlockInGrid(this);
        bs = blockInDir.bs;
        bsc.SpriteUpdate();
        //bm.MergeGroups(this, blockInDir);
    }

    public bool CheckRight() {
        SetRight();
        return (blockRight && (blockRight.bs == BlockState.Static || blockRight.bs == BlockState.Hold)
            && blockRight.bc == bc && blockRight.group != group);
    }

    public BlockScript SetRight() {  //TODO: palauta blockRight
        //Vector2 centerPoint = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.45f);
        //stuffRight = Physics2D.OverlapBoxAll(centerPoint, new Vector2(0.5f, 0.2f), 0);
        stuffRight = Physics2D.OverlapPointAll(new Vector2((transform.position.x + 0.75f), (transform.position.y + 0.45f)));
        bool found = false;
        foreach (Collider2D col in stuffRight) {
            if (col != gameObject.GetComponent<Collider2D>() && col != player.GetComponent<Collider2D>()) {
                blockRight = col.gameObject.GetComponent<BlockScript>();
                found = true;
            }
        }

        if (!found) {
            blockRight = null;
        }
        //if(blockRight)
        //    blockRight.blockLeft = this;
        return blockRight;
    }

    public bool CheckLeft() {
        SetLeft();
        //print("onko palikkaa vasemmalla " + blockLeft);
        //print("onko palikka vasemmalla static tai holding " + (blockLeft && (blockLeft.bs == BlockState.Static || blockLeft.bs == BlockState.Hold)));
        return (blockLeft && (blockLeft.bs == BlockState.Static || blockLeft.bs == BlockState.Hold)
            && blockLeft.bc == bc && blockLeft.group != group);
    }

    public BlockScript SetLeft() {
        //Vector2 centerPoint = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.45f);
        //stuffLeft = Physics2D.OverlapBoxAll(centerPoint, new Vector2(0.5f, 0.2f), 0);
        stuffLeft = Physics2D.OverlapPointAll(new Vector2((transform.position.x - 0.75f), (transform.position.y + 0.45f)));
        bool found = false;
        foreach (Collider2D col in stuffLeft) {
            if (col != gameObject.GetComponent<Collider2D>() && col != player.GetComponent<Collider2D>()) {
                blockLeft = col.gameObject.GetComponent<BlockScript>();
                found = true;
            }
        }

        if (!found) {
            blockLeft = null;
        }
        //if(blockLeft)
        //    blockLeft.blockRight = this;
        return blockLeft;
    }

    public bool CheckAnyBelow() {
        if (blockBelow && blockBelow.toBeDestroyed) {
            return false;
        }
        SetBelow();
        if (blockBelow && blockBelow.levelEnd) {
            return true;
        }
        else {
            return blockBelow && !blockBelow.toBeDestroyed && (blockBelow.bs == BlockState.Static ||
            (blockBelow.group != group && blockBelow.bs == BlockState.Hold));
        }
    }

    public bool CheckSameBelow() {
        if (CheckAnyBelow()) {
            return blockBelow.bc == bc && group != blockBelow.group;
        }
        return false;
    }

    public BlockScript SetBelow() {
        //luodaan overlap joka kattoo onko alapuolella blokki
        Vector2 centerPoint = new Vector2(transform.position.x, transform.position.y - 0.54f);
        stuffBelow = Physics2D.OverlapPointAll(centerPoint);

        bool found = false;
        foreach (Collider2D col in stuffBelow) {
            if (col != gameObject.GetComponent<Collider2D>() && col != player.GetComponent<Collider2D>()) { //TODO: siisti
                blockBelow = col.gameObject.GetComponent<BlockScript>();
                found = true;
                //print("ALLA ON " + blockBelow);
                //print(this + " is on top of " + blockBelow);
            }
        }
        if(!found) {
            blockBelow = null;
        }
        return blockBelow;
    }

    public void SnapInPlace(BlockState state) { //että onko blockbelow static vai hold) {
        //print("SNAPPING " + this);
        bs = state;

        Vector3 placeToSnap = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
        //Vector3 placeToSnap = new Vector3(Mathf.Round(transform.position.x), transform.position.y, transform.position.z);
        transform.position = placeToSnap;

        bm.SetBlockInGrid(this);
        //TÄSTÄ MERGE OTETTU POIS!!!
    }

    public void SetGridPos(int posX, int posY, int columns) {
        gridPos = columns * posY + posX +1;
    }

    public void SetGroup(List<BlockScript> g) {
        group = g;
        //print("blokki nro " + gridPos + " on ryhmässä nro " + groupNumber);
    }

    public void Pop(int hits) {
        //kerro block managerille että poksahti
        //animaatio tms?
        hitsLeft = hitsLeft - hits;
        if (hitsLeft < 1) {
            Destroy(gameObject);
            gm.AddScore();
            toBeDestroyed = true;
        }


    }
}
