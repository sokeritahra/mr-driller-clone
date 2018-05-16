﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpriteChanger : MonoBehaviour {
    public SpriteRenderer topLeft;
    public SpriteRenderer topRight;
    public SpriteRenderer bottomLeft;
    public SpriteRenderer bottomRight;
    public Sprite cornerOut;
    public Sprite cornerIn;
    public Sprite edge;

    public bool sameAbove;
    public bool sameLeft;
    public bool sameBelow;
    public bool sameRight;

    List <SpriteRenderer> blockTiles;

    void Awake() {

        topLeft = transform.Find("TopLeft").GetComponent<SpriteRenderer>();
        topRight = transform.Find("TopRight").GetComponent<SpriteRenderer>();
        bottomLeft = transform.Find("BottomLeft").GetComponent<SpriteRenderer>();
        bottomRight = transform.Find("BottomRight").GetComponent<SpriteRenderer>();

        blockTiles = new List<SpriteRenderer> {bottomRight, topRight, bottomLeft, topLeft};
    }


    void Start () {
		
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpriteUpdate();
        }
	}

    public void SpriteUpdate() {
        for (int cornerID = 0; cornerID < 4; cornerID++) {
            bool checkUp = (cornerID & 1) != 0;
            bool checkLeft = (cornerID & 2) != 0;
            bool sameAboveOrBelow = checkUp ? sameAbove : sameBelow;
            bool sameLeftOrRight = checkLeft ? sameLeft : sameRight;
            //int index = cornerID + (sameAboveOrBelow ? 4 : 0) + (sameLeftOrRight ? 8 : 0);

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