using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    private Spawner spawner;

    List<string> allDemands = new List<string>();
    public List<string> demands;
    List<string> specialPickups = new List<string> { "speed", "time", "score" };

    public Image timer;

    public GameObject[] pickupDimension = new GameObject[3];

    int numOfDemands;
    public int playerTypeSatisfied;
    public int spawnedIndex;
    int penaltyToPlayer;
    public HashSet<int> playersWronglySatisfied = new HashSet<int>();

    float waitTime;
    float totalWaitTime;
    public float decrementRate;

    public bool satisfied;
    bool stop;
    public bool isAngry;

    PlayerController[] playerControllers;

    void Start()
    {
        allDemands = new List<string> { "Guava", "Kiwi", "MuskMelon", "WaterMelon", "Pumpkin", "Orange" };
        playerControllers = FindObjectsOfType<PlayerController>();

        penaltyToPlayer = 1;

        spawner = GameObject.Find("CustomerSpawnPoints").GetComponent<Spawner>();
        decrementRate = 0.5f;
        numOfDemands = Random.Range(1, 4);

        totalWaitTime = waitTime = numOfDemands * 15;

        for (int i = 0; i < numOfDemands; i++) {
            demands.Add(allDemands[Random.Range(0, allDemands.Count)]);
        }

        //spawning chooped veggie icon at customer's HUD
        foreach (string s in demands) {
            GameObject needPrefab = Resources.Load<GameObject>("Prefabs/" + s + "chopIcon");
            GameObject needObject = Instantiate(needPrefab, transform.position, Quaternion.identity);
            needObject.transform.SetParent(gameObject.transform.GetChild(0).GetChild(0));
        }

        //Defining the area bounds where special pickups has to be spawned
        for (int i = 0; i < pickupDimension.Length; i++) {
            pickupDimension[i] = GameObject.Find("Dimension").transform.GetChild(i).gameObject;
        }

        StartCoroutine(Wait());
    }

    void Update()
    {
        if (!stop) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 4.62f, transform.position.z), Random.Range(0.001f, 0.1f));
        }
        if (satisfied || waitTime <= 0) {       //customer gameObject destroyed if satisfied or wait time reaches 0
            spawner.customerSpawnPoints[spawnedIndex].isOccupied = false;
            transform.Translate(Vector3.up * Random.Range(2, 3) * Time.deltaTime);
            Destroy(gameObject, 2f);
        }

        if (transform.position.y == 4.62f)
            stop = true;
    }

    //Calculates waiting time for the customer
    IEnumerator Wait() {
        while (waitTime > 0 && !satisfied) {
            waitTime -= decrementRate;
            timer.fillAmount = waitTime/totalWaitTime;
            yield return new WaitForSeconds(0.5f);
        }

        //Penalize player when wait time reaches 0 and customer is not satisfied yet
        if (waitTime <= 0) {
            foreach(var pC in playerControllers) {
                if(pC.timer!=0)
                    pC.score -= penaltyToPlayer;
            }

            //Penalty is doubled if customer is given wrong combination of veggies
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

    //Special pickups are spawned at random locations if customer is satisfied before wait time falls below 70% of total wait time
    void GeneratePickups() {
        if (waitTime > (70 * totalWaitTime) / 100) {
            int pT = Random.Range(0, specialPickups.Count-1);
            GameObject pickUpPrefab = Resources.Load<GameObject>("Prefabs/" + specialPickups[pT]);
            GameObject pickUpObject = Instantiate(pickUpPrefab, new Vector3(Random.Range(pickupDimension[0].transform.position.x, pickupDimension[2].transform.position.x), Random.Range(pickupDimension[0].transform.position.y, pickupDimension[1].transform.position.y), 0), Quaternion.identity);
            pickUpObject.GetComponent<PowerPickups>().forPlayer = playerTypeSatisfied;
            pickUpObject.GetComponent<PowerPickups>().pickupType = specialPickups[pT];
        }
    }

}
