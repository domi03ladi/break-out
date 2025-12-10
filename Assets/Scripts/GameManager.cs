using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState { Playing, GameOver, MainMenu, SelectLevel, CreateLevel}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;



    [Header("Game State Containers")]
    [SerializeField] private GameObject playingContainer;
    [SerializeField] private GameObject livesUI;             
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject mainMenuUi;
    [SerializeField] private GameObject selectLevelUi;
    [SerializeField] private GameObject createLevelUi;

    [Header("Sounds")]
    [SerializeField] private AudioClip btnClickSound;
    [SerializeField] private AudioClip loseLifeSound;


    [SerializeField] private Image[] lifeImages;

    [SerializeField] private Transform paddleTransform;

    [SerializeField] private GameObject ballPrefab;
    public Vector3 startPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI highScoreText;

    private AudioSource audioSource;


    private int currentLives = 3;
    private int score = 0;
        

    void Awake()
    {
        // Singleton-Pattern
        if (Instance == null)   
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();

        // show all lives at the beginning
        UpdateLivesUI();
        SetState(GameState.MainMenu);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoseLife()
    {

        if (audioSource != null && loseLifeSound != null)
        {
            audioSource.PlayOneShot(loseLifeSound);
        }

        if (currentLives > 0)
        {
            currentLives--;
            UpdateLivesUI();

            if (currentLives == 0)
            {
                Debug.Log("Game Over! All lives lost");
                SetState(GameState.GameOver);
            }
            else
            {
                Debug.Log("Start new round!");
                ResetBall();
            }
        }
    }

    private void UpdateLivesUI()
    {
        if (lifeImages == null || lifeImages.Length == 0) return;

        for (int i = 0; i < lifeImages.Length; i++) 
        {
            lifeImages[i].gameObject.SetActive(i < currentLives);
        }
    }

    private void ResetBall()
    {
        if (currentLives > 0)
        {
            // reset paddle
            if (paddleTransform != null)
            {
                paddleTransform.position = new Vector3(0f, paddleTransform.position.y, paddleTransform.position.z);
            }

            // instantiate new ball
            GameObject newBallObject = Instantiate(ballPrefab, startPosition, Quaternion.identity);

            BallControl ballControl = newBallObject.GetComponent<BallControl>();

            if (ballControl != null)
            {
                // Delay on start (1 Second)
                ballControl.LaunchDelayed();
            }
            else
            {
                Debug.LogError("Der neu erstellte Ball hat kein BallControl-Skript! Bitte das Ball-Prefab pr�fen.");
            }
        }
    }

    public void StartGame()
    {
        playClickSound();

        SetState(GameState.Playing);

        score = 0;
        UpdateScoreUI();
        UpdateHighScoreUI();

        // Resets lives to 3 (if you come from the Game Over screen)
        currentLives = 3;
        UpdateLivesUI();

        // First check whether an old ball exists and destroy it (important when restarting).
        GameObject oldBall = GameObject.FindGameObjectWithTag("Ball");
        if (oldBall != null)
        {
            Destroy(oldBall);
        }

        ResetBall();
    }

    public void OpenLevelSelect()
    {
        playClickSound();
        SetState(GameState.SelectLevel);
    }

    private void SetLevelParameters(int maxRange, List<EquestionSymbol> allowedSymbols)
    {
        BrickManager.CurrentMaxEquestionValue = maxRange;
        BrickManager.AllowedSymbols = allowedSymbols;

        StartGame();
    }


    public void SelectLevel1()
    {
        playClickSound();
        SetLevelParameters(20, new List<EquestionSymbol> { EquestionSymbol.addition });
    }

    public void SelectLevel2()
    {
        playClickSound();
        SetLevelParameters(20, new List<EquestionSymbol> { EquestionSymbol.subtraction });
    }

    public void SelectLevel3()
    {
        playClickSound();
        SetLevelParameters(10, new List<EquestionSymbol> { EquestionSymbol.multiplication });
    }


    public void SelectLevel4()
    {
        playClickSound();
        SetLevelParameters(100, new List<EquestionSymbol> {
        EquestionSymbol.addition,
        EquestionSymbol.subtraction,
        EquestionSymbol.multiplication
    });
    }

    private void playClickSound()
    {
        if (audioSource != null && btnClickSound != null)
        {
            audioSource.PlayOneShot(btnClickSound);
        }
    }

    public void SetState(GameState newState)
    {
        // Deactivate all containers by default
        playingContainer.SetActive(false);
        livesUI.SetActive(false);
        gameOverUI.SetActive(false);
        mainMenuUi.SetActive(false);
        selectLevelUi.SetActive(false);
        createLevelUi.SetActive(false);

        // stop game time when game over
        Time.timeScale = (newState == GameState.Playing) ? 1f : 0f;

        // active new State
        switch (newState)
        {
            case GameState.Playing:
                playingContainer.SetActive(true);
                livesUI.SetActive(true);
                break;

            case GameState.GameOver:
                gameOverUI.SetActive(true);
                //gameOverUI.GameOverScore.updateUi(score,PlayerPrefs.GetInt("HighScore", 0).ToString());
                break;

            case GameState.MainMenu:
                mainMenuUi.SetActive(true);
                break;

            case GameState.SelectLevel:
                selectLevelUi.SetActive(true);
                break;

            case GameState.CreateLevel:
                createLevelUi.SetActive(true);
                break;

        }
    }
    public void UpdateScore(int points = 1)
	{
        score += points;
        // Update the ui;
        UpdateScoreUI();
        if (score > PlayerPrefs.GetInt("HighScore", 0))
		{
            PlayerPrefs.SetInt("HighScore", score);
            UpdateHighScoreUI();
		}


	}
    private void UpdateScoreUI()
	{
		// Find the score TextMeshProUGUI by tag Score and update its text
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }

	}
    private void UpdateHighScoreUI()
	{
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
	}


}


