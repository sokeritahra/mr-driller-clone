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
    //blokki prefabi
    BlockScript blockScript;
    //Block testBlock;
    List<List<BlockScript>> AllGroups;

    public void AtLevelStart () {
        //luodaan taulukko ja generoidaan blokit sinne
        blockGrid = new BlockScript[columns, rows];

        //for (int x = 0; x < columns; x++) {
        //    for (int y = 0; y < rows; y++) {
        //        Vector3 pos = new Vector3(x, -y, 0);
        //        //TODO: pos so that blocks are in the right place in the scene
        //        //also where is the first row?
        //        GameObject tempObj = Instantiate(blockPrefab, pos, Quaternion.identity);

        //        blockGrid[x,y] = tempObj.GetComponent<BlockScript>();
        //        blockGrid[x, y].SetGridPos(x, y, columns);

        //    }
        //}
        FindGroups();
    }

    void FindGroups() {

        var blockArray = FindObjectsOfType<BlockScript>();

        float posx = 0;
        float posy = 0;

        foreach ( BlockScript block in blockArray) {
            posx = block.transform.position.x;
            posy = block.transform.position.y;
            blockGrid[(int)posx, -(int)posy] = block;
            blockGrid[(int)posx, -(int)posy].SetGridPos((int)posx, -(int)posy, columns);
            print(posx + " mitä " + posy + " numero " + blockGrid[(int)posx, -(int)posy].gridPos);
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
                        AllGroups[topSquare.groupNumber].Add(thisSquare);
                        thisSquare.SetGroupNumber(topSquare.groupNumber);

                        if (leftSquare.groupNumber != topSquare.groupNumber) {
                            AllGroups[topSquare.groupNumber].Add(leftSquare);
                            AllGroups.RemoveAt(leftSquare.groupNumber);
                            //jos vasemmalla oleva blokki ei vielä samassa ryhmässä, lisätään se samaan ryhmään
                            leftSquare.SetGroupNumber(topSquare.groupNumber);
                            //ja tuhotaan sen ryhmä
                            print("this block " + thisSquare.gridPos + " and the one(s) on the left added to the one on top");
                        }
                        else {
                            //jos vasemmalla ja ylhäällä olevat blokit on samassa ryhmässä, mitään ei tuhota 
                            //TODO: vaihda blokin ryhmänumero: thisSquare.SetGroupNumber(
                            print("this block " + thisSquare.gridPos + " and the blocks on the top and left added to the same group!");

                        }
                    }

                    else {
                        print("nyt ei ole tämä ja vasen ja yläkerta samat");
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
                    print("moi");
                    //kerro blockscriptille missä ryhmässä se on
                    int tempInt = AllGroups.FindIndex(l => l == tempList);
                    thisSquare.SetGroupNumber(tempInt);
                }
                }
            }
        foreach (List<BlockScript> group in AllGroups) {
            string juttu = "";
            foreach (BlockScript bs in group) {
                juttu += bs.gridPos;
            }
            print(AllGroups.IndexOf(group) + " : " + juttu);
        }
        }

    // tee tästä semmonen että poistetaan ryhmä jossa ei oo enää ketää et jos checcaa vasemman ni sit ei tee heti omaa ryhmää
    //vaan vasta sitte jos ylhäälläkään ei oo :sob: void LeftCheck()
    void CheckOtherSquare(BlockScript otherSquare, BlockScript thisSquare) {
        if (thisSquare.bc == otherSquare.bc) {
            print("samanväriset");
            //samanvärisiä joten laitetaan samaan ryhmään
            AllGroups[otherSquare.groupNumber].Add(thisSquare);
            thisSquare.SetGroupNumber(otherSquare.groupNumber);

        }
        else if (thisSquare.groupNumber == -1) {
            //tee uusi ryhmä
            List<BlockScript> tempList = new List<BlockScript> { thisSquare };
            AllGroups.Add(tempList);
            int tempInt = AllGroups.FindIndex(l => l == tempList);
            thisSquare.SetGroupNumber(tempInt);
        }
        else {
            print(thisSquare.gridPos + " on ryhmässä " + thisSquare.groupNumber);
        }

    }

	
	void Update () {
        //if (blockGrid[0, 0] != null) {
        //    testBlock = blockGrid[0, 0].GetComponent<Block>();
        //    testBlock.Pop();
        //}

		//tsekataan mitkä blokit on yhdessä
        //(mitkä on staattisia ja mitkä liikkuvia?)
        //taulukko ja blokin transform vastaa toisiaan kun taulukon ruutu on 1 unity-yksikkö * 1 unity-yksikkö
	}

    public void PopBlocks(GameObject popped) {
        print(popped);
        //pop (destroy, animation??) the adjacent blocks that are the same color as popped
    }
}
