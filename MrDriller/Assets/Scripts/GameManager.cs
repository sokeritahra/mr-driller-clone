﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    int lifeLeft = 100;
    int livesLeft = 3;
    float lifeDeductionTick = 0.9f;
    float lifeDeductionCounter = 0;

    void Update() {
        lifeDeductionCounter += Time.deltaTime;
        while (lifeDeductionCounter > lifeDeductionTick) {
            lifeLeft -= 1;
            lifeDeductionCounter -= lifeDeductionTick;
            print(lifeLeft + "% To Death");
        }

        if (lifeLeft <= 0) {
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
    }

}
