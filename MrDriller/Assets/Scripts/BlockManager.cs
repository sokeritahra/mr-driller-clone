using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    //taulukko
    public BlockScript[,] blockGrid;
    public int rows;
    //how many rows (how tall the grid is -> y)
    public int columns;
    //how many columns (how wide the grid is -> x)
    public GameObject blockPrefab;
    public PlayerCharacter player;
    public Vector2 firstBlock = new Vector2(0,0);
    //blokki prefabi
    BlockScript blockScript;
    public Transform blockFolder;
    //Block testBlock;
    List<List<BlockScript>> AllGroups;
    public BlockScript[] blockArray;
    float posx = 0;
    float posy = 0;
    public Color[] colorList;

    public void AtLevelStart() {
        //luodaan taulukko ja generoidaan blokit sinne
        //GenerateBlocks();
        blockGrid = new BlockScript[columns, rows];
        FindGroups();
        foreach (BlockScript block in blockArray) {
            block.AtLevelStart();
        }
        player = FindObjectOfType<PlayerCharacter>();
    }

    void GenerateBlocks() {
        float firstX = firstBlock.x;
        float firstY = firstBlock.y;

        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                GameObject go = Instantiate(blockPrefab);
                go.name = ((i * rows) + j).ToString();
                Vector2 newPosition = new Vector2(firstX + i, firstY - j);
                blockScript = go.GetComponent<BlockScript>();
                go.transform.parent = blockFolder;
                var blockColorRandomizer = Random.value;
                if (blockColorRandomizer > .75f) {
                    blockScript.bc = BlockColor.Blue;
                }
                else if (blockColorRandomizer > .50f) {
                    blockScript.bc = BlockColor.Green;
                }
                else if (blockColorRandomizer > .25f) {
                    blockScript.bc = BlockColor.Red;
                }
                else {
                    blockScript.bc = BlockColor.Yellow;
                }
                go.transform.position = newPosition;
            }
        }
    }

    public void SetBlockInGrid (BlockScript block) {
        posx = block.transform.position.x;
        posy = block.transform.position.y;
        blockGrid[Mathf.RoundToInt(posx), -Mathf.RoundToInt(posy)] = block;
        blockGrid[Mathf.RoundToInt(posx), -Mathf.RoundToInt(posy)].SetGridPos(Mathf.RoundToInt(posx), -Mathf.RoundToInt(posy), columns);
    }

    void FindGroups() {

        blockArray = FindObjectsOfType<BlockScript>();

        foreach (BlockScript block in blockArray) {
            SetBlockInGrid(block);
        }

        //rows = -(int)posy;
        //columns = (int)posx;
        // ei tää toimi näin kun tän pitäis toimia niin että otetaan yhen palikan transform ja 
        // se sit kertoo monesko palikka ja missä kohtaa ruudukkoa blockGrid[x, y] = block;

        AllGroups = new List<List<BlockScript>>();
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {

                var thisSquare = blockGrid[x, y];

                if (x != 0 && y != 0) {

                    var leftSquare = blockGrid[x - 1, y];
                    var topSquare = blockGrid[x, y - 1];

                    if (thisSquare.bc == leftSquare.bc && thisSquare.bc == topSquare.bc) {
                        //jos on samanvärinen sekä vasemmalla että ylempänä lisätään ylempään
                        topSquare.group.Add(thisSquare);
                        //kutsu spritefunktiota
                        thisSquare.SetGroup(topSquare.group);

                        if (leftSquare.group != topSquare.group) {
                            //jos vasemmalla olevat blokit ei vielä samassa ryhmässä, lisätään ne samaan ryhmään
                            foreach (BlockScript blockOnTheLeft in leftSquare.group) {
                                if (blockOnTheLeft != leftSquare) {
                                    topSquare.group.Add(blockOnTheLeft);
                                    blockOnTheLeft.SetGroup(topSquare.group);
                                }
                            }
                            topSquare.group.Add(leftSquare);
                            //ja tuhotaan sen ryhmä
                            AllGroups.Remove(leftSquare.group);
                            //asetetaan viittaus oikeaan ryhmään -- tämän saa tehdä vasta tuhoamisen jälkeen!!
                            leftSquare.SetGroup(topSquare.group);

                            //print("this block " + thisSquare.gridPos + " and the one(s) on the left added to the one on top");
                        }
                        else {
                            //jos vasemmalla ja ylhäällä olevat blokit on samassa ryhmässä, mitään ei tuhota 
                            //TODO: vaihda blokin ryhmänumero: thisSquare.SetGroupNumber(
                            //print("this block " + thisSquare.gridPos + " and the blocks on the top and left added to the same group!");

                        }
                    }

                    else {
                        //print("nyt ei ole " + thisSquare + " ja vasen ja yläkerta samat");
                        //onko vasemmalla
                        CheckOtherSquare(leftSquare, thisSquare);
                        //onko ylhäällä
                        CheckOtherSquare(topSquare, thisSquare);
                    }
                }

                else if (x != 0) {

                    var leftSquare = blockGrid[x - 1, y];

                    CheckOtherSquare(leftSquare, thisSquare);


                }

                else if (y != 0) {

                    var topSquare = blockGrid[x, y - 1];

                    CheckOtherSquare(topSquare, thisSquare);

                }

                else {
                    List<BlockScript> tempList = new List<BlockScript> { thisSquare };
                    AllGroups.Add(tempList);
                    //print("moi");
                    //kerro blockscriptille missä ryhmässä se on
                    //int tempInt = AllGroups.FindIndex(l => l == tempList);
                    thisSquare.SetGroup(tempList);
                }
            }
        }
        foreach (List<BlockScript> group in AllGroups) {
            string juttu = "";
            foreach (BlockScript bs in group) {
                juttu += bs.gridPos;
                juttu += " ";
            }
            print(AllGroups.IndexOf(group) + " : " + juttu);
        }

        //print(" täsä kaikki: " + AllGroups);
    }

    // tee tästä semmonen että poistetaan ryhmä jossa ei oo enää ketää et jos checcaa vasemman ni sit ei tee heti omaa ryhmää
    //vaan vasta sitte jos ylhäälläkään ei oo :sob: void LeftCheck()
    void CheckOtherSquare(BlockScript otherSquare, BlockScript thisSquare) {
        if (thisSquare.bc == otherSquare.bc) {
            //print("samanväriset " + thisSquare + " ja " + otherSquare);
            //samanvärisiä joten laitetaan samaan ryhmään ja tuhotaan edellinen
            otherSquare.group.Add(thisSquare);
            AllGroups.Remove(thisSquare.group);

            //tämän saa tehdä vasta tuhoamisen jälkeen!!
            thisSquare.SetGroup(otherSquare.group);

        }
        else if (thisSquare.group.Count == 0) {
            //tee uusi ryhmä
            List<BlockScript> tempList = new List<BlockScript> { thisSquare };
            AllGroups.Add(tempList);
            // int tempInt = AllGroups.IndexOf(tempList);
            thisSquare.SetGroup(tempList);
            //print("uusi ryhmä tehty blokille " + thisSquare + " ryhmä: " + AllGroups.IndexOf(tempList));
        }
        else {
            print(thisSquare + ":lle luotiin ryhmä " + AllGroups.IndexOf(thisSquare.group));
        }

    }

    public BlockScript FindBlock(Vector3 place) {

        if (-place.y < rows) {
           
            return blockGrid[(int)place.x, -(int)place.y];
        }
        else {
            //print("palautetaan palikka 0");
            return null;
            //jos yritetään katsoa alimman rivin alapuolelta, palautetaan palikka 0
        }
    }

    public void DropBlocks(List<BlockScript> group) {
        //int anyStatic = 0;
        ////print(group.Count);
        //foreach (BlockScript block in group) {
        //    if (block.bs == BlockState.Static) {
        //        anyStatic++;
        //    }
        //}
        //if (anyStatic == 0) {
            //print("pitäis tippuu");
       foreach (BlockScript block in group) {
                block.transform.Translate(0, (-block.velocity * Time.deltaTime), 0);
                //print("blokki " + block + " liikkuu nopeudella " + (-block.velocity) / group.Count + " kun frame on " + Time.frameCount);
            }
        }

    void Update() {

        foreach (List<BlockScript> g in AllGroups) {
            //tsekkaa putoavista snäppääkö
            int falling = 0;
            foreach (BlockScript block in g) {
                if (block.bs == BlockState.Falling) {
                    falling++;
                }
            }

            if (falling == g.Count) {
                List<List<BlockScript>> snaps = new List<List<BlockScript>>();
                foreach (BlockScript block in g) {
                    //tää ei vielä kato onko samaa väriä, onko eri ryhmässä
                    if (block.CheckBelow()) {
                        snaps.Add(block.blockBelow.group);
                    }
                    //toimisko tämä: snaps.Add(block.CheckBelow().group) jos checkbelow palauttaa blockbelown
                    if (block.CheckLeft()) {
                        snaps.Add(block.blockLeft.group);
                    }

                    if (block.CheckRight()) {
                        snaps.Add(block.blockRight.group);
                    }
                }
                if (snaps.Count > 0) {
                    snaps.Add(g);
                    //merge kaikki snapslistassa
                }
                else {
                    //fall
                }

            }

            //tsekkaa ei-putoavista pitäiskö pudota
            if (falling < g.Count ) {

            }

        }



        //if (blockGrid[0, 0] != null) {
        //    testBlock = blockGrid[0, 0].GetComponent<Block>();
        //    testBlock.Pop();
        //}

        //taulukko ja blokin transform vastaa toisiaan kun taulukon ruutu on 1 unity-yksikkö * 1 unity-yksikkö
    }

    //void Fall() {
    //    CheckBelow();
    //    CheckLeft();
    //    CheckRight();

    //    if (blockBelow && !blockBelow.toBeDestroyed && (blockBelow.bs == BlockState.Static || 
    //        (blockBelow.group != group && blockBelow.bs == BlockState.Hold))) {
    //        if (blockBelow.bc == bc && group != blockBelow.group) {
    //            print("MERGE " + this + " WITH " + blockBelow);
    //            bm.MergeGroups(this, blockBelow);
    //            //Vector3 placeToSnap = blockBelow.transform.position + new Vector3(0, 1, 0);
    //            //transform.position = placeToSnap;
    //            print(transform.position + " " + blockBelow.transform.position);
    //            //holdTimer = blockBelow.holdTimer;
    //            foreach (BlockScript block in group) {
    //                //print(block + " going to snap in place");
    //                block.SnapInPlace(blockBelow.bs);
    //            }
    //        }
    //        //print(this + " is on top of " + blockBelow);
    //        //tell the group to be static
    //        foreach (BlockScript block in group) {
    //            //print(block + " going to snap in place");
    //            block.SnapInPlace(blockBelow.bs);
    //        }
    //    }


    //    if (blockLeft && (blockLeft.bs == BlockState.Static || blockLeft.bs == BlockState.Hold) 
    //        && blockLeft.bc == bc && blockLeft.group != group) {
    //        print(this + " MERGING LEFT " + blockLeft);
    //        Merge(blockLeft);
    //        if (blockRight && (blockRight.bs == BlockState.Static || blockRight.bs == BlockState.Hold) 
    //            && blockRight.bc == bc && blockRight.group != group) {
    //            Merge(blockRight);
    //            print("mergasi vasemmalle oli myös oikealla joten mergattiin myös sinne");
    //        }
    //        /// if there are more than 3 blocks of the same color, Pop();
    //    }
    //    else if (blockRight && (blockRight.bs == BlockState.Static || blockRight.bs == BlockState.Hold) 
    //        && blockRight.bc == bc && blockRight.group != group) {
    //        print(this + " MERGING RIGHT " + blockRight);
    //        Merge(blockRight);
    //        /// if there are more than 3 blocks of the same color, Pop();
    //    }
    //    else {
    //        bm.DropBlocks(group);
    //        //print("A");
    //        bm.SetBlockInGrid(this); // ?

    //    }

    //}

    public void PopBlocks(BlockScript popped) {
        print(popped);
        foreach (BlockScript block in popped.group) {
                block.Pop();
        }
    }

    public void HoldBlocks(BlockScript toFall) {
        foreach (BlockScript block in toFall.group) {
            block.bs = BlockState.Hold;
        }

        //if (bs == BlockState.Hold) {
        //    //print("holding " + this);
        //    holdTimer -= Time.deltaTime;
        //    if (blockAbove && blockAbove.group != group && bm.CheckIfGroupOnAir(blockAbove.group)) {
        //            bm.HoldBlocks(blockAbove);
        //    }
        //    Vector3 placeToSnap = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        //    if (transform.position != placeToSnap) {
        //        transform.position = placeToSnap;
        //        //print("snapped " + this + " when holding");
        //    }
        //}
        //if (bs == BlockState.Hold && holdTimer <= 0) {
        //    bs = BlockState.Falling;
        //    holdTimer = 2f;
        //}
    }

    //public void StopBlocks(List<BlockScript> group) {
    //    foreach (BlockScript block in group) {
    //        block.bs = BlockState.Static;
    //        print("estää " + block + ":n putoamisen");
    //    }
    //}

    public void MergeGroups(BlockScript first, BlockScript second) {
        foreach (BlockScript block in first.group) {
            block.SetGroup(second.group);
            second.group.Add(block);
        }
    }

    public bool CheckIfGroupOnAir(List<BlockScript> group) {
        //kattoo onko alla tyhjää
        int tempInt = 0;
        foreach (BlockScript block in group) {
            if (block.CheckBelow() && block.blockBelow.group != block.group && block.blockBelow.bs == BlockState.Static) {
                tempInt++;
            }
        }
        return !(tempInt > 0);
    }

    

    //public void DestroyThreeColumnsOnTop() {
    //    for (int i = rows-Mathf.Abs(Mathf.RoundToInt(player.transform.position.y)); i > 0; i--) {
    //        // Destroy blocks without adding to score
    //        Mathf.RoundToInt(player.transform.position.x);
    //        Mathf.RoundToInt(player.transform.position.x - 1);
    //        Mathf.RoundToInt(player.transform.position.x + 1);
    //    }   
    //}
    

}
