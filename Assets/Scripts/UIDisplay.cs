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


    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
              
    }

    //Restart the match
    public void RestartGame() {
        SceneManager.LoadScene("SampleScene");
    }
}
