
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using TMPro;


public class BrickSpawner : MonoBehaviour
{
    public GameObject brick;
    public GameObject answer;
    public int AnswersAmount = 3;
    public float width;
    public float height;
    public float columnGap = 0.1f;
    public float rowGap = 0.1f;

    private Vector3 highestPoint;
    // Start is called before the first frame update
    void Start()
    {
        // Assuming transform.position is the desired bottom-left corner of the grid area.
        highestPoint = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        spawn();


    }
    [ContextMenu("Spawn Bricks")]
    void spawn()
    {
        float maxX = highestPoint.x + width;
        float maxY = transform.position.y; // Bottom edge Y

        float brickWidth = brick.GetComponent<Renderer>().bounds.size.x;
        float brickHeight = brick.GetComponent<Renderer>().bounds.size.y;


        for (float x = highestPoint.x; x < maxX; x += brickWidth + columnGap)
        {
            for (float y = highestPoint.y; y > maxY; y -= brickHeight + rowGap)
            {
                // Make them a child of the spawner for better hierarchy organization
                Instantiate(brick, new Vector3(x + brickWidth / 2, y - brickHeight / 2, transform.position.z), transform.rotation).transform.parent = transform;
            }
        }

    }
    [ContextMenu("Despawn Bricks")]
    void despawn()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    [ContextMenu("Respawn Bricks")]
    void Respawn()
    {
        despawn();
        spawn();
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        highestPoint = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        // Top-Left Corner is highestPoint
        Vector3 topLeft = highestPoint;

        // Define the other corners based on the size (width, height)
        Vector3 topRight = new Vector3(topLeft.x + width, topLeft.y, topLeft.z);
        Vector3 bottomLeft = new Vector3(topLeft.x, topLeft.y - height, topLeft.z);
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, topLeft.z);

        // Draw the lines for the bounding box
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

	public void SpawnAnswers(GameObject brick)
    {
        // Spawn answers object on the spawner line without them overlapping and with randomized positions
        float brickWidth = answer.GetComponent<Renderer>().bounds.size.x;

        // 1. Calculate the total number of available slots (object + space)
        float spacing = 0.5f; // Adjust this value for minimum space between objects
        float totalSlotWidth = brickWidth + spacing;
        int maxSlots = Mathf.FloorToInt(width / totalSlotWidth);

        // Ensure we don't try to spawn more than the available slots
        int answersToSpawn = Mathf.Min(AnswersAmount, maxSlots);

        // 2. Determine all possible center X-positions for the available slots
        List<float> possibleXPositions = new List<float>();
        // The starting x-position for the first object's center
        float startX = transform.position.x + totalSlotWidth / 2;

        for (int i = 0; i < maxSlots; i++)
        {
            float xPosition = startX + i * totalSlotWidth;
            possibleXPositions.Add(xPosition);
        }

        // 3. Randomize the order of the possible X-positions (Fisher-Yates shuffle is common)
        // This is a simple randomization that ensures all positions are considered randomly
        System.Random rng = new System.Random();
        int n = possibleXPositions.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            float value = possibleXPositions[k];
            possibleXPositions[k] = possibleXPositions[n];
            possibleXPositions[n] = value;
        }

        // Calculate the equestions value 
        int equationValue = brick.GetComponent<BrickManager>().CalculateEquestionValue();
        bool hasAnsweredBeenset = false;

        // 4. Instantiate the answers using the first 'AnswersAmount' randomized positions
        for (int i = 0; i < answersToSpawn; i++)
        {
            float xPosition = possibleXPositions[i];
            GameObject answerObject = Instantiate(answer, new Vector3(xPosition,transform.position.y, transform.position.z), transform.rotation);
            answerObject.transform.parent = transform;
            answerObject.GetComponent<AnswersManager>().answer = i;
            if (!hasAnsweredBeenset && i == answersToSpawn - 1)
            {
                answerObject.GetComponent<AnswersManager>().answer = equationValue;
                hasAnsweredBeenset = true;
            }else
			{
                // Get random chances to spawn correct answer in this position
                int chance = Random.Range(0, answersToSpawn);
                if (chance == 0 && !hasAnsweredBeenset)
                {
                    answerObject.GetComponent<AnswersManager>().answer = equationValue;
                    answerObject.GetComponent<AnswersManager>().isCorrectAnswer = true;
                    hasAnsweredBeenset = true;
                }else
				{
                    // set possible wrong answer within range of equation value 
                    // For now just random range
                    // TODO: Improve wrong answer generation logic
                    answerObject.GetComponent<AnswersManager>().answer = Random.Range(0, equationValue + Random.Range(-11, 20));
				}
			}
        }
        // Freeze the ball to let the player see the answers 
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            BallControl ballControl = ball.GetComponent<BallControl>();
            if (ballControl != null)
            {
                ballControl.ToggleFreeze();
            }
        }
    }
}
