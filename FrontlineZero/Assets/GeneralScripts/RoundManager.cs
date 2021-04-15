using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public Text killCountText;
    public Text timerText;

    int killCount;

    public float timer;
    public bool timeStarted = false;

    public int maxDifficulty;
    public int difficulty = 0;
    public float difficultyIncreaseTime;

    public Animator MechAnimator;
    public Animator StartShipMovementAnimator;

    public GameStage gameStage = GameStage.MainMenu;

    public Text lastScoreText;
    public Text bestScoreText;

    public bool isInGameScene;

    public enum GameStage
    {
        MainMenu, Starting, InGame
    }

    // Singleton
    private static RoundManager _instance;
    public static RoundManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if (isInGameScene)
        {
            InitDisplays();
        }
    }

    // set text of the displays in the main menu, with last- and highscore
    void InitDisplays()
    {
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey("LastTime"))
        {
            string textToDisplay = "Operation Duration: "+FormatTime(PlayerPrefs.GetFloat("LastTime")) + System.Environment.NewLine + System.Environment.NewLine;
            textToDisplay += "Confirmed Eliminations: " + PlayerPrefs.GetInt("LastKillCount");
            lastScoreText.text = textToDisplay;

            if (PlayerPrefs.HasKey("BestTime"))
            {
                string bestTextToDisplay = "Operation Duration: " + FormatTime(PlayerPrefs.GetFloat("BestTime")) + System.Environment.NewLine + System.Environment.NewLine;
                bestTextToDisplay += "Confirmed Eliminations: " + PlayerPrefs.GetInt("BestKillCount");
                bestScoreText.text = bestTextToDisplay;
            }
        }
        else
        {
            lastScoreText.text = "This is your first Operation.";
            bestScoreText.text = "This is your first Operation.";
        }        
    }

    void Update()
    {
        if (isInGameScene)
        {
            StageManaging();
        }
    }

    void StageManaging()
    {
        switch (gameStage)
        {
            case GameStage.MainMenu:
                MainMenuUpdate();
                break;
            case GameStage.Starting:
                StartingUpdate();
                break;
            case GameStage.InGame:
                InGameUpdate();
                break;
        }
    }


    void MainMenuUpdate()
    {

    }
    void StartingUpdate()
    {
        if(MechAnimator.transform.position.y < 70)
        {
            StartRound();
        }
    }
    void InGameUpdate()
    {
        Timer();
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string timeFormated = string.Format("{0:00}:{1:00}", minutes, seconds);
        return timeFormated;
    }

    void Timer()
    {
        if (timeStarted == true)
        {
            timer += Time.deltaTime;
        }        

        if(timerText != null)
        {
            timerText.text = FormatTime(timer);
        }
    }

    public void AddKill()
    {
        killCount++;
        if(killCountText != null)
        {
            killCountText.text = "" + killCount;
        }
    }

    // increases game difficulty every few seconds
    IEnumerator IncreaseDifficulty()
    {
        while (difficulty < maxDifficulty)
        {            
            yield return new WaitForSeconds(difficultyIncreaseTime);
            difficulty++;
            Debug.Log("New Difficulty: " + difficulty + " DifficultyMultiplier: " + GetDifficultyMultiplier());
            
        }
    }

    public float GetDifficultyMultiplier()
    {
        return Mathf.Clamp01((float)difficulty / (float)maxDifficulty); 
    }

    public void EndRoundIn(float time)
    {
        ScoreSubmission();
        Invoke("EndRound", time);
    }

    // submit final score of the round, and compares with highscore
    void ScoreSubmission()
    {
        float finalTime = timer;
        int finalKillCount = killCount;

        PlayerPrefs.SetFloat("LastTime", finalTime);
        PlayerPrefs.SetInt("LastKillCount", finalKillCount);

        if (PlayerPrefs.HasKey("BestTime") && PlayerPrefs.HasKey("BestKillCount"))
        {
            if(finalKillCount >= PlayerPrefs.GetInt("BestKillCount"))
            {
                PlayerPrefs.SetFloat("BestTime", finalTime);
                PlayerPrefs.SetInt("BestKillCount", finalKillCount);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("BestTime", finalTime);
            PlayerPrefs.SetInt("BestKillCount", finalKillCount);
        }
    }



    void EndRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        gameStage = GameStage.Starting;
        MechAnimator.SetBool("isUp", true);
        StartShipMovementAnimator.SetBool("GameStarted", true);
    }

    public void StartRound()
    {
        StartShipMovementAnimator.SetBool("RoundStarted", true);
        StartCoroutine(IncreaseDifficulty());
        SpawnManager.Instance.Activate();
        SupplyManager.Instance.Activate();
        gameStage = GameStage.InGame;
        timeStarted = true;
        MusicManager.Instance.isInGame = true;
    }

    [ContextMenu("DeletePlayerPrefs")]
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
