using UnityEngine;

public class Plate : MonoBehaviour
{
    public GameObject itemPlaced;

    void Start()
    {
        itemPlaced = null;
    }

    void Update()
    {
        
    }

    //Function to instantiate veggie prefab on plate when something is placed on it
    public void PlaceItem(string[] items) {
        GameObject vPlaced = Resources.Load<GameObject>("Prefabs/" + items[0]);
        itemPlaced = Instantiate(vPlaced, GameObject.Find(items[1]).transform.position, GameObject.Find(items[1]).transform.rotation);
        itemPlaced.name = items[0];

        itemPlaced.transform.SetParent(gameObject.transform);
    }
}
