using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    //taulukko
    public int[,] blockGrid;
    public int rows;
    public int columns;
    //blokki prefabi

    void Start () {
        //luodaan taulukko ja generoidaan blokit sinne
        blockGrid = new int[rows, columns];

        for (int x = 0; x < rows; x++) {
            for (int y = 0; y < columns; y++) {
                //blockGrid[x, y] = some block;
                //aseta blockit paikoilleen scenessä 
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
