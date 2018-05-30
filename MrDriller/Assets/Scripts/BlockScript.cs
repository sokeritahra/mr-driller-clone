using System;
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
    GameManager gm;
    public List<BlockScript> group;
    SpriteRenderer sr;
    public BlockScript blockBelow;
    BlockScript blockAbove;
    BlockScript blockLeft;
    BlockScript blockRight;
    Vector3 below;
    Collider2D[] stuffBelow;
    Collider2D[] stuffLeft;
    Collider2D[] stuffRight;
    BlockSpriteChanger bsc;
    public bool toBeDestroyed;
    public PlayerCharacter player;

    private void Awake() {
        bm = FindObjectOfType<BlockManager>().GetComponent<BlockManager>();
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        bsc = gameObject.GetComponentInChildren<BlockSpriteChanger>();
        player = FindObjectOfType<PlayerCharacter>();
    }

    public void AtLevelStart() {
        //kun on static tarvii vaan kerran kattoo mikä palikka on alla
        //sille täytyy kertoo mikä on sen yllä jotta se voi kutsua sitä
        below = transform.position + new Vector3(0, -1, 0);
        blockBelow = bm.FindBlock(below);
        if (blockBelow) {
            blockBelow.SetBlockAbove(this);
        }

    }

    public void SetBlockAbove(BlockScript above) {
        blockAbove = above;
    }

    void Update() {
        // if block underneath destroyed, hold & wobble for 2 seconds, fall

        //TODO: make hold work!
        //wobble

        if (bs == BlockState.Hold) {
            //print("holding " + this);
            holdTimer -= Time.deltaTime;
            if (blockAbove && bm.CheckIfGroupOnAir(blockAbove.group)) {
                    bm.HoldBlocks(blockAbove);
            }
        }
            //print(holdTimer);

        if (bs == BlockState.Hold && holdTimer <= 0) {
            bs = BlockState.Falling;
            //holdTimer = 2f;
        }

        if (bs == BlockState.Falling) {
            //print(this + " STATE is FALLING");
            Fall();
            //if (blockBelow && !blockBelow.toBeDestroyed && blockBelow.bs == BlockState.Static) {
            //    //print(this + " is on top of " + blockBelow);
            //    //tell the group to be static
            //    foreach (BlockScript block in group) {
            //        //print(block + " going to snap in place");
            //        block.SnapInPlace();
            //        }
            //}

        }

        if (bs == BlockState.Static) {
            ////print(this + " STATE is STATIC");
            //Vector3 placeToSnap = transform.position + new Vector3(direction, 0, 0);
            //transform.position = new Vector3(Mathf.Round(placeToSnap.x), Mathf.Round(placeToSnap.y), Mathf.Round(placeToSnap.z));
            holdTimer = 2f;
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
        bm.SetBlockInGrid(this);
        bs = blockInDir.bs;
        bsc.SpriteUpdate();
        bm.MergeGroups(this, blockInDir);
    }

    void Fall() {
        CheckBelow();
        CheckLeft();
        CheckRight();

        if (blockBelow && !blockBelow.toBeDestroyed && (blockBelow.bs == BlockState.Static || 
            (blockBelow.group != group && blockBelow.bs == BlockState.Hold))) {
            if (blockBelow.bc == bc && group != blockBelow.group) {
                print("MERGE " + this + " WITH " + blockBelow);
                bm.MergeGroups(this, blockBelow);
                //Vector3 placeToSnap = blockBelow.transform.position + new Vector3(0, 1, 0);
                //transform.position = placeToSnap;
                print(transform.position + " " + blockBelow.transform.position);
                //holdTimer = blockBelow.holdTimer;
                foreach (BlockScript block in group) {
                    //print(block + " going to snap in place");
                    block.SnapInPlace(blockBelow.bs);
                }
            }
            //print(this + " is on top of " + blockBelow);
            //tell the group to be static
            foreach (BlockScript block in group) {
                //print(block + " going to snap in place");
                block.SnapInPlace(blockBelow.bs);
            }
        }

        if (blockLeft && (blockLeft.bs == BlockState.Static || blockLeft.bs == BlockState.Hold) 
            && blockLeft.bc == bc && blockLeft.group != group) {
            print(this + " MERGING " + blockLeft);
            Merge(blockLeft);
            if (blockRight && (blockRight.bs == BlockState.Static || blockRight.bs == BlockState.Hold) 
                && blockRight.bc == bc && blockRight.group != group) {
                Merge(blockRight);
                print("mergasi vasemmalle oli myös oikealla joten mergattiin myös sinne");
            }
            /// if there are more than 3 blocks of the same color, Pop();
        }
        else if (blockRight && (blockRight.bs == BlockState.Static || blockRight.bs == BlockState.Hold) 
            && blockRight.bc == bc && blockRight.group != group) {
            print(this + " MERGING " + blockRight);
            Merge(blockRight);
            /// if there are more than 3 blocks of the same color, Pop();
        }
        else {
            bm.DropBlocks(group);
            //print("A");
            bm.SetBlockInGrid(this);

        }

    }

    private bool CheckRight() {
        Vector2 centerPoint = new Vector2(transform.position.x - 0.25f, transform.position.y + 0.5f);
        stuffRight = Physics2D.OverlapBoxAll(centerPoint, new Vector2(0.5f, 0.25f), 0);

        int tempInt = 0;
        foreach (Collider2D col in stuffRight) {
            if (col != gameObject.GetComponent<Collider2D>()) {
                blockRight = col.gameObject.GetComponent<BlockScript>();
                tempInt++; //lisätään yksi tempInt:iin jos vasemmalla blokki
            }
        }
        return tempInt > 0;
    }

    public bool CheckLeft() {
        //luodaan overlap joka kattoo onko alapuolella blokki
        Vector2 centerPoint = new Vector2(transform.position.x - 0.25f, transform.position.y + 0.5f);
        stuffLeft = Physics2D.OverlapBoxAll(centerPoint, new Vector2(0.5f, 0.5f), 0);

        int tempInt = 0;
        foreach (Collider2D col in stuffLeft) {
            if (col != gameObject.GetComponent<Collider2D>()) {
                blockLeft = col.gameObject.GetComponent<BlockScript>();
                tempInt++; //lisätään yksi tempInt:iin jos vasemmalla blokki
            }
        }
        return tempInt > 0;
    }


    public bool CheckBelow() {
        //luodaan overlap joka kattoo onko alapuolella blokki
        Vector2 centerPoint = new Vector2(transform.position.x, transform.position.y - 0.25f);
        stuffBelow = Physics2D.OverlapBoxAll(centerPoint, new Vector2(0.5f, 0.5f), 0);

        //if (col != gameObject.GetComponent<Collider2D>()) {
        //    blockBelow = col.gameObject.GetComponent<BlockScript>();
        //    if (blockBelow.toBeDestroyed) {  //lisätään yksi tempInt:iin jos alapuolella blokki
        //        tempInt++;
        //    }
        //    //print(this + " is on top of " + blockBelow);
        //}

        //jos alla on pelaaja ja pelaaja if (stuffBelow.contains
        int tempInt = 0;
        foreach (Collider2D col in stuffBelow) {
            if (col != gameObject.GetComponent<Collider2D>() && col != player) {
                blockBelow = col.gameObject.GetComponent<BlockScript>();
                //print("ALLA ON " + blockBelow);
                tempInt++; //lisätään yksi tempInt:iin jos alapuolella blokki
                //print(this + " is on top of " + blockBelow);
            }
        }
        if(blockBelow && blockBelow.toBeDestroyed) {
            return false;
        }
        return tempInt > 0;
        //print(blockBelow + " is below " + this); 
    }

    public void SnapInPlace(BlockState state) { //että onko blockbelow static vai hold) {
        //print("SNAPPING " + this);
        bs = state;
        if(blockBelow) {
            holdTimer = blockBelow.holdTimer;
        }
        else {
            holdTimer = 2f;
        }
                //mistä voin ottaa holdTimerin jos en blockbelowstA? jonkun muun ??
        //vois odottaa että alempi alkaa pudota ja sitten vasta jatkaa putoamista
        //ei saa snäpätä blockbelow:n päälle, pitää snäpätä siihen missä tällä hetkellä on.
        //Vector3 placeToSnap = blockBelow.transform.position + new Vector3(0, 1, 0);
        //transform.position = placeToSnap;
        Vector3 placeToSnap = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
        //Vector3 placeToSnap = new Vector3(Mathf.Round(transform.position.x), transform.position.y, transform.position.z);
        transform.position = placeToSnap;
        //print("STOP " + this);
        CheckBelow();
        if (blockBelow) {
            placeToSnap = blockBelow.transform.position + new Vector3(0, 1, 0);
            blockBelow.SetBlockAbove(this);
        }
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

    public void Pop() {
        //kerro block managerille että poksahti
        //animaatio tms?
        
        Destroy(gameObject);
        gm.AddScore();
        toBeDestroyed = true;

        if (blockAbove && bm.CheckIfGroupOnAir(blockAbove.group)) {
            bm.HoldBlocks(blockAbove); 
        }

        //print("Pop!");
    }
}
