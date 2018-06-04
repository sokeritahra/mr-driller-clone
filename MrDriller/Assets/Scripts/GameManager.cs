﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI sugarText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI statusText;
    float statusTextTimer = 0;
    int lifeLeft = 100;
    int livesLeft = 3;
    float lifeDeductionTick = 0.9f;
    float lifeDeductionCounter = 0;
    BlockSpriteChanger[] bscArray;
    BlockManager bm;
    int level = 1;
    int depth = 0;
    float score;
    float highScore;
    public string BGMaudioEvent;

    private void Start() {
        highScore = PlayerPrefs.GetFloat("highScore", 0);
        depthText = depthText.GetComponent<TextMeshProUGUI>();
        scoreText = scoreText.GetComponent<TextMeshProUGUI>();
        sugarText = sugarText.GetComponent<TextMeshProUGUI>();
        levelText = levelText.GetComponent<TextMeshProUGUI>();
        livesText = livesText.GetComponent<TextMeshProUGUI>();
        statusText = statusText.GetComponent<TextMeshProUGUI>();

        AtGameStart();
        depthText.text = ("Depth: " + depth + "µm");
        scoreText.text = ("Score: " + score);
        sugarText.text = ("Sugar level: " + lifeLeft);
        levelText.text = ("Level: " + level);
        livesText.text = ("Lives left: " + livesLeft);
        //different levels?

        Fabric.EventManager.Instance.PostEvent(BGMaudioEvent);
    }

    void AtGameStart() {
        // Load level, generate blocks, drop player in scene
        bm = FindObjectOfType<BlockManager>();
        bm.AtLevelStart();
        bscArray = FindObjectsOfType<BlockSpriteChanger>();
        foreach (BlockSpriteChanger bsc in bscArray) {
            bsc.AtLevelStart();
        }
    }

    private void Update() {
        if (statusTextTimer < 0) {
            statusText.text = "";
        }
    }

    private void FixedUpdate() {
        lifeDeductionCounter += Time.deltaTime;
        while (lifeDeductionCounter > lifeDeductionTick) {
            lifeLeft -= 1;
            lifeDeductionCounter -= lifeDeductionTick;
            sugarText.text = ("Sugar level: " + lifeLeft);
        }

        if (lifeLeft <= 0) { // Life amount deducter
            DeadOnArrival();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if(statusTextTimer > 0) {
            statusTextTimer -= Time.deltaTime;
        }
    }

    public void PauseGame() {
        Time.timeScale = 1 - Time.timeScale;
        statusText.text = (Time.timeScale == 1) ? "" : "Paused";
        statusTextTimer = 0.1f;
    }

    public void SugarDepletion() {
        lifeLeft = lifeLeft - 20;
        statusText.text = "Kohta on mehut vex!";
        statusTextTimer = 5;
    }

    public void AddScore() {
        score += 100;
        scoreText.text = ("Score: " + score);
    }

    public void Depth(int d) {
        depth = d;
        depthText.text = ("Depth: " + depth + "µm");
    }
    
    public void DeadOnArrival() {
        if (livesLeft > 1) {
            livesLeft--;
            livesText.text = ("Lives left: " + livesLeft);
            statusText.text = "Yksi henki läx!";
            statusTextTimer = 5;
            lifeLeft = 100;
        } else {
            GameOver();
        }
    }

    void GameOver() {
        scoreText.text = ("Game");
        sugarText.text = ("Over");
        levelText.text = ("Mother");
        livesText.text = ("Hugger");
        PlayerPrefs.SetFloat("highScore", highScore);
        Time.timeScale = 0;
    }

    //create level & level end blocks
    //the level end blocks are below the last row of blocks
}