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

    public void AtLevelStart () {
        //luodaan taulukko ja generoidaan blokit sinne
        blockGrid = new BlockScript[columns, rows];

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                Vector3 pos = new Vector3(x, -y, 0);
                //TODO: pos so that blocks are in the right place in the scene
                //also where is the first row?
                GameObject tempObj = Instantiate(blockPrefab, pos, Quaternion.identity);

                blockGrid[x,y] = tempObj.GetComponent<BlockScript>();
                blockGrid[x, y].SetGridPos(x, y);
                
            }
        }
        FindGroups();
    }

    void FindGroups() {
        List<List<BlockScript>> AllGroups = new List<List<BlockScript>>();
        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {

                var thisSquare = blockGrid[x, y];

                if (x != 0 && y != 0) {

                    var leftSquare = blockGrid[x - 1, y];
                    var topSquare = blockGrid[x, y - 1];

                    if (thisSquare.bc == leftSquare.bc && thisSquare.bc == topSquare.bc) {
                        //jos on samanvärinen sekä vasemmalla että ylempänä lisätään ylempään
                        AllGroups[topSquare.groupNumber].Add(thisSquare);
                        if (leftSquare.groupNumber != topSquare.groupNumber) {
                            AllGroups[topSquare.groupNumber].Add(leftSquare);
                            AllGroups.RemoveAt(leftSquare.groupNumber);
                            //jos vasemmalla oleva blokki ei vielä samassa ryhmässä, lisätään se samaan ryhmään
                            //TODO: vaihda blokin ryhmänumero: thisSquare.SetGroupNumber(
                            //ja tuhotaan sen ryhmä
                            print("this block " + thisSquare.gridPos + " and the one(s) on the left added to the one on top");
                        }
                        else {
                            //jos vasemmalla ja ylhäällä olevat blokit on samassa ryhmässä, mitään ei tuhota 
                            //TODO: vaihda blokin ryhmänumero: thisSquare.SetGroupNumber(
                            print("added to the same group, should pop now!");
                        }


                    }
                }
                //onko vasemmalla
                else if (x != 0) {

                    var leftSquare = blockGrid[x - 1, y];

                    if (thisSquare.bc == leftSquare.bc) {
                        //samanvärisiä joten laitetaan samaan ryhmään
                        AllGroups[leftSquare.groupNumber].Add(thisSquare);

                    }
                    else {
                        //tee uusi ryhmä
                        List<BlockScript> tempList = new List<BlockScript> { thisSquare };
                        AllGroups.Add(tempList);
                        int tempInt = AllGroups.FindIndex(l => l == tempList);
                        thisSquare.SetGroupNumber(tempInt);
                    }
                }
                //onko ylhäällä
                else if (y != 0) {

                    var topSquare = blockGrid[x, y - 1];
                    print(topSquare);
                    if (thisSquare.bc == topSquare.bc) {
                        AllGroups[topSquare.groupNumber].Add(thisSquare);
                    }
                    else {
                        List<BlockScript> tempList = new List<BlockScript> { thisSquare };
                        AllGroups.Add(tempList);
                        int tempInt = AllGroups.FindIndex(l => l == tempList);
                        thisSquare.SetGroupNumber(tempInt);
                    }
                }

                else {
                    List<BlockScript> tempList = new List<BlockScript> { thisSquare };
                    AllGroups.Add(tempList);
                    //kerro blockscriptille missä ryhmässä se on
                    int tempInt = AllGroups.FindIndex(l => l == tempList);
                    thisSquare.SetGroupNumber(tempInt);
                }
                }
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
