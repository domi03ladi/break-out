
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
    public List<GameObject> bricks;
    public int AnswersAmount = 3;
    public float width;
    public float height;
    public float columnGap = 0.1f;
    public float rowGap = 0.1f;
    private List<int> questionBrickIndices = new List<int>();
    public List<GameObject> spawnedAnswers = new List<GameObject>();
    public List<(string, bool)> givenAnswers = new List<(string, bool)>();

    public float[] answerVerticalOffsets = { 0.1f, 0.5f, 1f };

    public bool isRespawning = false;

    private Vector3 highestPoint;
    private IEnumerator bricks_generator; // Change type to IEnumerator

    // Start is called before the first frame update
    void Start()
    {
        bricks = new List<GameObject>
        {
            GameObject.Find("Earth_Brick2"),
            GameObject.Find("Neptune_Brick"),
            GameObject.Find("Venus_Brick"),
            GameObject.Find("Moon_Brick"),
            GameObject.Find("Jupiter_Brick"),
            GameObject.Find("Mars_Brick")
        };
        // Assuming transform.position is the desired bottom-left corner of the grid area.
        highestPoint = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        //Respawn();


    }


    void Update()
    {
        // Process the generator one step per frame if respawning is active
        if (isRespawning && bricks_generator != null)
        {
            // Try to move the iterator to the next yielded value (i.e., generate the next brick)
            bool keepGoing = bricks_generator.MoveNext();

            if (!keepGoing)
            {
                isRespawning = false;
                bricks_generator = null;
            }
        }
    }
    public IEnumerator Gen_Bricks()
    {
        questionBrickIndices.Clear();

        float maxX = highestPoint.x + width;
        float maxY = transform.position.y;

        float brickWidth = brick.GetComponent<Renderer>().bounds.size.x;
        float brickHeight = brick.GetComponent<Renderer>().bounds.size.y;

        int columns = Mathf.FloorToInt((width + columnGap) / (brickWidth + columnGap));
        int rows = Mathf.FloorToInt((height + rowGap) / (brickHeight + rowGap));

        PrepareQuestionIndices(columns, rows);

        int currentBrickIndex = 0;

        for (float x = highestPoint.x; x < maxX; x += brickWidth + columnGap)
        {
            for (float y = highestPoint.y; y > maxY; y -= brickHeight + rowGap)
            {
                GameObject selectedBrick = GetRandomBrick();
                // For testing only
                //GameObject selectedBrick = brick;

                if (selectedBrick != null)
                {
                    GameObject newBrick = Instantiate(selectedBrick, new Vector3(x + brickWidth / 2, y - brickHeight / 2, transform.position.z), transform.rotation, transform);

                    if (questionBrickIndices.Contains(currentBrickIndex))
                    {
                        BrickManager manager = newBrick.GetComponent<BrickManager>();
                        if (manager != null)
                        {
                            manager.GenerateEquestion();
                            manager.PrepareAndDisplayEquestion();
                            newBrick.GetComponentInChildren<Animator>().enabled = false;
                        }
                    }

                    currentBrickIndex++;
                }
                yield return true;
            }
        }
        yield return false;
    }
    [ContextMenu("Spawn Bricks")]
    void Spawn()
    {
        questionBrickIndices.Clear();

        float maxX = highestPoint.x + width;
        float maxY = transform.position.y;

        float brickWidth = brick.GetComponent<Renderer>().bounds.size.x;
        float brickHeight = brick.GetComponent<Renderer>().bounds.size.y;

        int columns = Mathf.FloorToInt((width + columnGap) / (brickWidth + columnGap));
        int rows = Mathf.FloorToInt((height + rowGap) / (brickHeight + rowGap));

        PrepareQuestionIndices(columns, rows);

        int currentBrickIndex = 0;

        for (float x = highestPoint.x; x < maxX; x += brickWidth + columnGap)
        {
            for (float y = highestPoint.y; y > maxY; y -= brickHeight + rowGap)
            {
                GameObject selectedBrick = GetRandomBrick();
                // For testing only
                //GameObject selectedBrick = brick;

                if (selectedBrick != null)
                {
                    GameObject newBrick = Instantiate(selectedBrick, new Vector3(x + brickWidth / 2, y - brickHeight / 2, transform.position.z), transform.rotation, transform);

                    if (questionBrickIndices.Contains(currentBrickIndex))
                    {
                        BrickManager manager = newBrick.GetComponent<BrickManager>();
                        if (manager != null)
                        {
                            manager.GenerateEquestion();
                            manager.PrepareAndDisplayEquestion();
                        }
                    }

                    currentBrickIndex++;
                }
            }
        }
    }



    [ContextMenu("Despawn Bricks")]
    public void DespawnBricks()
    {
        questionBrickIndices.Clear();
        givenAnswers.Clear();

        // destroy all objects
        foreach (GameObject ans in spawnedAnswers)
        {
            if (ans != null) Destroy(ans);
        }
        spawnedAnswers.Clear();

        GameObject[] looseAnswers = GameObject.FindGameObjectsWithTag("Answers");
        foreach (GameObject a in looseAnswers)
        {
            Destroy(a);
        }

        foreach (Transform child in transform)
        {
            BrickManager bm = child.GetComponent<BrickManager>();

            if (bm != null)
            {
                bm.isBeingDestroyedBySystem = true;
            }

            Destroy(child.gameObject);
        }
    }


    [ContextMenu("Reset Bricks")]
    void Reset()
    {
        DespawnBricks();
        Spawn();
    }

    public void Respawn()
    {
        isRespawning = true;
        // Start the generator/iterator
        bricks_generator = Gen_Bricks();
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
        spawnedAnswers.Clear();

        // 1. Calculate the total number of available slots (object + space)
        float spacing = 5f; // Adjust this value for minimum space between objects
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
        BrickManager brickManager = brick.GetComponent<BrickManager>();
        int equationValue = brickManager.CalculateEquestionValue();
        string mathTask = brickManager.mathTask;
        bool hasAnsweredBeenset = false;

        HashSet<int> usedNumbers = new HashSet<int>();
        usedNumbers.Add(equationValue);

        // 4. Instantiate the answers using the first 'AnswersAmount' randomized positions
        for (int i = 0; i < answersToSpawn; i++)
        {
            float xPosition = possibleXPositions[i];

            float currentOffset = 0f;

            if (i < answerVerticalOffsets.Length)
            {
                // Nutze den definierten Offset aus dem Array
                currentOffset = answerVerticalOffsets[i];
            }
            else
            {
                // F�llt auf 0 zur�ck, falls mehr als 3 Antworten gespawnt werden (Sicherheitsfall)
                Debug.LogWarning("More answers to spawn than oPrepareQuestionIndicesffsets defined! Using 0 offset.");
            }

            float spawnY = transform.position.y - currentOffset;

            GameObject answerObject = Instantiate(answer,
                new Vector3(xPosition, spawnY, transform.position.z),
                transform.rotation);

            answerObject.transform.parent = transform;
            answerObject.GetComponent<AnswersManager>().parent = this;

            int assignedNumber;
            if (!hasAnsweredBeenset && (i == answersToSpawn - 1 || Random.Range(0, answersToSpawn - i) == 0))
            {
                assignedNumber = equationValue;
                answerObject.GetComponent<AnswersManager>().isCorrectAnswer = true;
                hasAnsweredBeenset = true;
            }
            else
            {
                do
                {
                    assignedNumber = Random.Range(Mathf.Max(0, equationValue - 10), equationValue + 20);
                } while (usedNumbers.Contains(assignedNumber));
            }
            answerObject.GetComponent<AnswersManager>().answer = assignedNumber;
            answerObject.GetComponent<AnswersManager>().mathTask = mathTask;
            usedNumbers.Add(assignedNumber);
            spawnedAnswers.Add(answerObject);
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
    GameObject GetRandomBrick()
    {
        if (bricks == null || bricks.Count == 0)
        {
            Debug.LogError("No bricks assigned!");
            return null;
        }

        int index = Random.Range(0, bricks.Count);
        return bricks[index];
    }

    private void PrepareQuestionIndices(int cols, int rows)
    {
        questionBrickIndices.Clear();

        // Hinweis: Dein Loop läuft erst Zeile für Zeile (Y) innerhalb einer Spalte (X).
        // Index-Formel: (Spalte * AnzahlZeilen) + Zeile

        // 1. Linke Obere Ecke (Spalte 0, erste Zeile)
        int topLeft = 0;
        questionBrickIndices.Add(topLeft);

        // 2. Linke Untere Ecke (Spalte 0, letzte Zeile)
        int bottomLeft = rows - 1;
        questionBrickIndices.Add(bottomLeft);

        // 3. Rechte Obere Ecke (Letzte Spalte, erste Zeile)
        int topRight = (cols - 1) * rows;
        questionBrickIndices.Add(topRight);

        // 4. Rechte Untere Ecke (Letzte Spalte, letzte Zeile)
        int bottomRight = (cols - 1) * rows + (rows - 1);
        questionBrickIndices.Add(bottomRight);

        // 5. Mitte (Mittlere Spalte, Mittlere Zeile)
        int midCol = cols / 2;
        int midRow = rows / 2;
        int center = (midCol * rows) + midRow;
        questionBrickIndices.Add(center);
    }

    public void AnswerDestroyed(GameObject answer)
    {
        spawnedAnswers.Remove(answer);
        if (spawnedAnswers.Count == 0)
        {
            GameManager.Instance.CollectAnwser(-999);
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

    public void AnswerDestroyed(string answer, bool isCorrect)
    {
        givenAnswers.Add((answer, isCorrect));
    }


}
