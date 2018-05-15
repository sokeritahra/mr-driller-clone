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
    public int groupNumber = -1;
    SpriteRenderer sr;

    private void Awake() {
        bm = FindObjectOfType<BlockManager>().GetComponent<BlockManager>();
        //int tempInt = Random.Range(0, 3); //TODO: generalize
        //bc = (BlockColor)tempInt;
        //sr = GetComponent<SpriteRenderer>();
        //sr.color = new Color(0, tempInt / 4f, 0);
    }

    void Update () {
        // if block underneath destroyed, hold & wobble for 2 seconds, falling = true

        //if (no block underneath) {
        //wobble
        //only blockmanager can change the blockstate to hold?
        if (bs == BlockState.Hold) {
            holdTimer -= Time.deltaTime;
            //print(holdTimer);
        }

        if (holdTimer < 0) {
            bs = BlockState.Falling;
            holdTimer = 2f;
        }

        if(bs == BlockState.Falling) {
            Fall();
            //if (block underneath) {bs = BlockState.Static}
            ///then stop on top of next block OR merge with a same color block
            ///
        }

        ///when the block is falling:
        /// check if block's center would pass (by unity yksikkö?? how??)
        ///to the next ruutu where there already is a static block
        ///if would, snap to the previous ruutu
        ///if not, keep falling
        ///

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

    }

    public void SetGridPos(int posX, int posY, int columns) {
        gridPos = columns * posY + posX ;
    }

    public void SetGroupNumber(int nr) {
        groupNumber = nr;
        //print("blokki nro " + gridPos + " on ryhmässä nro " + groupNumber);
    }

    public void Pop() {
        bm.PopBlocks(gameObject);
        //kerro block managerille että poksahti
        //animaatio tms?
        Destroy(gameObject);
        print("Pop!");
    }
}
