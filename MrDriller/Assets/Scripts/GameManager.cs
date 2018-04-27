using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    int lifeLeft = 100;
    int livesLeft = 3;
    float lifeDeductionTick = 0.9f;
    float lifeDeductionCounter = 0;
    BlockManager bm;
    float score;
    float highScore;

    private void Start() {
        bm = FindObjectOfType<BlockManager>();
        bm.AtLevelStart();
        highScore = PlayerPrefs.GetFloat("highScore", 0);
        AtGameStart();
        //different levels?
    }



    void AtGameStart() {
        // Load level, generate blocks, drop player in scene
    }

    void Update() {
        lifeDeductionCounter += Time.deltaTime;
        while (lifeDeductionCounter > lifeDeductionTick) {
            lifeLeft -= 1;
            lifeDeductionCounter -= lifeDeductionTick;
            // print(lifeLeft + "% To Death");
        }

        if (lifeLeft <= 0) { // Life amount deducter
            if (livesLeft > 0) {
                livesLeft--;
                lifeLeft = 100;
            } else {
                GameOver();
            }
        }
    }

    void GameOver() {
        print("GameOver!");
        PlayerPrefs.SetFloat("highScore", highScore);
    }

    //create level & level end blocks
    //the level end blocks are below the last row of blocks
}
