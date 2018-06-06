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
    public TextMeshProUGUI statusText;
    float statusTextTimer = 0;
    int lifeLeft = 100;
    int lifeLeftAtLvlEnd = 100;
    int livesLeft = 3;
    float lifeDeductionTick = 0.9f;
    float lifeDeductionCounter = 0;
    BlockSpriteChanger[] bscArray;
    BlockManager bm;
    int level = 0;
    int depth = 0;
    float score;
    float highScore;
    public string BGMaudioEvent;
    public string exhaustedAudioEvent;
    public string sugarAudioEvent;
    public string depletedAudioEvent;
    public PlayerCharacter player;
    float lvlEndTimer = 5f;
    bool levelEndReached;
    Camera cam;
    Vector3 camStartPos;
    public string lvlClearedAudioEvent;
    bool played;

    private void Start() {
        highScore = PlayerPrefs.GetFloat("highScore", 0);
        depthText = depthText.GetComponent<TextMeshProUGUI>();
        scoreText = scoreText.GetComponent<TextMeshProUGUI>();
        sugarText = sugarText.GetComponent<TextMeshProUGUI>();
        levelText = levelText.GetComponent<TextMeshProUGUI>();
        livesText = livesText.GetComponent<TextMeshProUGUI>();
        statusText = statusText.GetComponent<TextMeshProUGUI>();

        AtGameStart();
        depthText.text = ("DEPTH: " + depth + "µm");
        scoreText.text = ("SCORE: " + score);
        sugarText.text = ("SUGAR: " + lifeLeft);
        levelText.text = ("LEVEL: " + level);
        livesText.text = ("LIVES: " + livesLeft);
        //different levels?

        Fabric.EventManager.Instance.PostEvent(BGMaudioEvent);
        player = FindObjectOfType<PlayerCharacter>();
        cam = FindObjectOfType<Camera>();
        camStartPos = cam.transform.position;
    }

    public void AtGameStart() {
        // Load level, generate blocks, drop player in scene
        bm = FindObjectOfType<BlockManager>();
        NewLevel();
    }

    public void LevelEnd() {
        levelEndReached = true;
        lifeLeftAtLvlEnd = lifeLeft;
    }

    void NewLevel() {
        lifeLeft = lifeLeftAtLvlEnd;
        level++;
        print("starting new level!");
        bm.AtLevelStart(level);
        bscArray = FindObjectsOfType<BlockSpriteChanger>();
        foreach (BlockSpriteChanger bsc in bscArray) {
            bsc.AtLevelStart();
        }
        played = false;
    }

    private void Update() {
        if (statusTextTimer < 0) {
            statusText.text = "";
        }

        if (levelEndReached && lvlEndTimer >= 0) {
            lvlEndTimer -= Time.deltaTime;
        }

        if (lvlEndTimer < 4) {
            bm.PopAll();
            if (!played) {
                Fabric.EventManager.Instance.PostEvent(lvlClearedAudioEvent);
                played = true;
            }
            statusText.text = "LEVEL CLEARED!";
            statusTextTimer = 3;
        }

        if (lvlEndTimer < 0) {
            levelEndReached = false;
            player.StartNewLvl();
            cam.transform.position = camStartPos + new Vector3(0, 5, 0);
            lvlEndTimer = 5f;
            NewLevel();
        }

    }

    private void FixedUpdate() {

        if (lifeLeft > 100) {
            lifeLeft = 100;
        }

        lifeDeductionCounter += Time.deltaTime;
        while (lifeDeductionCounter > lifeDeductionTick) {
            lifeLeft -= 1;
            lifeDeductionCounter -= lifeDeductionTick;

            sugarText.text = ("SUGAR: " + lifeLeft);
            bm.PopFarAwayBlocks();

        }

        if (lifeLeft <= 0) { // Life amount deducter
            DeadOnArrival();
            player.ColdAndLonelyDeath(false);
            Fabric.EventManager.Instance.PostEvent(exhaustedAudioEvent);
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
        statusText.text = "Sugar -20%";
        statusTextTimer = 3;
        Fabric.EventManager.Instance.PostEvent(depletedAudioEvent);
    }

    public void CandyGet() {
        lifeLeft = lifeLeft + 20;
        statusText.text = "Candy GET! Sugar +20%";
        statusTextTimer = 3;
        Fabric.EventManager.Instance.PostEvent(sugarAudioEvent);
    }

    public void AddScore(int addS) {
        score += addS;
        scoreText.text = ("SCORE:" + score);
    }

    public void Depth(int d) {
        depth = d;
        depthText.text = ("DEPTH: " + depth + "µm");
    }
    
    public void DeadOnArrival() {
        if (!levelEndReached && livesLeft > 1) {
            livesLeft--;
            livesText.text = ("LIVES: " + livesLeft);
            statusText.text = "A Life is lost";
            statusTextTimer = 3;
            lifeLeft = 100;
        } else if (!levelEndReached) {
            GameOver();
        } else {
            print("jotain tapahtui");
        }
    }

    void GameOver() {
        statusText.text = ("Game Over!");
        statusTextTimer = 2f;
        //sugarText.text = ("Over");
        //levelText.text = ("Mother");
        //livesText.text = ("Hugger");
        PlayerPrefs.SetFloat("highScore", highScore);
        Time.timeScale = 0;
    }


}