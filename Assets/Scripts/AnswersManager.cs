using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswersManager : MonoBehaviour
{
    public int answer;
    public bool isCorrectAnswer = false;
    public GameObject equestionText;
    public BrickSpawner parent;

    public Vector2 screenOffset = new Vector2(0, -10f);

    private GameObject equestionObject;
    public float relativeTextSize = 0.5f;

    void Awake()
    {
    }

    void Start()
    {
        // WICHTIG: GetComponentInChildren verwenden für importierte Modelle
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer == null) return;

        // Find the Canvas Transform
        GameObject[] canvases = GameObject.FindGameObjectsWithTag("UICanvas");
        if (canvases.Length == 0) return;
        Transform uiCanvasTransform = canvases[0].transform;

        // Instantiate the text prefab as a child of the Canvas
        equestionObject = Instantiate(equestionText, uiCanvasTransform);
        TextMeshProUGUI tmpComponent = equestionObject.GetComponent<TextMeshProUGUI>();

        if (tmpComponent == null)
        {
            tmpComponent = equestionObject.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmpComponent == null)
            {
                Destroy(equestionObject);
                return;
            }
        }

        Vector3 targetWorldPosition = renderer.bounds.center;

        // Project World Point to Screen/Canvas Point
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetWorldPosition);

        // Check if brick is in front of the camera
        if (screenPosition.z < 0)
        {
            Destroy(equestionObject);
            return;
        }

        screenPosition.x += screenOffset.x;
        screenPosition.y += screenOffset.y;

        RectTransform rt = equestionObject.GetComponent<RectTransform>();
        rt.position = screenPosition;

        tmpComponent.fontSize = 35;
        tmpComponent.rectTransform.sizeDelta = new Vector2(120, 60);
        tmpComponent.color = Color.white;

        tmpComponent.text = answer.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        // Move the text along with the brick
        if (equestionObject != null)
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null) return;

            // Wir nutzen auch hier bounds.center für konsistente Positionierung
            Vector3 targetWorldPosition = renderer.bounds.center;

            // Project World Point to Screen/Canvas Point
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetWorldPosition);

            // NEU: Offset auch im Update anwenden, damit es nicht springt
            screenPosition.x += screenOffset.x;
            screenPosition.y += screenOffset.y;

            // Update UI Text Position
            equestionObject.GetComponent<RectTransform>().position = screenPosition;
        }
    }

    void OnDestroy()
    {
        if (equestionObject != null)
        {
            Destroy(equestionObject);
            equestionObject = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paddle"))
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                if (isCorrectAnswer)
                {
                    gameManager.UpdateScore(1);
                } else
                {
                    gameManager.playWrongAnswerSound();
                }
            }

            GameObject[] answerBricks = GameObject.FindGameObjectsWithTag("Answers");
            foreach (GameObject brick in answerBricks)
            {
                Destroy(brick);
            }
            GameObject.FindGameObjectWithTag("Paddle").GetComponentInChildren<Animator>().SetTrigger("Close");

            GameObject ballObject = GameObject.FindGameObjectWithTag("Ball");
            if (ballObject != null)
            {
                BallControl ballControl = ballObject.GetComponent<BallControl>();
                if (ballControl != null)
                {
                    ballControl.ToggleFreeze();
                }
            }
        }
        else if (other.CompareTag("DeathZone"))
        {
            parent.AnswerDestroyed(this.gameObject);
        }
    }
}