using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    //taulukko
    public int[,] blockGrid;
    public int rows;
    public int columns;

    void Start () {
        //luodaan taulukko ja generoidaan blokit sinne
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 5; y++) {
                blockGrid[x, y] = VALUE_HERE;
            }
        }
    }
	
	void Update () {
		//tsekataan mitkä blokit on yhdessä
        //(mitkä on staattisia ja mitkä liikkuvia?)
        //taulukko ja blokin transform vastaa toisiaan kun taulukon ruutu on 1 unity-yksikkö * 1 unity-yksikkö
	}
}
