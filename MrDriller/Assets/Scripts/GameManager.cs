using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI sugarText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;
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


    private void Start() {
        highScore = PlayerPrefs.GetFloat("highScore", 0);
        depthText = depthText.GetComponent<TextMeshProUGUI>();
        scoreText = scoreText.GetComponent<TextMeshProUGUI>();
        sugarText = sugarText.GetComponent<TextMeshProUGUI>();
        levelText = levelText.GetComponent<TextMeshProUGUI>();
        livesText = livesText.GetComponent<TextMeshProUGUI>();

        AtGameStart();
        depthText.text = ("Depth: " + depth + "µm");
        scoreText.text = ("Score: " + score);
        sugarText.text = ("Sugar level: " + lifeLeft);
        levelText.text = ("Level: " + level);
        livesText.text = ("Lives left: " + livesLeft);
        //different levels?
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
            lifeLeft = 100;
        } else {
            GameOver();
        }
    }

    void GameOver() {
        scoreText.text = ("Game");
        sugarText.text = ("Over");
        levelText.text = ("Mutha");
        livesText.text = ("Fukka");
        PlayerPrefs.SetFloat("highScore", highScore);
        Time.timeScale = 0;
    }

    //create level & level end blocks
    //the level end blocks are below the last row of blocks
}