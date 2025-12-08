using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public enum EquestionSymbol
{
    addition,
    subtraction,
    multiplication,
}


public class BrickManager : MonoBehaviour
{
    private Vector2 equestion;
    private EquestionSymbol symbol;
    public GameObject equestionText;

    private GameObject equestionObject;
    public float relativeTextSize = 0.5f;


    // I don't really like them being here 
    // putting this on a spawnder would make more sense 
    public float equestionSpanwChance = 0.1f;
    public static int CurrentMaxEquestionValue = 10;
    public static List<EquestionSymbol> AllowedSymbols = new List<EquestionSymbol> { EquestionSymbol.addition }; // Standard: Nur Addition

    // Start is called before the first frame update
    void Start()
    {

        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= equestionSpanwChance)
        {
            GenerateEquestion();
            PrepareAndDisplayEquestion();
        }

    }

    void OnDestroy()
    {
        if (equestionObject != null)
        {
            // Spawn answers when the brick is destroyed
            GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
            if (spawner != null)
            {
                spawner.GetComponent<BrickSpawner>().SpawnAnswers(gameObject);
                GameObject.FindGameObjectWithTag("Paddle").GetComponentInChildren<Animator>().SetTrigger("Open");
            }
            Destroy(equestionObject);
            equestionObject = null;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Animator animator = gameObject.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Play("Destroy", 0, 0f);
            }
            // This ensures the physics engine handles the collision resolution before the object vanishes.
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
            return;
        }

    }
    void GenerateEquestion()
    {

        if (AllowedSymbols.Count == 0)
        {
            return;
        }

        // Decide on a symbol
        int symbolIndexInList = UnityEngine.Random.Range(0, AllowedSymbols.Count);
        symbol = AllowedSymbols[symbolIndexInList];

        int firstValue = 0;
        int secondValue = 0;

        int currentMax = CurrentMaxEquestionValue;

        switch (symbol)
        {
            case EquestionSymbol.addition:
                firstValue = UnityEngine.Random.Range(1, currentMax);
                secondValue = UnityEngine.Random.Range(1, currentMax - firstValue);
                break;
            case EquestionSymbol.subtraction:
                firstValue = UnityEngine.Random.Range(1, currentMax);
                secondValue = UnityEngine.Random.Range(1, firstValue);
                break;
            case EquestionSymbol.multiplication:
                firstValue = UnityEngine.Random.Range(1, (int)Mathf.Sqrt(currentMax));
                secondValue = UnityEngine.Random.Range(1, (int)Mathf.Sqrt(currentMax));
                break;
        }

        equestion = new Vector2(firstValue, secondValue);
    }

    void PrepareAndDisplayEquestion()
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

        // Set Content
        string sign = "";
        switch (symbol)
        {
            case EquestionSymbol.addition: sign = "+"; break;
            case EquestionSymbol.subtraction: sign = "-"; break;
            case EquestionSymbol.multiplication: sign = "×"; break;
        }

        string equationString = $"{equestion.x} {sign} {equestion.y}";
        tmpComponent.text = equationString;

        equestionObject.SetActive(true);
    }
    public int CalculateEquestionValue()
	{
		if (equestionObject == null)
		{
            return -1; // No equation associated
		}
        switch (symbol)
        {
            case EquestionSymbol.addition:
                return (int)(equestion.x + equestion.y);
            case EquestionSymbol.subtraction:
                return (int)(equestion.x - equestion.y);
            case EquestionSymbol.multiplication:
                return (int)(equestion.x * equestion.y);
            default:
                return -1;
        }
	}

}
