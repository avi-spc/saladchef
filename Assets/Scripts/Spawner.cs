using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    System.Random rnd = new System.Random();

    public GameObject UIDisplay;

    public GameObject spawnPrefab;
    public GameObject customerPrefab;

    Customer[] allCustomers;

    int numOfCustomers;
    List<int> emptySpots;
    List<int> placesToBeOccupied;


    public bool gameOver;

    public PlayerController[] playerControllers;

    [System.Serializable]
    public struct SpawnPoint {
        public GameObject spawnPoint;
        public bool isOccupied;
    }

    public SpawnPoint[] customerSpawnPoints;


    private void Awake() {
        emptySpots = new List<int>();
        placesToBeOccupied = new List<int>();

        customerSpawnPoints = new SpawnPoint[5];
        float startX = -5.92f;

        //creating spawnpoints for the customers
        for (int i = 0; i < customerSpawnPoints.Length; i++) {
            customerSpawnPoints[i].spawnPoint = Instantiate(spawnPrefab, new Vector3(startX, 6.14f, 0), Quaternion.identity);
            customerSpawnPoints[i].spawnPoint.transform.SetParent(GameObject.Find("CustomerSpawnPoints").transform);
            customerSpawnPoints[i].spawnPoint.name = "SpanPoint_" + i;
            customerSpawnPoints[i].isOccupied = false;

            startX += 2.7f;
        }
    }

    void Start() {
        playerControllers = FindObjectsOfType<PlayerController>();

        StartCoroutine(CallSpwaner());
    }

    void Update() {
        if (playerControllers[0].timer == 0 && playerControllers[1].timer == 0) {
            gameOver = true;

            UIDisplay.GetComponent<UIDisplay>().gameOverPanel.SetActive(true);

            UIDisplay.GetComponent<UIDisplay>().player1_finalScore.text = playerControllers[1].score.ToString();
            UIDisplay.GetComponent<UIDisplay>().player2_finalScore.text = playerControllers[0].score.ToString();

        }

        if (gameOver) {     //If game over destroy all customers that are currently waiting and display winner
            allCustomers = FindObjectsOfType<Customer>();
            foreach (var c in allCustomers) {
                Destroy(c.gameObject);
            }

            UIDisplay.GetComponent<UIDisplay>().winnerText.text = (Mathf.Max(playerControllers[0].score, playerControllers[1].score) == playerControllers[1].score ? "Player " + playerControllers[1].playerType : "Player " + playerControllers[0].playerType) + " wins";  
        }
    }

    IEnumerator CallSpwaner() {
        while (!gameOver) {
            CalculateEmptySpots();
            yield return new WaitForSeconds(20f);
        }
    }

    //Calculate empty spots for customer to be spawned to avoid double spaning at the same spot 
    void CalculateEmptySpots() {
        emptySpots.Clear();
        for (int i = 0; i < customerSpawnPoints.Length; i++) {
            if (!customerSpawnPoints[i].isOccupied) {
                emptySpots.Add(i);
            }
        }

        numOfCustomers = (70 * emptySpots.Count) / 100;     //customers will be spawned occupying 70% of the empty spots

        placesToBeOccupied.Clear();

        IEnumerable<int> g = emptySpots.OrderBy(x => rnd.Next()).Take(numOfCustomers);

        foreach (int index in g) {
            placesToBeOccupied.Add(index);
        }

        CustomerSpawner(placesToBeOccupied);
    }

    //Spawn customers at the empty spots calculated
    public void CustomerSpawner(List<int> spawnPositions) {
        for (int i = 0; i < placesToBeOccupied.Count; i++) {
            GameObject customer = Instantiate(customerPrefab, customerSpawnPoints[spawnPositions[i]].spawnPoint.transform.position, customerSpawnPoints[spawnPositions[i]].spawnPoint.transform.rotation);
            customer.GetComponent<Customer>().spawnedIndex = spawnPositions[i];
            customerSpawnPoints[spawnPositions[i]].isOccupied = true;
        }
    }
}
