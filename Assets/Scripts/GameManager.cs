using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState { Playing, GameOver, MainMenu, SelectLevel, CreateLevel, LeaderBoard}

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
    [SerializeField] private GameObject leaderBoardUI;

    [Header("Sounds")]
    [SerializeField] private AudioClip btnClickSound;
    [SerializeField] private AudioClip loseLifeSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip rightAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;

    [Header("AudioSources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicAudioSource;



    [Header("Rest")]
    [SerializeField] private Image[] lifeImages;

    [SerializeField] private Transform paddleTransform;

    [SerializeField] private GameObject ballPrefab;
    public Vector3 startPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI highScoreText;
    [SerializeField] public GameObject backgroundGreen;
    [SerializeField] public GameObject backgroundRed;



    private int currentLives = 3;
    private int score = 0;
        

    void Awake()
    {
        // Singleton-Pattern
        if (Instance == null)   
            Instance = this;
        else
            Destroy(gameObject);


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

    public void OpenLeaderBoard()
    {
        playClickSound();
        SetState(GameState.LeaderBoard);
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
        leaderBoardUI.SetActive(false);

        // stop game time when game over
        Time.timeScale = (newState == GameState.Playing) ? 1f : 0f;

        // active new State
        switch (newState)
        {
            case GameState.Playing:
                playingContainer.SetActive(true);
                livesUI.SetActive(true);

                // reduce volume
                if (musicAudioSource != null)
                {
                    musicAudioSource.volume = 0.1f;
                }

                PlaySong(backgroundMusic);
                break;

            case GameState.GameOver:
                gameOverUI.SetActive(true);
                gameOverUI.GetComponent<GameOverScore>().updateUi(score,PlayerPrefs.GetInt("HighScore", 0));
                //gameOverUI.GameOverScore.updateUi(score,PlayerPrefs.GetInt("HighScore", 0).ToString());
                if(audioSource != null)
                {
                    audioSource.PlayOneShot(gameOverSound);
                }
                // on game over we can start playing main meny theme again
                PlaySong(mainMenuMusic);
                break;

            case GameState.MainMenu:
                mainMenuUi.SetActive(true);
                PlaySong(mainMenuMusic);
                break;

            case GameState.SelectLevel:
                selectLevelUi.SetActive(true);
                break;

            case GameState.CreateLevel:
                createLevelUi.SetActive(true);
                break;
            case GameState.LeaderBoard:
                leaderBoardUI.SetActive(true);
                break;

        }
    }
    public void UpdateScore(int points = 1)
	{
        playRightAnswerSound();
        score += points;
        // Update the ui;
        UpdateScoreUI();
        if (score > PlayerPrefs.GetInt("HighScore", 0))
		{
            PlayerPrefs.SetInt("HighScore", score);
            UpdateHighScoreUI();
		}
	}

    public void playWrongAnswerSound()
    {
        if (audioSource != null)
        {
            audioSource.volume = 0.8f;
        }
        audioSource.PlayOneShot(wrongAnswerSound);
        StartCoroutine(FlashBackground(backgroundRed));

    }

    private void playRightAnswerSound()
    {
        if (audioSource != null)
        {
            audioSource.volume = 0.8f;
        }
        audioSource.PlayOneShot(rightAnswerSound);
        StartCoroutine(FlashBackground(backgroundGreen));
    }

    private IEnumerator FlashBackground(GameObject image,float duration = 0.5f)
    {
        if (image != null)
        {
            image.SetActive(true);       // Show background
            yield return new WaitForSeconds(duration); // Wait a short time
            image.SetActive(false);      // Hide background
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

    public void OpenMainMenu()
    {
        playClickSound();
        SetState(GameState.MainMenu);
    }
    public void PlaySong(AudioClip song)
    {
        if (musicAudioSource)
        {
            // 1. Stop the current audio immediately (clears the buffer)
            musicAudioSource.Stop(); 

            // 2. Assign the new song to the clip property
            musicAudioSource.clip = song;

            // 3. Play the new song from the beginning
            musicAudioSource.Play();
        }
    }


}


