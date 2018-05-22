﻿using System.Collections;
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
    public BlockScript[] blockArray;
    float posx = 0;
    float posy = 0;
    public Color[] colorList;

    public void AtLevelStart() {
        //luodaan taulukko ja generoidaan blokit sinne
        blockGrid = new BlockScript[columns, rows];

        FindGroups();
        foreach (BlockScript block in blockArray) {
            block.AtLevelStart();
        }
        }

    public void SetBlockInGrid (BlockScript block) {
        posx = block.transform.position.x;
        posy = block.transform.position.y;
        blockGrid[(int)posx, -(int)posy] = block;
        blockGrid[(int)posx, -(int)posy].SetGridPos((int)posx, -(int)posy, columns);
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
            print(thisSquare.gridPos + " on ryhmässä " + AllGroups.IndexOf(thisSquare.group));
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

    void Update() {
        //if (blockGrid[0, 0] != null) {
        //    testBlock = blockGrid[0, 0].GetComponent<Block>();
        //    testBlock.Pop();
        //}

        //tsekataan mitkä blokit on yhdessä
        //(mitkä on staattisia ja mitkä liikkuvia?)
        //taulukko ja blokin transform vastaa toisiaan kun taulukon ruutu on 1 unity-yksikkö * 1 unity-yksikkö
    }

    public void PopBlocks(BlockScript popped) {
        print(popped);
        foreach (BlockScript block in popped.group) {
            //print("hei minun nimi on " + block);

                block.Pop();

        }
        //}
        //toimiikohan tää??
        //pop (destroy, animation??) the adjacent blocks that are the same color as popped
    }

    public void HoldBlocks(BlockScript toFall) {
        foreach (BlockScript block in toFall.group) {
            block.bs = BlockState.Hold;
        }
    }

    public void MergeGroups(BlockScript first, BlockScript second) {
        foreach (BlockScript block in first.group) {
            block.SetGroup(second.group);
            second.group.Add(block);
        }
    }

}
