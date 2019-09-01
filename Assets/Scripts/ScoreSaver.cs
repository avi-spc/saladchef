using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSaver : MonoBehaviour
{
    public Spawner spawner;
    public UIDisplay uI;

    int scoreIndex;

    SortedSet<int> topScores = new SortedSet<int>();        //Sorted set used to avoid same high score duplicacy
    List<int> t;

    bool scoreAvailable;
    bool getScores;

    public GameObject topScorePrefab;
    void Start()
    {
        t = new List<int>();    
        scoreIndex = 1;
        getScores = true;
        scoreAvailable = true;
    }

    void Update()
    {
        if (spawner.gameOver && scoreAvailable) {
            SetTopScores();
            scoreAvailable = false;
        }
    }

    void SetTopScores() {

        while (getScores) {     //get all scores stored in PlayerPrefs
            if (PlayerPrefs.HasKey(scoreIndex.ToString())) {
                topScores.Add(PlayerPrefs.GetInt(scoreIndex.ToString()));
            }
            else
                getScores = false;

            scoreIndex++;
        }

        //Add new scores to the PlayerPrefs
        topScores.Add(spawner.playerControllers[0].score);
        topScores.Add(spawner.playerControllers[1].score);
        
        topScores.Reverse();

        foreach (int score in topScores) {
            t.Add(score);
        }

        t.Sort();
        t.Reverse();

        for (int i = 0; i < Mathf.Min(t.Count, 10); i++) {
            PlayerPrefs.SetInt((i + 1).ToString(), t[i]);
        }

        PlayerPrefs.Save();

        ShowScores();
    }

    //Show top scores after the game is over
    void ShowScores() {
        for (int i = 0; i < Mathf.Min(t.Count, 10); i++) {
            GameObject topScoreText = Instantiate(topScorePrefab);
            topScoreText.transform.SetParent(uI.topScorePanel.transform);
            topScoreText.GetComponent<Text>().text = PlayerPrefs.GetInt((i + 1).ToString()).ToString();
        }
    }
}
