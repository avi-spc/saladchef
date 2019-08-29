using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;
    public int playerType;
    public int pickUpsLen;
    public int totalItems;
    public int score;
    public int timer;

    public Vector2 rotationAngle;

    public Queue pickUps;
    public List<string> choppedItems;

    public GameObject pickedItemsCanvas;
    public GameObject choppedItemsCanvas;

    public GameObject vegPicked;
    public GameObject vegChopped;

    public string[] plateInteraction;

    public bool chopping;


    public GameObject firepoint;
    // Start is called before the first frame update
    void Start() {
        timer = 60;
        StartCoroutine(Timer());
        plateInteraction = new string[2];
        pickedItemsCanvas.SetActive(false);
        choppedItemsCanvas.SetActive(false);
        pickUps = new Queue();
        rotationAngle = Vector2.up;
    }

    private void FixedUpdate() {
        totalItems = pickUps.Count + choppedItems.Count;
    }
    // Update is called once per frame
    void Update() {
        
        if (playerType == 1) {
            if (!chopping) {
                if (Input.GetKey(KeyCode.W)) {
                    transform.Translate(Vector2.up * speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S)) {
                    transform.Translate(Vector2.down * speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.A)) {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D)) {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                }

                if (Input.GetKeyDown(KeyCode.Q)) {
                    Pick();
                }

                if (Input.GetKeyDown(KeyCode.E)) {
                    Drop();
                }
            }

        }
        else {
            if (Input.GetKey(KeyCode.UpArrow)) {
                transform.Translate(Vector2.up * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                transform.Translate(Vector2.down * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.Translate(Vector2.left * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                transform.Translate(Vector2.right * speed * Time.deltaTime);
            }

        }
            
    }

    void Pick() {
        Debug.DrawRay(firepoint.transform.position, rotationAngle, Color.blue, 10000);

        RaycastHit raycastHit;

        if (Physics.Raycast(firepoint.transform.position, rotationAngle , out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Veg")) {
                if (!pickedItemsCanvas.activeSelf)
                    pickedItemsCanvas.SetActive(true);

                if (pickUps.Count < 2) {
                    GameObject vPicked = Resources.Load<GameObject>("Prefabs/" + raycastHit.collider.gameObject.name + "Icon");
                    vegPicked = Instantiate(vPicked, transform.position, Quaternion.identity);
                    vegPicked.transform.SetParent(GameObject.Find("Picked").transform);

                    pickUps.Enqueue(raycastHit.collider.gameObject.name);
                    pickUpsLen = pickUps.Count;
                }
            }

            RaycastHit[] raycastHits = Physics.RaycastAll(firepoint.transform.position, rotationAngle, 10000f);

            foreach(var e in raycastHits) {
                if (e.collider.gameObject.tag.Equals("VegOnPlate")) {
                    if (!pickedItemsCanvas.activeSelf)
                        pickedItemsCanvas.SetActive(true);

                    if (pickUps.Count < 2) {
                        GameObject vPicked = Resources.Load<GameObject>("Prefabs/" + e.collider.gameObject.name + "Icon");
                        vegPicked = Instantiate(vPicked, transform.position, Quaternion.identity);
                        vegPicked.transform.SetParent(GameObject.Find("Picked").transform);

                        pickUps.Enqueue(e.collider.gameObject.name);
                        pickUpsLen = pickUps.Count;
                    }

                    Destroy(e.collider.gameObject);
                }
            }

            Debug.Log(pickUps.Count);
            foreach (var item in pickUps) {
                Debug.Log(item);
            }
        }
    }

    void Drop() {
        Debug.DrawRay(firepoint.transform.position, rotationAngle, Color.red, 10000);

        RaycastHit raycastHit;

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("ChopBoard")) {
                Debug.Log(raycastHit.collider.gameObject.tag);
                if (pickUps.Count > 0) {

                    StartCoroutine(Chopping(pickUps.Dequeue().ToString()));
                    if (pickUps.Count == 0) {
                        pickedItemsCanvas.SetActive(false);
                    }
                }
            }             
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Plate")) {
                if (raycastHit.collider.gameObject.GetComponent<Plate>().itemPlaced == null) {
                    Destroy(GameObject.Find("Picked").transform.GetChild(0).gameObject);
                    plateInteraction[0] = pickUps.Dequeue().ToString();
                    plateInteraction[1] = raycastHit.collider.gameObject.name;
                    raycastHit.collider.gameObject.SendMessage("PlaceItem", plateInteraction);
                    Debug.Log(pickUps.Count);

                    if (pickUps.Count == 0) {
                        pickedItemsCanvas.SetActive(false);
                    }
                }
            }
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Trash") && choppedItems.Count > 0) {                
                GameObject[] choppedItemsHUD = new GameObject[choppedItems.Count];
                for (int i = 0; i < choppedItems.Count; i++) {
                    Destroy(GameObject.Find("Chopped").transform.GetChild(i).gameObject);
                }

                score--;

                choppedItems.Clear();
                choppedItemsCanvas.SetActive(false);

            }
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Customer")) {
                if (raycastHit.collider.gameObject.GetComponent<Customer>().demands.Count == choppedItems.Count) {
                    raycastHit.collider.gameObject.GetComponent<Customer>().demands.Sort();
                    choppedItems.Sort();

                    int demandCounter = 0;

                    for (int i = 0; i < choppedItems.Count; i++) {
                        if (choppedItems[i] == raycastHit.collider.gameObject.GetComponent<Customer>().demands[i]) {
                            demandCounter++;
                        }
                        else {
                            Debug.Log("User not satisfied");
                            raycastHit.collider.gameObject.GetComponent<Customer>().decrementRate = 1;
                            break;
                        }
                    }

                    if (demandCounter == choppedItems.Count) {
                        Debug.Log("Satisfied");
                        raycastHit.collider.gameObject.GetComponent<Customer>().satisfied = true;
                        score++;
                        timer += 5;
                        GameObject[] choppedItemsHUD = new GameObject[choppedItems.Count];
                        for (int i = 0; i < choppedItems.Count; i++) {
                            Destroy(GameObject.Find("Chopped").transform.GetChild(i).gameObject);
                        }

                        choppedItems.Clear();
                        choppedItemsCanvas.SetActive(false);
                    }
                }
                else {
                    raycastHit.collider.gameObject.GetComponent<Customer>().decrementRate = 1;
                }

            }
        }

    }

    void Throw() {
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.name.Equals("Left")) {
            rotationAngle = Vector2.left;
            firepoint.transform.localPosition = new Vector2(-0.34f, 0);
        }

        if (col.gameObject.name.Equals("Right")) {
            rotationAngle = Vector2.right;
            firepoint.transform.localPosition = new Vector2(0.34f, 0);
        }

        if (col.gameObject.name.Equals("Down")) {
            rotationAngle = Vector2.down;
            firepoint.transform.localPosition = new Vector2(0, -0.34f);
        }

        if (col.gameObject.name.Equals("Up")) {
            rotationAngle = Vector2.up;
            firepoint.transform.localPosition = new Vector2(0, 0.34f);
        }
    }

    //private void OnCollisionStay2D(Collision2D col) {
    //    if (Input.GetKeyDown(KeyCode.C)) {
    //        if (col.gameObject.tag.Equals("ChopBoard")) {
    //            StartCoroutine(Chopping());
    //        }
    //    }
    //}


    IEnumerator Chopping(string item) {
        Debug.Log("chopping");
        chopping = true;
        Debug.Log(pickUps.Count);
        
        Destroy(GameObject.Find("Picked").transform.GetChild(0).gameObject);

        yield return new WaitForSeconds(2f);
        choppedItems.Add(item);
        if (choppedItems.Count == 1) {
            choppedItemsCanvas.SetActive(true);
        }

        GameObject cPicked = Resources.Load<GameObject>("Prefabs/" + item + "chopIcon");
        vegChopped = Instantiate(cPicked, transform.position, Quaternion.identity);
        vegChopped.transform.SetParent(GameObject.Find("Chopped").transform);

        chopping = false;
    }

    IEnumerator Timer() {
        while (timer > 0) {
            timer--;
            yield return new WaitForSeconds(1f);
        }
    }

    void FulfillCustomerDemand() {

    }
}
