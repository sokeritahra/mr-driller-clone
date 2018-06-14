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
    public TextMeshProUGUI highScoreText;
    float statusTextTimer = 0;
    int lifeLeft = 100;
    int lifeLeftAtLvlEnd = 100;
    int livesLeft = 3;
    float lifeDeductionTick = 0.9f;
    float lifeDeductionCounter = 0;
    BlockSpriteChanger[] bscArray;
    public BlockManager bm;
    int level = 1;
    int depth = 0;
    float score = 0;
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
    Transform endBlokit;
    Transform blokit;
    public bool gameEnded;
    bool gameOn;
    public GameObject canGame;
    public GameObject canMenu;
    public GameObject canCred;
    public float endTimer;

    private void Start() {
        highScore = PlayerPrefs.GetFloat("highScore", 0);
        depthText = depthText.GetComponent<TextMeshProUGUI>();
        scoreText = scoreText.GetComponent<TextMeshProUGUI>();
        highScoreText = highScoreText.GetComponent<TextMeshProUGUI>();
        sugarText = sugarText.GetComponent<TextMeshProUGUI>();
        levelText = levelText.GetComponent<TextMeshProUGUI>();
        livesText = livesText.GetComponent<TextMeshProUGUI>();
        statusText = statusText.GetComponent<TextMeshProUGUI>();

        endBlokit = bm.endBlocks;
        blokit = bm.blockFolder;
        depthText.text = ("DEPTH: " + depth + "um");
        scoreText.text = ("SCORE: " + score);
        highScoreText.text = ("HISCORE: " + highScore);
        sugarText.text = ("SUGAR: " + lifeLeft);
        levelText.text = ("LEVEL: " + level);
        livesText.text = ("LIVES: " + livesLeft);
        //different levels?

        Fabric.EventManager.Instance.PostEvent(BGMaudioEvent);
        cam = FindObjectOfType<Camera>();
        camStartPos = cam.transform.position;
    }

    //TODO KORJAA UUSI PELI
    public void AtGameStart() {
        // Load level, generate blocks, drop player in scene
        canCred.SetActive(false);
        gameOn = true;
        print(bm);
        bm.gameObject.SetActive(true);
        blokit.gameObject.SetActive(true);
        endBlokit.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        level = 1;
        gameEnded = false;
        NewLevel();
        player.transform.position = player.startPos;
        cam.transform.position = camStartPos;
        levelEndReached = false;
        player.alive = true;
        player.pm = PlayerMode.Down;
        livesLeft = 3;
        endTimer = 1f;
        score = 0;
        lifeLeft = 100;
        depthText.text = ("DEPTH: " + depth + "um");
        scoreText.text = ("SCORE: " + score);
        highScoreText.text = ("HISCORE: " + highScore);
        sugarText.text = ("SUGAR: " + lifeLeft);
        levelText.text = ("LEVEL: " + level);
        livesText.text = ("LIVES: " + livesLeft);
    }

    public void LevelEnd() {
        level++;
        levelEndReached = true;
    }

    void NewLevel() {
        statusText.text = "Level " + level + " START!";
        levelText.text = ("LEVEL: " + level);
        player.StartNewLvl();
        cam.transform.position = camStartPos + new Vector3(0, 5, 0);
        bm.AtLevelStart(level);
        bscArray = FindObjectsOfType<BlockSpriteChanger>();
        foreach (BlockSpriteChanger bsc in bscArray) {
            bsc.AtLevelStart();
        }
        played = false;
    }

    private void Update() {
        if (gameOn) {

            if (statusTextTimer < 0) {
                statusText.text = "";
            }

            if (level > 3 && !gameEnded) {
                GameEnd(true);
            } else if (level < 4 && levelEndReached && lvlEndTimer >= 0) {
                lvlEndTimer -= Time.deltaTime;
            }

            if (lvlEndTimer < 3) {
                bm.PopAll();
                if (!played) {
                    Fabric.EventManager.Instance.PostEvent(lvlClearedAudioEvent);
                    played = true;
                }
                statusText.text = "LEVEL CLEARED!";
                statusTextTimer = 2;
            }

            if (lvlEndTimer < 0) {
                levelEndReached = false;
                
                lvlEndTimer = 4f;
                NewLevel();
            }

        }
        else {
                endTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        if (gameOn) {

            if(lifeLeft < 20) {
                statusText.text = "Running out of\nsugar!";
                statusTextTimer = 0.1f;
            }

            if (lifeLeft > 100) {
                lifeLeft = 100;
            }

           if(!levelEndReached) {
                lifeDeductionCounter += Time.deltaTime;
                while (lifeDeductionCounter > lifeDeductionTick) {
                    lifeLeft -= 1;
                    lifeDeductionCounter -= lifeDeductionTick;

                    sugarText.text = ("SUGAR: " + lifeLeft + "%");
                    bm.PopFarAwayBlocks();
                }
            }

            if (lifeLeft <= 0) { // Life amount deducter
                DeadOnArrival();
                player.ColdAndLonelyDeath(false);
                Fabric.EventManager.Instance.PostEvent(exhaustedAudioEvent);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                ReturnToMenu();
            }

            if (statusTextTimer > 0) {
                statusTextTimer -= Time.deltaTime;
            }

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
        statusText.text = "Sugar +20%";
        statusTextTimer = 3;
        Fabric.EventManager.Instance.PostEvent(sugarAudioEvent);
    }

    public void AddScore(int addS) {
        score += addS;
        scoreText.text = ("SCORE: " + score);
    }

    public void Depth(int d) {
            depth = d + bm.rows * (level-1);
        depthText.text = ("DEPTH: " + depth + "um");
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


        GameEnd(false);
    }

    public void ReturnToMenu() {

        gameEnded = true;
        bm.PopAll();
        blokit.gameObject.SetActive(false);
        endBlokit.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        bm.gameObject.SetActive(false);
        canGame.SetActive(false);
        canMenu.SetActive(true);
        player.transform.position = player.startPos;
        cam.transform.position = camStartPos;
    }

    void GameEnd(bool win) {
        Fabric.EventManager.Instance.PostEvent(lvlClearedAudioEvent);
        bm.PopAll();
        played = true;
        if (win) {
            statusText.text = "YOU WIN!!! \n \n \n \n Press any key\nto quit";
        }
        else {
            statusText.text = "GAME OVER!!! \n \n \n \n Press any key\nto quit";
        }
        statusTextTimer = 5f;
        gameEnded = true;
        gameOn = false;
        if (score > highScore) {
            highScore = score;
            PlayerPrefs.SetFloat("highScore", highScore);
            highScoreText.text = ("HISCORE: " + highScore);
        }
        //pysäytä laskureita, ReturnToMenu();
    }
}