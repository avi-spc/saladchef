using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public GameObject itemPlaced;

    // Start is called before the first frame update
    void Start()
    {
        itemPlaced = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceItem(string[] items) {
        Debug.Log(items[0]);
        GameObject vPlaced = Resources.Load<GameObject>("Prefabs/" + items[0]);
        itemPlaced = Instantiate(vPlaced, GameObject.Find(items[1]).transform.position, GameObject.Find(items[1]).transform.rotation);
        itemPlaced.name = items[0];
        //itemPlaced.transform.SetParent(GameObject.Find("Chopped").transform);

        itemPlaced.transform.SetParent(gameObject.transform);
    }
}
