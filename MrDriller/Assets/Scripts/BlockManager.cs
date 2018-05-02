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
                FindGroups();
            }
        }
    }

    void FindGroups () {
        List<List<BlockScript>> AllGroups = new List<List<BlockScript>>();
        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                if (x != 0 && y != 0) {
                    if (blockGrid[x, y].bc == blockGrid[x - 1, y].bc && blockGrid[x, y].bc == blockGrid[x, y - 1].bc) {
                        //jokuryhmä.Contains(ylempi) ja jokuryhmä.Contains(vasempi)
                        //if (ne on jo samaa ryhmää) { lisää vaan siihen ryhmään }
                        //else { AllGroups[jokuindeksi nimittäin juuri se ylempi].Add(blockGrid[x,y])
                        //lisätään vasemmalta 
                        //tuhotaan }

                    }
                }
                //onko vasemmalla
                else if (x != 0) {
                    //tän gridpos on blockGrid[x,y]
                    //edellisen on blockGrid[x-1,y]
                    if (blockGrid[x, y].bc == blockGrid[x - 1, y].bc) {
                        //samanvärisiä joten laitetaan samaan ryhmään

                    }
                    else {
                        //tee uusi ryhmä
                        AllGroups.Add(new List<BlockScript>());
                    }
                }
                //onko ylhäällä
                else if (y != 0) {
                    if (blockGrid[x, y].bc == blockGrid[x, y - 1].bc) {

                    }
                    else {

                    }
                }

                else {
                    List<BlockScript> tempList = new List<BlockScript> { blockGrid[x, y] };
                    AllGroups.Add(tempList);

                    int tempInt = AllGroups.FindIndex(l => l == tempList);
                    blockGrid[x, y].SetGroupNumber(tempInt);
                    //kerro blockscriptille missä ryhmässä se on
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
