using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public GameObject UIDisplay;

    public GameObject spawnPrefab;

    public GameObject customerPrefab;

    public Customer[] allCustomers;
    // Start is called before the first frame update
    public int numOfCustomers;
    public List<int> emptySpots;
    public List<int> placesToBeOccupied;

    System.Random rnd = new System.Random();

    public bool gameOver;

    public PlayerController[] playerControllers;

    [System.Serializable]
    public struct SpawnPoint {
        public GameObject spawnPoint;
        public bool isOccupied;
    }

    public SpawnPoint[] customerSpawnPoints;


    private void Awake() {
        customerSpawnPoints = new SpawnPoint[5];
        float startX = -5.92f;
        for (int i = 0; i < customerSpawnPoints.Length; i++) {
            customerSpawnPoints[i].spawnPoint = Instantiate(spawnPrefab, new Vector3(startX, 6.14f, 0), Quaternion.identity);
            customerSpawnPoints[i].spawnPoint.transform.SetParent(GameObject.Find("CustomerSpawnPoints").transform);
            customerSpawnPoints[i].spawnPoint.name = "SpanPoint_" + i;
            customerSpawnPoints[i].isOccupied = false;

            startX += 2.7f;
        }
    }
    void Start()
    {
        playerControllers = FindObjectsOfType<PlayerController>();

        StartCoroutine(CallSpwaner());
    }

    // Update is called once per frame
    void Update() {
        if (playerControllers[0].timer == 0 && playerControllers[1].timer == 0) {
            gameOver = true;

            UIDisplay.GetComponent<UIDisplay>().gameOverPanel.SetActive(true);

            UIDisplay.GetComponent<UIDisplay>().player1_finalScore.text = playerControllers[1].score.ToString();
            UIDisplay.GetComponent<UIDisplay>().player2_finalScore.text = playerControllers[0].score.ToString();

        }

        if (gameOver) {
            allCustomers = FindObjectsOfType<Customer>();
            foreach (var c in allCustomers) {
                Destroy(c.gameObject);
            }

            UIDisplay.GetComponent<UIDisplay>().winnerText.text = Mathf.Max(playerControllers[0].score, playerControllers[1].score) == playerControllers[1].score ? "Player " + playerControllers[1].playerType : "Player " + playerControllers[0].playerType;
        }
    }

    IEnumerator CallSpwaner() {
        while (!gameOver) {
            CalculateEmptySpots();
            yield return new WaitForSeconds(28f);
        }
    }

    void CalculateEmptySpots() {
        emptySpots.Clear();
        for (int i = 0; i < customerSpawnPoints.Length; i++) {
            if (!customerSpawnPoints[i].isOccupied) {
                emptySpots.Add(i);
            }
        }

        numOfCustomers = (70 * emptySpots.Count) / 100;

        placesToBeOccupied.Clear();

        IEnumerable<int> g = emptySpots.OrderBy(x => rnd.Next()).Take(numOfCustomers);

        foreach (int index in g) {
            placesToBeOccupied.Add(index);
        }

        CustomerSpawner(placesToBeOccupied);
    }

    public void CustomerSpawner(List<int> spawnPositions) {
        for (int i = 0; i < placesToBeOccupied.Count; i++) {
            GameObject customer = Instantiate(customerPrefab, customerSpawnPoints[spawnPositions[i]].spawnPoint.transform.position, customerSpawnPoints[spawnPositions[i]].spawnPoint.transform.rotation);
            //customer.transform.SetParent(customerSpawnPoints[spawnPositions[i]].spawnPoint.transform);
            customer.GetComponent<Customer>().spawnedIndex = spawnPositions[i];
            customerSpawnPoints[spawnPositions[i]].isOccupied = true;
        }
    }
}
