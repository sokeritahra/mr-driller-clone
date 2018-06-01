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
        GenerateBlocks();
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
            for (int j = 0; j < rows-1; j++) {
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

       foreach (BlockScript block in group) {
                block.transform.Translate(0, (-block.velocity * Time.deltaTime), 0);
            }
        }

    void Update() {
        List<List<BlockScript>> toBeRemoved = new List<List<BlockScript>>();

        foreach (List<BlockScript> g in AllGroups) {
            //tsekkaa putoavista snäppääkö
            int falling = 0;
            int holding = 0;
            foreach (BlockScript block in g) {
                if (block.bs == BlockState.Falling) {
                    falling++;
                }
                else if (block.bs == BlockState.Hold) {
                    holding++;
                }
            }

            //voiko tässä haitata se missä järjestyksessä foreach käy ryhmät läpi?
            //TODO: JOSKUS TÄÄ KELLUMAAN??? LUULEE ETTÄ ALLA ON JOTAIN VAIKKEI OLE

            if (falling == g.Count) {
                List<List<BlockScript>> merges = new List<List<BlockScript>>();
                int snaps = 0;
                foreach (BlockScript block in g) {
                    //katsotaan onko samaa väriä ja eri ryhmässä eli pitääkö mergee
                    //mutta pitäis kattoo myös pitääkö vain snäpätä
                    if (block.CheckSameBelow()) {
                        merges.Add(block.blockBelow.group);
                    }
                    else if (block.CheckAnyBelow()) {
                        snaps++;
                    }

                    if (block.CheckLeft()) {
                        merges.Add(block.blockLeft.group);
                        print("merging from the LEFT " + block.blockLeft + " with " + block);
                        //jos jokin näistä on static, snäpätessä staticiks
                    }

                    if (block.CheckRight()) {
                        merges.Add(block.blockRight.group);
                        print("merging from the RighT " + block.blockRight + " with " + block);
                        //jos jokin näistä on static, snäpätessä staticiks
                    }
                }
                if (merges.Count > 0) {

                    foreach (List<BlockScript> mg in merges) {
                        MergeGroups(mg, g);
                        toBeRemoved.Add(mg);
                        print("merging " + mg + " with " + g);
                    }
                    //HUOM HUOM! TÄMÄ SEURAAVA PÄTEE SIIS VAIN MERGEÄVILLE RYHMILLE
                    //SEN PITÄISI TOIMIA MYÖS ERIVÄRISILLE RYHMILLE!
                    //TODOOOOOOOOOOOOOOOOOO!!!
                    float holdTime = 2f;
                    foreach (BlockScript block in g) {
                        holdTime = holdTime < block.holdTimer ? holdTime : block.holdTimer;
                    }
                    foreach (BlockScript block in g) {
                        block.holdTimer = holdTime;
                        block.SnapInPlace(BlockState.Hold);
                        print("snapping and changing to hold " + block + " and holdtime is " + block.holdTimer);
                    }
                    //TODO: snäppäys - voinko tehä snäppifunktion joka toimii sekä sivuille että alas?
                    //voiko toimia että sen laittaa staticiksi - muuttuuko holdiksi at all tai tarpeeks nopee?
                    //tai siis eihän se tietenkään mihinkään muutu vaan se pitää laittaa
                    //mutta niin se laitetaankin tuolla else-kohdassa

                    //MILLON TÄÄ PITÄÄ TEHÄ bm.SetBlockInGrid(this);
                }
                else if (snaps > 0) {
                    foreach (BlockScript block in g) {
                        block.SnapInPlace(BlockState.Static);
                        //block.SnapInPlace(block.blockBelow.bs);
                        //TODO: tässä tapahtuu nyt niin että jokainen blokki ottaa alapuolella sijaitsevasta blokista erikseen sen
                        //blockstaten ja sen pitäis mennä sillee että jos jokin niistä (blockbelow) on static sen pitäis olla kaikille 
                        //g:ssä static ja jos ne kaikki (blockbelow) on hold, blockille myös hold
                    }
                }
                else {
                    DropBlocks(g);
                    foreach (BlockScript block in g) {
                        SetBlockInGrid(block);
                        block.SetBelow();
                        block.SetLeft();
                        block.SetRight();
                        //PITÄISKÖ NÄITÄ PÄIVITELLÄ MUULLOINKIN?
                    }
                }
            }

            else if (holding == g.Count) {
                foreach (BlockScript block in g) {
                    block.SetBelow();
                    block.SetLeft();
                    block.SetRight();
                }
                //print("holding");

                foreach (BlockScript block in g) {
                    block.holdTimer -= Time.deltaTime;
                }
                // tällä tavalla olin blockscriptin snapissa tätä asiaa käsitellyt;
                //if (blockBelow) {
                //    var tempTime = blockBelow.holdTimer;
                //    foreach (BlockScript block in group) {
                //        if (block.blockBelow) {
                //            tempTime = tempTime < block.blockBelow.holdTimer ? tempTime : block.blockBelow.holdTimer;
                //        }
                //    }
                //    foreach (BlockScript block in group) {
                //        block.holdTimer = tempTime;
                //    }
                //    placeToSnap = blockBelow.transform.position + new Vector3(0, 1, 0);
                //    blockBelow.SetBlockAbove(this);
                //}

                //else {
                //    print("no block below " + this);
                //    holdTimer = 2f;
                //}
                /// ehkä täällä vois olla holdtimer? mut sit se pitäis olla joka ryhmälle erikseen :G pitäisköhän sittenki jättää se joka 
                /// blokkiin ja sitte vaa käsitellä sitä könttänä aina foreach loopeilla
                //    

            }
            //tsekkaa ei-putoavista pitäiskö pudota
            else {
                foreach (BlockScript block in g) {
                    block.SetBelow();
                    block.SetLeft();
                    block.SetRight();
                }
                if (CheckIfGroupOnAir(g)) {
                    print("Group's states changed to hold");
                    foreach (BlockScript block in g) {
                        block.bs = BlockState.Hold;
                        block.holdTimer = 2f;
                        print("group contains " + block);
                    }
                }
                //kyllähän pitää pudota myös sillon ku alapuolella olevat putoo?
                //riittääkö??
                //    if (blockAbove && blockAbove.group != group && bm.CheckIfGroupOnAir(blockAbove.group)) {
                //            bm.HoldBlocks(blockAbove);
                //    }
                }

            foreach (BlockScript block in g) {
                block.SetBelow();
                block.SetLeft();
                block.SetRight();
            }
        }

        foreach (List<BlockScript> g in toBeRemoved) {
            AllGroups.Remove(g);
        }

        //millon tsekataan mikä puhkeaA?

        //taulukko ja blokin transform vastaa toisiaan kun taulukon ruutu on 1 unity-yksikkö * 1 unity-yksikkö
    }

    public void PopBlocks(BlockScript popped) {
        print(popped);
        foreach (BlockScript block in popped.group) {
                block.Pop();
        }
        AllGroups.Remove(popped.group);
    }

    public void HoldBlocks(BlockScript toFall) {
        foreach (BlockScript block in toFall.group) {
            block.bs = BlockState.Hold;
        }

        
    }

    public void MergeGroups(List<BlockScript> first, List<BlockScript> second) { //merge first to second
        foreach (BlockScript block in first) {
            block.SetGroup(second);
            second.Add(block);
        }
    }

    public bool CheckIfGroupOnAir(List<BlockScript> group) {
        //kattoo onko alla tyhjää
        int tempInt = 0;
        foreach (BlockScript block in group) {
            //if (!block.CheckBelow()) { //toimiiko / miten pitää tehä jos checkbelow palauttaa blokin??
            if ((block.CheckAnyBelow() && block.blockBelow.group != block.group && block.blockBelow.bs == BlockState.Static)
                || block.levelEnd || (block.blockBelow && block.blockBelow.levelEnd)) {
                tempInt++; //lisätään yksi, jos alla on blokki joka on eri ryhmässä ja static tai jos alla on tai blokki itse on levelEnd
            }
        }
        return !(tempInt > 0); //jos enemmän kuin yksi palautetaan false
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
