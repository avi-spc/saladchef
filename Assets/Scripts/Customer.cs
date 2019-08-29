using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public List<string> allDemands = new List<string>{ "A", "P" };

    public List<string> demands;

    public int numOfDemands;
    public float waitTime;
    public float decrementRate;

    public bool satisfied;
    // Start is called before the first frame update
    void Start()
    {
        waitTime = 60;
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

        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Wait() {
        while (waitTime > 0 && !satisfied) {
            waitTime -= decrementRate;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
