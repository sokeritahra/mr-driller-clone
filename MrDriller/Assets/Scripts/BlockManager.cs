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
	public GameObject candyPrefab;
    public GameObject greyPrefab;
    public PlayerCharacter player;
    public Vector2 firstBlock = new Vector2(0,0);
    //blokki prefabi
    BlockScript blockScript;
    public Transform blockFolder;
    public Transform endBlocks;
    //Block testBlock;
    List<List<BlockScript>> AllGroups;
    public BlockScript[] blockArray;
    float posx = 0;
    float posy = 0;
    public Color[] colorList;
    public int lvlEndBlocks;
    public GameManager gm;
    public string poppedAudioEvent;

    public void AtLevelStart(int level) {
        //luodaan taulukko ja generoidaan blokit sinne
        GenerateBlocks(level);
        blockGrid = new BlockScript[columns, rows];
        FindGroups();
        foreach (BlockScript block in blockArray) {
            block.AtLevelStart();
        }
        player = FindObjectOfType<PlayerCharacter>();
    }

    //if the block above is durable -> chance of generating a candy block?
    //should the durable & candy blocks be generated?
    //if there is a durable block here, delete the generated block from here


    public void CreateBlock(int type, int row, int column) {
        //candy = type 1, block = type 2, durable block = type 3, level end block = type 4
        string typeS;
        if (type == 1) {
            typeS = "Candy";
        } else if (type == 2) {
            typeS = "Block";
        } else if (type == 3) {
            typeS = "Fluori";
        }
        else {
            typeS = "LvlEnd";
        }
        GameObject go;

        //configure candies
        if (type == 1) {
			go = Instantiate (candyPrefab);
            blockScript = go.GetComponent<BlockScript>();
            blockScript.bc = BlockColor.Candy;
			blockScript.bs = BlockState.Static;
		} else if (type == 3) {
            go = Instantiate (greyPrefab);
        }
        else {
			go = Instantiate (blockPrefab);
		} 
        //for all types, change name and position
		go.name = (typeS + "_" + row + "_" + column).ToString();
		Vector2 newPosition = new Vector2(firstBlock.x + column, firstBlock.y - row);
        go.transform.position = newPosition;
        blockScript = go.GetComponent<BlockScript>();

        //separate level end blocks from blocks and candies (why?)
        //configure level end blocks
        if (type < 4)
        {
            go.transform.parent = blockFolder;
        }
        else {
            go.transform.parent = endBlocks;
            blockScript.bc = BlockColor.LevelEnd;
            blockScript.bs = BlockState.Static;
            blockScript.levelEnd = true;
  		}
	}
		
    public void GenerateBlocks(int level) {

        if (level == 1) {
            //TODO: TUTORIAL ALKU!
            int levelStartRows = 7;
            //int[, ,] levelStartBlocks = new int[7, columns, 4] { { { 1, 2, 3 }, { 4, 5, 6 } },
            //                     { { 7, 8, 9 }, { 10, 11, 12 } } };
            //Dictionary<Vector2, int> dict = new Dictionary<string, int>
            //{
            //    { "one", 1 },
            //    { "two", 2 },
            //    { "three", 3 },
            //    { "four", 4 }
            //};
            //for (int i = 0; i < levelStartRows; i++) {
            //    for (int j = 0; j < columns; j++) {
            //        if(i == 0 && j == 4) {

            //        }
            //        CreateBlock(3, i, j);
            //    }
            //}
            for (int i = levelStartRows; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    if (i < rows - lvlEndBlocks) {
                        var candyRandomizer = Random.value;
                        if (candyRandomizer < .02) {
                            CreateBlock(1, i, j);
                            //tee cndy ja else tee blokki
                        }
                        else if (candyRandomizer < .15) {
                            CreateBlock(3, i, j);
                            blockScript.bc = BlockColor.Grey;
                            }
                        else { 
                            CreateBlock(2, i, j);
                            //assign color to blocks
                            var blockColorRandomizer = Random.value;
                            if (blockColorRandomizer > .775f)
                            {
                                blockScript.bc = BlockColor.Blue;
                            }
                            else if (blockColorRandomizer > .55f)
                            {
                                blockScript.bc = BlockColor.Green;
                            }
                            else if (blockColorRandomizer > .325f)
                            {
                                blockScript.bc = BlockColor.Red;
                            }
                            else {
                                blockScript.bc = BlockColor.Yellow;
                            }

                            }
                    } else {
                        CreateBlock(4, i, j);
                    }
                }
            }
        }

        else if (level == 2) {
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    if (i < rows - lvlEndBlocks) {
                        var candyRandomizer = Random.value;
                        if (candyRandomizer < .01) {
                            CreateBlock(1, i, j);
                            //tee cndy ja else tee blokki
                        }
                        else if (candyRandomizer < .1) {
                            CreateBlock(3, i, j);
                            blockScript.bc = BlockColor.Grey;
                            }
                        else { 
                            CreateBlock(2, i, j);
                            //assign color to blocks
                            var blockColorRandomizer = Random.value;
                            if (blockColorRandomizer > .5f) {
                                blockScript.bc = BlockColor.Blue;
                            }
                            else {
                                blockScript.bc = BlockColor.Green;
                            }


                            }
                    } else {
                        CreateBlock(4, i, j);
                    }
                }
            }
        }

        else if (level == 3) {
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    if (i < rows - lvlEndBlocks) {
                        var candyRandomizer = Random.value;
                        if (candyRandomizer < .015) {
                            CreateBlock(1, i, j);
                            //tee cndy ja else tee blokki
                        }
                        else if (candyRandomizer < .2) {
                            CreateBlock(3, i, j);
                            blockScript.bc = BlockColor.Grey;
                        }
                        else {
                            CreateBlock(2, i, j);
                            //assign color to blocks
                            var blockColorRandomizer = Random.value;
                            if (blockColorRandomizer > .5f) {
                                blockScript.bc = BlockColor.Red;
                            }
                            else if (blockColorRandomizer > .325f) {
                                blockScript.bc = BlockColor.Green;
                            }
                            else if (blockColorRandomizer > .15) {
                                blockScript.bc = BlockColor.Blue;
                            }
                            else {
                                blockScript.bc = BlockColor.Yellow;
                            }

                        }
                    }
                    else {
                        CreateBlock(4, i, j);
                    }
                }
            }
        }

        //else {
        //    for (int i = 0; i < rows; i++) {
        //        for (int j = 0; j < columns; j++) {
        //            if (i < rows - lvlEndBlocks) {
        //                var candyRandomizer = Random.value;
        //                if (candyRandomizer < .001) {
        //                    CreateBlock(1, i, j);
        //                    //tee cndy ja else tee blokki
        //                }
        //                else {
        //                    CreateBlock(2, i, j);
        //                    //assign color to blocks
        //                    var blockColorRandomizer = Random.value;
        //                    if (blockColorRandomizer > .01f) {
        //                        blockScript.bc = BlockColor.Blue;
        //                    }
        //                    else if (blockColorRandomizer > .002f) {
        //                        blockScript.bc = BlockColor.Green;
        //                    }
        //                    //else if (blockColorRandomizer > .4f)
        //                    //{
        //                    //    blockScript.bc = BlockColor.Red;
        //                    //}
        //                    //else if (blockColorRandomizer > .3f)
        //                    //{
        //                    //    blockScript.bc = BlockColor.Yellow;
        //                    //}
        //                    else {
        //                        blockScript.bc = BlockColor.Grey;
        //                    }
        //                }
        //            }
        //            else {
        //                CreateBlock(3, i, j);
        //            }
        //        }
        //    }
        //} 
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

        AllGroups = new List<List<BlockScript>>();
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {

                var thisSquare = blockGrid[x, y];

                if (thisSquare.bc != BlockColor.Candy && x != 0 && y != 0) {

                    var leftSquare = blockGrid[x - 1, y];
                    var topSquare = blockGrid[x, y - 1];

                    //candyt aina yksitellen omassa ryhmässään
                    if (thisSquare.bc == leftSquare.bc && thisSquare.bc == topSquare.bc) {
                        //jos on samanvärinen sekä vasemmalla että ylempänä lisätään ylempään
                        topSquare.group.Add(thisSquare);
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
                        }
                            //jos vasemmalla ja ylhäällä olevat blokit on samassa ryhmässä, mitään ei tuhota
                    }

                    else {
                        //onko vasemmalla
                        CheckOtherSquare(leftSquare, thisSquare);
                        //onko ylhäällä
                        CheckOtherSquare(topSquare, thisSquare);
                    }
                }

                else if (thisSquare.bc != BlockColor.Candy && x != 0) {

                    var leftSquare = blockGrid[x - 1, y];

                    CheckOtherSquare(leftSquare, thisSquare);


                }

                else if (thisSquare.bc != BlockColor.Candy && y != 0) {

                    var topSquare = blockGrid[x, y - 1];

                    CheckOtherSquare(topSquare, thisSquare);

                }

                else {
                    List<BlockScript> tempList = new List<BlockScript> { thisSquare };
                    AllGroups.Add(tempList);
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
           // print(AllGroups.IndexOf(group) + " : " + juttu);
        }

        //print(" täsä kaikki: " + AllGroups);
    }

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

    void FixedUpdate() {
        List<List<BlockScript>> toBeRemoved = new List<List<BlockScript>>();
        List<List<BlockScript>> toBePopped = new List<List<BlockScript>>();
        int anyStatic = 0;

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

            if (falling == g.Count) {

                List<List<BlockScript>> merges = new List<List<BlockScript>>();
                int snaps = 0;
                foreach (BlockScript block in g) {
                    if(block.bc != BlockColor.Candy) {
                        if (block.CheckSameBelow()) {
                            merges.Add(block.blockBelow.group);
                        }
                        else if (block.CheckAnyBelow()) {
                            snaps++;
                            if(block.blockBelow.bs == BlockState.Static) {
                                anyStatic++;
                            }
                        }

                        if (block.CheckLeft()) {
                            merges.Add(block.blockLeft.group);
                            //print("merging from the LEFT " + block.blockLeft + " with " + block);
                            //jos jokin näistä on static, snäpätessä staticiks
                        }

                        if (block.CheckRight()) {
                            merges.Add(block.blockRight.group);
                            //print("merging from the RighT " + block.blockRight + " with " + block);
                            //jos jokin näistä on static, snäpätessä staticiks
                        }
                    }
                    //candyiltä pitää tarkistaa myös snäpätäänkö
                    else if (block.CheckAnyBelow()) {
                        snaps++;
                        if (block.blockBelow.bs == BlockState.Static) {
                            anyStatic++;
                        }
                    }
                    
                }
                if (merges.Count > 0) {

                    foreach (List<BlockScript> mg in merges) {
                        MergeGroups(mg, g);
                        toBeRemoved.Add(mg);
                        //print("merging " + mg + " with " + g);
                        //SHOULD POP IF GROUP.COUNT > 3
                        if (g.Count > 3 && !toBePopped.Contains(g)) {
                            toBePopped.Add(g);
                        }

                    }
                    //HUOM HUOM! TÄMÄ SEURAAVA PÄTEE SIIS VAIN MERGEÄVILLE RYHMILLE
                    //SEN PITÄISI TOIMIA MYÖS ERIVÄRISILLE RYHMILLE!
                    //TODO!!!
                    float holdTime = 2f;
                    foreach (BlockScript block in g) {
                        holdTime = holdTime < block.holdTimer ? holdTime : block.holdTimer;
                    }
                    foreach (BlockScript block in g) {
                        block.holdTimer = holdTime;
                        block.SnapInPlace(BlockState.Hold);
                        //print("snapping and changing to hold " + block + " and holdtime is " + block.holdTimer);
                    }
                    //TODO: snäppäys - voinko tehä snäppifunktion joka toimii sekä sivuille että alas?
                    //voiko toimia että sen laittaa staticiksi - muuttuuko holdiksi at all tai tarpeeks nopee?
                    //tai siis eihän se tietenkään mihinkään muutu vaan se pitää laittaa
                    //mutta niin se laitetaankin tuolla else-kohdassa

                    //MILLON TÄÄ PITÄÄ TEHÄ bm.SetBlockInGrid(this);
                }
                else if (snaps > 0) {

                    if (anyStatic > 0) {
                        foreach (BlockScript block in g) {
                            block.SnapInPlace(BlockState.Static);
                        }
                    }
                    else {
                        float holdTime = 2f;
                        foreach (BlockScript block in g) {
                            if(block.blockBelow) {
                                holdTime = holdTime < block.blockBelow.holdTimer ? holdTime : block.blockBelow.holdTimer;
                            }
                        }

                        foreach (BlockScript block in g) {
                            block.holdTimer = holdTime;
                            block.SnapInPlace(BlockState.Hold);
                            //print("snapping and changing to hold " + block + " and holdtime is " + block.holdTimer);
                        }

                        if (g.Count > 3 && !toBePopped.Contains(g)) {
                            toBePopped.Add(g);
                        }
                    }
                }
                else {
                    DropBlocks(g);

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
 

            }
            //tsekkaa ei-putoavista pitäiskö pudota
            else {
                foreach (BlockScript block in g) {
                    block.SetBelow();
                    block.SetLeft();
                    block.SetRight();
                }

                if (CheckIfGroupOnAir(g)) {
                    //print("Group's states changed to hold");
                    foreach (BlockScript block in g) {
                        block.bs = BlockState.Hold;
                        block.holdTimer = 1.5f;
                        //print("group contains " + block);
                    }
                }

                }

            //UNCOMMENT if falling / merging not working
            //foreach (BlockScript block in g)
            //{
            //    block.SetBelow();
            //    block.SetLeft();
            //    block.SetRight();
            //}
        }

        foreach (List<BlockScript> g in toBeRemoved) {
            AllGroups.Remove(g);
        }

        foreach (List<BlockScript> g in toBePopped) {
            foreach (BlockScript block in g) {
                if (block.bc == BlockColor.Grey) {
                    block.didGreyGetDrilled = false;
                }
            }
            PopBlocks(g, 5, 100);
        }

        //millon tsekataan mikä puhkeaa?

        //taulukko ja blokin transform vastaa toisiaan kun taulukon ruutu on 1 unity-yksikkö * 1 unity-yksikkö
    }

    //huom! tämä ei koskaan tapahtu harmaille blokeille
    public void PopBlocks(List<BlockScript> gToPop, int hits, int score) {
        //print(popped);
        bool lvlEndReached = false;
        //Fabric.EventManager.Instance.PostEvent(poppedAudioEvent);
        foreach (BlockScript block in gToPop) {
            lvlEndReached = block.levelEnd;
                block.Pop(hits, score);
        }
        AllGroups.Remove(gToPop);
        if (lvlEndReached) {
            gm.LevelEnd();
        }
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
            if ((block.CheckAnyBelow() && block.blockBelow.group != block.group && block.blockBelow.bs == BlockState.Static)
                || block.levelEnd || (block.blockBelow && block.blockBelow.levelEnd)) {
                tempInt++; //lisätään yksi, jos alla on blokki joka on eri ryhmässä ja static tai jos alla on tai blokki itse on levelEnd
            }
        }
        return !(tempInt > 0); //jos enemmän kuin yksi palautetaan false
    }

    public void PopAll() {
        List<List<BlockScript>> toBeRemoved = new List<List<BlockScript>>();
        List<List<BlockScript>> toBePopped = new List<List<BlockScript>>();

        foreach (List<BlockScript> g in AllGroups) {
            toBeRemoved.Add(g);
            toBePopped.Add(g);
        }
        foreach (List<BlockScript> g in toBeRemoved) {
            AllGroups.Remove(g);
        }
        foreach (List<BlockScript> g in toBePopped) {
            foreach(BlockScript block in g) {
                Destroy(block.gameObject);
            }
        }

    }

    public void PopFarAwayBlocks() {

        List<List<BlockScript>> toBeRemoved = new List<List<BlockScript>>();
        List<List<BlockScript>> toBePopped = new List<List<BlockScript>>();

        for (int y = Mathf.Abs(Mathf.RoundToInt(player.transform.position.y)) - 15; y > -1; y--) {
            // Destroy blocks without adding to score
            //lisätään toBePopped-listaan kaikki joihin pätee: vai toBeRemoved? kysyy Ari molempiin t. Sara

            foreach (List<BlockScript> group in AllGroups) {
                int onTop = 0;
                foreach (BlockScript block in group) {
                    var roundedPos = new Vector3(Mathf.RoundToInt(block.transform.position.x), Mathf.RoundToInt(block.transform.position.y),
                        Mathf.RoundToInt(block.transform.position.z));

                    if (roundedPos.y == y * -1) {
                        onTop++;
                        print("going to destroy " + block);
                    }
                }
                if (onTop > 0) {
                    toBeRemoved.Add(group);

                    toBePopped.Add(group);
                }
            }

        }

        foreach (List<BlockScript> g in toBeRemoved) {
            AllGroups.Remove(g);
        }

        foreach (List<BlockScript> g in toBePopped) {
            foreach (BlockScript block in g) {
                Destroy(block.gameObject);
            }
        }

    }

    public void PopThreeColumnsOnTop() {
        print("pop three columns");
        print(Mathf.Abs(Mathf.RoundToInt(player.transform.position.y)));

        List<List<BlockScript>> toBeRemoved = new List<List<BlockScript>>();
        List<List<BlockScript>> toBePopped = new List<List<BlockScript>>();
        for (int y = Mathf.Abs(Mathf.RoundToInt(player.transform.position.y)); y > -1; y--) {
            // Destroy blocks without adding to score
            //lisätään toBePopped-listaan kaikki joihin pätee: vai toBeRemoved? kysyy Ari molempiin t. Sara


            foreach(List<BlockScript> group in AllGroups) {
                int onTop = 0;
                foreach (BlockScript block in group) {
                    var roundedPos = new Vector3(Mathf.RoundToInt(block.transform.position.x), Mathf.RoundToInt(block.transform.position.y),
                        Mathf.RoundToInt(block.transform.position.z));

                    if (roundedPos == new Vector3(Mathf.RoundToInt(player.transform.position.x), y * -1, 0) ||
                        roundedPos == new Vector3(Mathf.RoundToInt(player.transform.position.x + 1), y * -1, 0) ||
                        roundedPos == new Vector3(Mathf.RoundToInt(player.transform.position.x - 1), y * -1, 0)) {
                        onTop++;
                        print("going to destroy " + block);
                    }
                }
                if (onTop > 0) {
                    toBeRemoved.Add(group);

                    toBePopped.Add(group);
                }
            }

        }

        foreach (List<BlockScript> g in toBeRemoved) {
            AllGroups.Remove(g);
        }

        foreach (List<BlockScript> g in toBePopped) {
            foreach (BlockScript block in g) {
                if (block.bc == BlockColor.Grey) {
                    block.didGreyGetDrilled = false;
                }
            }
            PopBlocks(g, 6, 0);
        }
    }
}
