using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Playing, GameOver }

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;



    [Header("Game State Containers")]
    [SerializeField] private GameObject playingContainer;
    [SerializeField] private GameObject livesUI;             
    [SerializeField] private GameObject gameOverUI;

    // LEBENS-LOGIK
    [Header("Lives UI")]
    // Im Inspector die drei Planeten-Images zuweisen!
    [SerializeField] private Image[] lifeImages;

    [SerializeField] private Transform paddleTransform;

    [SerializeField] private GameObject ballPrefab;
    private Vector3 startPosition = new Vector3(0f, 2f, 0f);

    // Die aktuelle Anzahl der Leben
    private int currentLives = 3;


    void Awake()
    {
        // Singleton-Pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // show all lives at the beginning
        UpdateLivesUI();
        SetState(GameState.Playing);
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
        if(currentLives > 0)
        {
            currentLives--;
            UpdateLivesUI();

            if (currentLives == 0)
            {
                //Spiel beenden (Game Over)
                Debug.Log("Game Over! All lives lost");
                SetState(GameState.GameOver);
            }
            else
            {
                //Spiel zurücksetzen (Ball respawnen, Paddle zurücksetzen)
                Debug.Log("Leben verloren. Starte neue Runde.");
                ResetBall(); // Implementiere diese Methode, um den Ball neu zu positionieren
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
        // Prüfen, ob noch Leben übrig sind (ist in LoseLife() schon passiert, aber zur Sicherheit)
        if (currentLives > 0)
        {
            Debug.Log("Ball wird in der Mitte neu gestartet.");
            GameObject newBall = Instantiate(ballPrefab, startPosition, Quaternion.identity);
            paddleTransform.position = new Vector3(0f, paddleTransform.position.y, paddleTransform.position.z);

        }
    }
    
    public void SetState(GameState newState)
    {
        // Deactivate all containers by default
        playingContainer.SetActive(false);
        livesUI.SetActive(false);
        gameOverUI.SetActive(false);

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
                playingContainer.SetActive(false);
                livesUI.SetActive(false);
                gameOverUI.SetActive(true);
                break;
        }
    }

}


