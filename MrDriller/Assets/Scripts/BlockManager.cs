using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    //taulukko
    public GameObject[,] blockGrid;
    public int rows;
    public int columns;
    public GameObject block;
    //blokki prefabi

    public void AtLevelStart () {
        //luodaan taulukko ja generoidaan blokit sinne
        blockGrid = new GameObject[rows, columns];

        for (int x = 0; x < rows; x++) {
            for (int y = 0; y < columns; y++) {
                Vector3 pos = new Vector3(x, y, 0);
                //TODO: pos so that blocks are around middle vertical axis
                //also where is the first row?
                blockGrid[x,y] = Instantiate(block, pos, Quaternion.identity);
                //tell the block where it is?
            }
        }
    }
	
	void Update () {
		//tsekataan mitkä blokit on yhdessä
        //(mitkä on staattisia ja mitkä liikkuvia?)
        //taulukko ja blokin transform vastaa toisiaan kun taulukon ruutu on 1 unity-yksikkö * 1 unity-yksikkö
	}
}
