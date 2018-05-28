using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpriteChanger : MonoBehaviour {
    public SpriteRenderer topLeft;
    public SpriteRenderer topRight;
    public SpriteRenderer bottomLeft;
    public SpriteRenderer bottomRight;
    public Sprite cornerOut;
    public Sprite edge;
    public Sprite cornerIn;

    public float posx;
    public float posy;
    public BlockManager bm;

    public bool sameAbove;
    public bool sameLeft;
    public bool sameBelow;
    public bool sameRight;

    BlockScript thisBlock;
    public BlockScript leftBlock;
    public BlockScript rightBlock;
    public BlockScript upBlock;
    public BlockScript downBlock;

    List <SpriteRenderer> blockTiles;
    List<BlockScript> adjacentBlocks;

    void Awake() {
        bm = FindObjectOfType<BlockManager>();
        thisBlock = transform.parent.gameObject.GetComponent<BlockScript>();

        topLeft = transform.Find("TopLeft").GetComponent<SpriteRenderer>();
        topRight = transform.Find("TopRight").GetComponent<SpriteRenderer>();
        bottomLeft = transform.Find("BottomLeft").GetComponent<SpriteRenderer>();
        bottomRight = transform.Find("BottomRight").GetComponent<SpriteRenderer>();
        blockTiles = new List<SpriteRenderer> {bottomRight, topRight, bottomLeft, topLeft};
    }

    public void AtLevelStart () {
        FindAdjacentBlocks();
        SpriteUpdate();
    }
	
	void Update () {
        FindAdjacentBlocks();
        //foreach (BlockScript script in adjacentBlocks) {
        //    if (script.bs != BlockState.Static) {
                SpriteUpdate();
        //    }
        //} //voisko tän tehä sillee että ei kutsuta täällä updatessa vaan kutsutaan muista aina ku tarvii?
	}

    public void FindAdjacentBlocks() {
        posx = transform.parent.position.x;
        posy = transform.parent.position.y;
        //print(posx + " " + posy);
        adjacentBlocks = new List<BlockScript>();

        //if(thisblock ei oo vasemmassa reunassa eli 
        if ((int)posx != 0) {
                leftBlock = bm.blockGrid[((int)posx - 1), -(int)posy];
                //print("left " + leftBlock);
                adjacentBlocks.Add(leftBlock);
        }
        //if(thisblock ei oo oikeessa reunassa eli 
        if ((int)posx != bm.columns - 1) {
                rightBlock = bm.blockGrid[((int)posx + 1), -(int)posy];
                //print("right " + rightBlock);
                adjacentBlocks.Add(rightBlock);
        }
        //if(thisblock ei oo yläreunassa eli 
        if (-(int)posy != 0 && bm.blockGrid[(int)posx, (-(int)posy - 1)] != thisBlock && bm.blockGrid[(int)posx, (-(int)posy - 1)] != downBlock) {
                upBlock = bm.blockGrid[(int)posx, (-(int)posy - 1)];
                //print("up " + upBlock);
                adjacentBlocks.Add(upBlock);
        }
        if (-(int)posy != bm.rows - 1) { //jos y ei ole viimeinen rivi
                downBlock = bm.blockGrid[(int)posx, (-(int)posy + 1)];
                //print("down " + downBlock);
                adjacentBlocks.Add(downBlock); 
        }
    }

    public void SpriteUpdate() {

        //FindAdjacentBlocks();

        if (upBlock) {
            sameAbove = thisBlock.bc == upBlock.bc; //thisBlock.bs == upBlock.bs && 
        }
        if (downBlock) {
            sameBelow = thisBlock.bc == downBlock.bc;
        }
        if (leftBlock) {
            sameLeft = thisBlock.bc == leftBlock.bc;
        }
        if (rightBlock) {
            sameRight = thisBlock.bc == rightBlock.bc;
        }

        for (int cornerID = 0; cornerID < 4; cornerID++) {
            bool checkUp = (cornerID == 1) || (cornerID == 3); // (cornerID & 1) != 0;
            bool checkLeft = (cornerID == 2) || (cornerID == 3); // (cornerID & 2) != 0;
            bool sameAboveOrBelow = checkUp ? sameAbove : sameBelow;
            bool sameLeftOrRight = checkLeft ? sameLeft : sameRight;
            blockTiles[cornerID].color = bm.colorList[(int)thisBlock.bc];
            if (sameAboveOrBelow && sameLeftOrRight) {
                blockTiles[cornerID].sprite = cornerIn;
                if (cornerID == 0)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 180f);
                }
                else if (cornerID == 1)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 270f);
                }
                else if (cornerID == 2)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 90f);
                }
                else
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            } else if (sameAboveOrBelow && !sameLeftOrRight) {
                blockTiles[cornerID].sprite = edge;
                if (cornerID == 0)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 270f);
                }
                else if (cornerID == 1)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 270f);
                }
                else if (cornerID == 2)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 90f);
                }
                else
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 90f);
                }
            }
            else if (!sameAboveOrBelow && sameLeftOrRight)
            {
                blockTiles[cornerID].sprite = edge;
                if (cornerID == 0)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 180f);
                }
                else if (cornerID == 1)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (cornerID == 2)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 180f);
                }
                else
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            } else {
                blockTiles[cornerID].sprite = cornerOut;
                if (cornerID == 0)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 180f);
                }
                else if (cornerID == 1)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 270f);
                }
                else if (cornerID == 2)
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 90f);
                }
                else
                {
                    blockTiles[cornerID].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }

        }

    }
}