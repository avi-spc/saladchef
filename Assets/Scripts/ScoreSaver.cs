using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSaver : MonoBehaviour
{
    public Spawner spawner;
    public UIDisplay uI;

    public int scoreIndex;

    public SortedSet<int> topScores = new SortedSet<int>();
    public List<int> t;

    public bool scoreAvailable;
    public bool getScores;

    public GameObject topScorePrefab;
    // Start is called before the first frame update
    void Start()
    {       
        scoreIndex = 1;
        getScores = true;
        scoreAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawner.gameOver && scoreAvailable) {
            SetTopScores();
            scoreAvailable = false;
        }
    }

    void SetTopScores() {

        while (getScores) {
            if (PlayerPrefs.HasKey(scoreIndex.ToString())) {
                topScores.Add(PlayerPrefs.GetInt(scoreIndex.ToString()));
            }
            else
                getScores = false;

            scoreIndex++;
        }

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


        for (int i = 0; i < t.Count; i++) {
            Debug.Log(t[i]);
        }
        
        ShowScores();
    }

    void ShowScores() {
        for (int i = 0; i < Mathf.Min(t.Count, 10); i++) {
            GameObject topScoreText = Instantiate(topScorePrefab);
            topScoreText.transform.SetParent(uI.topScorePanel.transform);
            topScoreText.GetComponent<Text>().text = PlayerPrefs.GetInt((i + 1).ToString()).ToString();
        }
    }
}
