using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    private Spawner spawner;

    public List<string> allDemands = new List<string>();
    public List<string> demands;
    public List<string> specialPickups = new List<string> { "speed", "time", "score" };

    public Image timer;

    public GameObject[] pickupDimension = new GameObject[3];

    public int numOfDemands;
    public int playerTypeSatisfied;
    public int spawnedIndex;
    public int penaltyToPlayer;
    public HashSet<int> playersWronglySatisfied = new HashSet<int>();

    public float waitTime;
    public float totalWaitTime;
    public float decrementRate;

    public bool satisfied;
    public bool stop;
    public bool isAngry;

    public PlayerController[] playerControllers;

    // Start is called before the first frame update
    void Start()
    {
        allDemands = new List<string> { "Guava", "Kiwi", "MuskMelon", "WaterMelon", "Pumpkin", "Orange" };
        playerControllers = FindObjectsOfType<PlayerController>();

        penaltyToPlayer = 1;

        spawner = GameObject.Find("CustomerSpawnPoints").GetComponent<Spawner>();
        decrementRate = 0.5f;
        numOfDemands = Random.Range(1, 4);

        totalWaitTime = waitTime = numOfDemands * 10 * 2;

        for (int i = 0; i < numOfDemands; i++) {
            demands.Add(allDemands[Random.Range(0, allDemands.Count)]);
        }

        foreach (string s in demands) {
            GameObject needPrefab = Resources.Load<GameObject>("Prefabs/" + s + "chopIcon");
            GameObject needObject = Instantiate(needPrefab, transform.position, Quaternion.identity);
            needObject.transform.SetParent(gameObject.transform.GetChild(0).GetChild(0));
        }

        for (int i = 0; i < pickupDimension.Length; i++) {
            pickupDimension[i] = GameObject.Find("Dimension").transform.GetChild(i).gameObject;
        }

        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 4.62f, transform.position.z), Random.Range(0.001f, 0.1f));
        }
        if (satisfied || waitTime <= 0) {
            spawner.customerSpawnPoints[spawnedIndex].isOccupied = false;
            transform.Translate(Vector3.up * Random.Range(2, 3) * Time.deltaTime);
            Destroy(gameObject, 2f);
        }

        if (transform.position.y == 4.62f)
            stop = true;
    }

    IEnumerator Wait() {
        while (waitTime > 0 && !satisfied) {
            waitTime -= decrementRate;
            timer.fillAmount = waitTime/totalWaitTime;
            yield return new WaitForSeconds(0.5f);
        }

        if (waitTime <= 0) {
            foreach(var pC in playerControllers) {
                if(pC.timer!=0)
                    pC.score -= penaltyToPlayer;
            }

            if (isAngry) {
                penaltyToPlayer = 2 * penaltyToPlayer;
                foreach (var pC in playerControllers) {
                    if (playersWronglySatisfied.Contains(pC.playerType)) {
                        pC.score -= penaltyToPlayer;
                    }
                }
            }
        }
    }

    void GeneratePickups() {
        if (waitTime > (70 * totalWaitTime) / 100) {
            int pT = Random.Range(0, specialPickups.Count-1);
            Debug.Log(specialPickups[pT]);
            GameObject pickUpPrefab = Resources.Load<GameObject>("Prefabs/" + specialPickups[pT]);
            GameObject pickUpObject = Instantiate(pickUpPrefab, new Vector3(Random.Range(pickupDimension[0].transform.position.x, pickupDimension[2].transform.position.x), Random.Range(pickupDimension[0].transform.position.y, pickupDimension[1].transform.position.y), 0), Quaternion.identity);
            pickUpObject.GetComponent<PowerPickups>().forPlayer = playerTypeSatisfied;
            pickUpObject.GetComponent<PowerPickups>().pickupType = specialPickups[pT];
        }
    }

}
