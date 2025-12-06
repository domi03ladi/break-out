using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswersManager : MonoBehaviour
{
    public int answer;
    public bool isCorrectAnswer = false;
    // Start is called before the first frame update
    public GameObject equestionText;

    private GameObject equestionObject;
    public float relativeTextSize = 0.5f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
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
            // Fallback for complex prefab structure
            tmpComponent = equestionObject.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmpComponent == null)
            {
                Destroy(equestionObject);
                return;
            }
        }

        // Calculate World Position of the brick's front face (-Z)
        Vector3 size = renderer.bounds.size;
        Vector3 brickFrontWorldPosition = transform.position;
        brickFrontWorldPosition.z -= (size.z / 2f);

        // Project World Point to Screen/Canvas Point
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(brickFrontWorldPosition);

        // Check if brick is in front of the camera
        if (screenPosition.z < 0)
        {
            Destroy(equestionObject);
            return;
        }
        // Dynamic Sizing (based on screen space projection of the brick height)
        float worldHeightInPixels = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * size.y).y - screenPosition.y;
        float dynamicFontSize = worldHeightInPixels * relativeTextSize;

        // Set UI Text Position
        equestionObject.GetComponent<RectTransform>().position = screenPosition;

        tmpComponent.fontSize = dynamicFontSize;
        tmpComponent.rectTransform.sizeDelta = new Vector2(worldHeightInPixels * 2.5f, worldHeightInPixels);

        tmpComponent.text = answer.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the text along with the brick
        if (equestionObject != null)
		{
            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null) return;

            // Project World Point to Screen/Canvas Point
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

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
        // Check if the collding object is a paddle
        if (other.CompareTag("Paddle"))
        {
            // Notify GameManager about the answer selection
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
			{
				if(isCorrectAnswer)
                {
                    gameManager.UpdateScore(1);
                }
			}
            // Find all the answers bricks in scene by tag "Answers" and destroy them
            GameObject[] answerBricks = GameObject.FindGameObjectsWithTag("Answers");
            foreach (GameObject brick in answerBricks)
            {
                Destroy(brick);
            }
            // Freeze the ball by finding the ballControl script and calling ToggleFreeze
            // Find the ball object by tag "Ball"
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
		
	}
}
