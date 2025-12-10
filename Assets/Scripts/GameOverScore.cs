using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScore : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI highScore;


    public void updateUi(int scoreValue, int highScoreValue) 
    {
        if (score != null)
        {
            score.text = "Score: " + scoreValue.ToString();
        }
        if (highScore != null)
        {
            highScore.text = "High Score: " + highScoreValue.ToString();
        }


    }


}