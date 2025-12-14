using System.Collections.Generic;
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
    public GameObject heartPrefab;

    private GameObject equestionObject;
    public float relativeTextSize = 0.5f;

    private CameraShake cameraShake;

    private ExplosionEffect explosionEffect;

    public static int CurrentMinValue = 1;
    public static int CurrentMaxEquestionValue = 10;
    public static List<EquestionSymbol> AllowedSymbols = new List<EquestionSymbol> { EquestionSymbol.addition }; // Standard: Nur Addition

    public bool isBeingDestroyedBySystem = false;

    public string mathTask;

    private bool hasHeart = false;


    void Start()
    {

        if (cameraShake == null)
        {
            cameraShake = Camera.main.GetComponent<CameraShake>();
        }

        explosionEffect = GetComponent<ExplosionEffect>();

    }

    void OnDestroy()
    {
        if (isBeingDestroyedBySystem)
        {
            HideEquestion();
            return;
        }
        if(hasHeart)
        {
            GameManager.Instance.RecoverHeart();
        }
        if (equestionObject != null && !hasHeart)
        {
            GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
            GameManager.Instance.CollectEquestion(equestion, symbol);
            if (spawner != null)
            {
                spawner.GetComponent<BrickSpawner>().SpawnAnswers(gameObject);

                var paddle = GameObject.FindGameObjectWithTag("Paddle");
                if (paddle != null)
                    paddle.GetComponentInChildren<Animator>().SetTrigger("Open");
            }

        }
        HideEquestion();
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            cameraShake.TriggerShake();

            if (explosionEffect != null)
            {
                explosionEffect.Explode();
            }
            GameManager.Instance.UpdateScore(1);
            Animator animator = gameObject.GetComponentInChildren<Animator>();
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.Play("Destroy", 0, 0f);
                Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
                return;
            }
            Destroy(gameObject);
            return;
        }
    }
    public void GenerateEquestion()
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

        // +1, da Random.Range beim Max-Wert exklusiv ist (10 wird sonst nie gewürfelt)
        int currentMax = CurrentMaxEquestionValue;
        int currentMin = CurrentMinValue;

        // Sicherheitscheck
        if (currentMin > currentMax) currentMin = currentMax;

        // Temporäre Variablen zum Würfeln
        int val1 = 0;
        int val2 = 0;

        switch (symbol)
        {
            case EquestionSymbol.addition:
                // Beide Zahlen müssen einfach nur zwischen Min und Max liegen
                firstValue = UnityEngine.Random.Range(currentMin, currentMax + 1);
                secondValue = UnityEngine.Random.Range(currentMin, currentMax + 1);
                break;

            case EquestionSymbol.subtraction:
                // Wir würfeln zwei erlaubte Zahlen
                val1 = UnityEngine.Random.Range(currentMin, currentMax + 1);
                val2 = UnityEngine.Random.Range(currentMin, currentMax + 1);

                // Damit keine negativen Zahlen rauskommen (z.B. 5 - 10),
                // setzen wir die größere Zahl immer als firstValue.
                firstValue = Mathf.Max(val1, val2);
                secondValue = Mathf.Min(val1, val2);
                break;

            case EquestionSymbol.multiplication:
                // Auch hier: Beide Zahlen strikt zwischen Min und Max
                firstValue = UnityEngine.Random.Range(currentMin, currentMax + 1);
                secondValue = UnityEngine.Random.Range(currentMin, currentMax + 1);
                break;
        }

        equestion = new Vector2(firstValue, secondValue);
    }
    public void PrepareAndDisplayEquestion()
    {
        string sign = "";
        switch (symbol)
        {
            case EquestionSymbol.addition: sign = "+"; break;
            case EquestionSymbol.subtraction: sign = "-"; break;
            case EquestionSymbol.multiplication: sign = "×"; break;
        }

        string equationString = $"{equestion.x} {sign} {equestion.y}";

        // Call the generalized function
        equestionObject = PrepareAndDisplayText(equestionText, equationString, relativeTextSize);
    }
    public GameObject PrepareAndDisplayText(GameObject textPrefab, string content, float relativeSizeMultiplier)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return null;

        GameObject[] canvases = GameObject.FindGameObjectsWithTag("UICanvas");
        if (canvases.Length == 0) return null;
        Transform uiCanvasTransform = canvases[0].transform;

        GameObject textObject = Instantiate(textPrefab, uiCanvasTransform);
        TextMeshProUGUI tmpComponent = textObject.GetComponent<TextMeshProUGUI>();

        if (tmpComponent == null)
        {
            tmpComponent = textObject.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmpComponent == null)
            {
                Destroy(textObject);
                return null;
            }
        }

        Vector3 size = renderer.bounds.size;

        // Calculate the position of the brick's local Z-face in World Space
        Vector3 brickSurfaceWorldPosition = transform.position + transform.forward * (size.z / 2f);

        // Add a slight offset in World Space to pull the text off the surface.
        float offset = 0.01f;
        Vector3 textWorldPosition = brickSurfaceWorldPosition + transform.forward * offset;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(textWorldPosition);

        if (screenPosition.z < 0)
        {
            Destroy(textObject);
            return null;
        }

        float worldHeightInPixels = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * size.y).y - screenPosition.y;
        float dynamicFontSize = worldHeightInPixels * relativeSizeMultiplier;

        textObject.GetComponent<RectTransform>().position = screenPosition;

        tmpComponent.fontSize = dynamicFontSize;
        tmpComponent.rectTransform.sizeDelta = new Vector2(worldHeightInPixels * 2.5f, worldHeightInPixels);

        tmpComponent.text = content;

        textObject.SetActive(true);

        return textObject;
    }


    public void HideEquestion()
    {
        if (equestionObject != null)
        {
            Destroy(equestionObject);
            equestionObject = null;
        }
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
                mathTask = equestion.x + " + " + equestion.y; return (int)(equestion.x + equestion.y);
            case EquestionSymbol.subtraction:
                mathTask = equestion.x + " - " + equestion.y; return (int)(equestion.x - equestion.y);
            case EquestionSymbol.multiplication:
                mathTask = equestion.x + " * " + equestion.y; return (int)(equestion.x * equestion.y);
            default:
                return -1;
        }
    }
    public void GiveHeart()
    {
        // We cannot have both healing and numbers
        if (equestionObject == null)
        {
            hasHeart = true;
            equestionObject = PrepareAndDisplayText(heartPrefab, "♥", 0.8f);
        }
    }

}
