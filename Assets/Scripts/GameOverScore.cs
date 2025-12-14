using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScore : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI highScore;
    public Transform scrollContent;
    public GameObject answerPrefab;


    public void updateUi(int scoreValue, int highScoreValue, List<Equestion> givenAnswers)
    {
        if (score != null)
        {
            score.text = "Score: " + scoreValue.ToString();
        }
        if (highScore != null)
        {
            highScore.text = "High Score: " + highScoreValue.ToString();
        }
        Populate(givenAnswers);

    }

    public void Populate(List<Equestion> answers)
    {
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Equestion ans in answers)
        {
            GameObject newAnswer = Instantiate(answerPrefab, scrollContent);
            TMP_Text taskText = newAnswer.transform.Find("Task").GetComponent<TMP_Text>();
            TMP_Text answerText = newAnswer.transform.Find("Answer").GetComponent<TMP_Text>();


            taskText.text =ans.StringifyEquestion();
            if (ans.correctAnswer == ans.answer)
            {
                answerText.color = Color.green;
                answerText.text = ans.correctAnswer.ToString();
            }
            else
            {

                answerText.color = Color.red;
                answerText.text = ans.answer.ToString() + " (" + ans.correctAnswer.ToString() + ")";
            }
        }

    }
}