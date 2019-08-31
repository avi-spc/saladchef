using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIDisplay : MonoBehaviour
{
    public Text player1_Timer;
    public Text player1_Score;

    public Text player2_Timer;
    public Text player2_Score;

    public GameObject gameOverPanel;

    public Text player1_finalScore;
    public Text player2_finalScore;

    public Text winnerText;

    public GameObject topScorePanel;


    // Start is called before the first frame update
    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
              
    }

    public void RestartGame() {
        SceneManager.LoadScene("SampleScene");
    }
}
