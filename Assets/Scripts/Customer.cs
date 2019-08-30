using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private Spawner spawner;

    public List<string> allDemands = new List<string> { "A", "P" };
    public List<string> demands;
    public List<string> specialPickups = new List<string> { "speed", "time", "score" };


    public GameObject[] pickupDimension = new GameObject[3];

    public int numOfDemands;
    public int playerTypeSatisfied;
    public int spawnedIndex;

    public float waitTime;
    public float totalWaitTime;
    public float decrementRate;

    public bool satisfied;
    public bool stop;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.Find("CustomerSpawnPoints").GetComponent<Spawner>();
        totalWaitTime = waitTime = 60;
        decrementRate = 0.5f;
        numOfDemands = Random.Range(1, 4);

        for (int i = 0; i < numOfDemands; i++) {
            demands.Add(allDemands[Random.Range(0, allDemands.Count)]);
        }

        foreach (string s in demands) {
            GameObject needPrefab = Resources.Load<GameObject>("Prefabs/" + s + "Icon");
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
        if (satisfied || waitTime == 0) {
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
            yield return new WaitForSeconds(0.5f);
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
