using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject UIDisplay;

    [SerializeField]
    private int speed;
    public int playerType;
    int pickUpsLen;
    int totalItems;
    public int score;
    public int timer;

    private Vector2 rotationAngle;

    private Queue pickUps;
    public List<string> choppedItems;

    public GameObject pickedItemsCanvas;
    public GameObject choppedItemsCanvas;

    GameObject vegPicked;
    GameObject vegChopped;

    public GameObject[] moveBounds = new GameObject[3];

    private string[] plateInteraction;

    bool chopping;

    private Animator animator;

    public GameObject firepoint;
    public GameObject chopProgress;

    void Start() {
        animator = GetComponent<Animator>();
        timer = 60;
        StartCoroutine(Timer());        //Timer drops down as soon as game starts
        plateInteraction = new string[2];
        pickedItemsCanvas.SetActive(false);
        choppedItemsCanvas.SetActive(false);
        pickUps = new Queue();
        rotationAngle = Vector2.up;
        chopProgress.SetActive(false);
    }

    private void FixedUpdate() {
        //totalItems = pickUps.Count + choppedItems.Count;
    }

    void Update() {        
        if (playerType == 1 && timer > -1) {        //Controls for player_1
            if (!chopping) {        //player can't move when chopping is in progress
                if (Input.GetKey(KeyCode.W) && transform.position.y < moveBounds[0].transform.position.y) {
                    transform.Translate(Vector2.up * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 0);      //Setting animaetion trigger for different directions
                }
                else if (Input.GetKey(KeyCode.S) && transform.position.y > moveBounds[1].transform.position.y) {
                    transform.Translate(Vector2.down * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 1);
                }
                else if (Input.GetKey(KeyCode.A) && transform.position.x > moveBounds[0].transform.position.x) {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 2);
                }
                else if (Input.GetKey(KeyCode.D) && transform.position.x < moveBounds[2].transform.position.x) {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 3);
                }
                else {
                    animator.SetInteger("F 1", -1);     //Setting default animation direction
                }

                if (Input.GetKeyDown(KeyCode.Q)) {
                    Pick();     //Function to pick veggies 
                }

                if (Input.GetKeyDown(KeyCode.E)) {
                    Drop();     //Function to drop veggies
                }
            }

            UIDisplay.GetComponent<UIDisplay>().player1_Timer.text = "Time left: " + timer.ToString();
            UIDisplay.GetComponent<UIDisplay>().player1_Score.text = "Score: " + score.ToString();
        }
        else if(playerType == 2 && timer > -1) {        //controls for player_2
            if (!chopping) {        //player can't move when chopping is in progress
                if (Input.GetKey(KeyCode.UpArrow) && transform.position.y < moveBounds[0].transform.position.y) {
                    transform.Translate(Vector2.up * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 0);      //Setting animaetion trigger for different directions

                }
                else if (Input.GetKey(KeyCode.DownArrow) && transform.position.y > moveBounds[1].transform.position.y) {
                    transform.Translate(Vector2.down * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 1);

                }
                else if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x > moveBounds[0].transform.position.x) {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 2);

                }
                else if (Input.GetKey(KeyCode.RightArrow) && transform.position.x < moveBounds[2].transform.position.x) {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                    animator.SetInteger("F 1", 3);
                }
                else {
                    animator.SetInteger("F 1", -1);     //Setting default animation direction
                }

                if (Input.GetKeyDown(KeyCode.Keypad0)) {
                    Pick();     //Function to pick veggies
                }

                if (Input.GetKeyDown(KeyCode.Keypad1)) {
                    Drop();     //Function to drop veggies 
                }

            }
            UIDisplay.GetComponent<UIDisplay>().player2_Timer.text = "Time left: " + timer.ToString();
            UIDisplay.GetComponent<UIDisplay>().player2_Score.text = "Score: " + score.ToString();
        }


    }

    void Pick() {
        RaycastHit raycastHit;

        if (Physics.Raycast(firepoint.transform.position, rotationAngle , out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Veg")) {
                PickVeggies(raycastHit.collider.gameObject);        //Function to pick veggies from source on key press-Q
            }

            PickItemFromPlate();        //Function to pick veggie from plates on key press-Q
           
        }
    }

    void Drop() {
        RaycastHit raycastHit;

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("ChopBoard")) {
                StartChopping();        //Function to chop veggies when near chopping board on key press-E 
            }             
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Plate")) {
                PlaceItemOnPlate(raycastHit.collider.gameObject);       //Function to place veggies on plate on key press-E
            }
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Trash") && choppedItems.Count > 0) {
                ThrowInTrash();     //Function to throw chopped veggies in trashcan on key press-E
            }
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Customer")) {
                SatisfyCustomer(raycastHit.collider.gameObject);        //Function to satisfy customer demands when standing near a particular customer on key press-E
            }
        }

    }

    private void OnTriggerEnter(Collider col) {

        string direction = col.gameObject.name;

        //Setting the direction of point of raycaster in which direction should the ray be casted to take different actions like picking veggies, satisfying customer etc.
        switch (direction) {
            case "Left":
                rotationAngle = Vector2.left;       //for picking vegggies
                firepoint.transform.localPosition = new Vector2(-0.2f, 0);
                break;
            case "Right":
                rotationAngle = Vector2.right;      //for picking veggies
                firepoint.transform.localPosition = new Vector2(0.2f, 0);
                break;
            case "Down":
                rotationAngle = Vector2.down;       //for placing item on plate, chopping, and throwing in trashcan
                firepoint.transform.localPosition = new Vector2(0, -0.2f);
                break;
            case "Up":
                rotationAngle = Vector2.up;         //for satisfying customer
                firepoint.transform.localPosition = new Vector2(0, 0.2f);
                break;

        }


        //Logic related to special pickups such as speed, time, score when satisfying customer before 70% of wait time
        if (col.gameObject.tag.Equals("Pickups")) {
            if (col.gameObject.GetComponent<PowerPickups>().forPlayer == playerType) {
                if (col.gameObject.GetComponent<PowerPickups>().pickupType == "speed") {
                    speed *= 2;
                    Invoke("NormalizeSpeed", 10f);
                    Destroy(col.gameObject);
                }
                else if (col.gameObject.GetComponent<PowerPickups>().pickupType == "time") {
                    timer += 20;
                    Destroy(col.gameObject);
                }
                else if (col.gameObject.GetComponent<PowerPickups>().pickupType == "score") {
                    score += 3;
                    Destroy(col.gameObject);
                }
            }
        }
    }


    //Function to normalize speed after 10 seconds of grabbing speed special pickup
    public void NormalizeSpeed() {
        speed = 4;
    }

    //Enumerator to process chopping for 2 seconds 
    IEnumerator Chop(string item) {
        chopping = true;
        
        Destroy(pickedItemsCanvas.transform.GetChild(0).gameObject);

        chopProgress.SetActive(true);

        yield return new WaitForSeconds(2f);
        choppedItems.Add(item);
        if (choppedItems.Count == 1) {
            choppedItemsCanvas.SetActive(true);
        }

        GameObject cPicked = Resources.Load<GameObject>("Prefabs/" + item + "chopIcon");
        vegChopped = Instantiate(cPicked, transform.position, Quaternion.identity);
        vegChopped.transform.SetParent(choppedItemsCanvas.transform);

        chopProgress.SetActive(false);

        chopping = false;
    }

    IEnumerator Timer() {
        while (timer > 0) {
            timer--;
            yield return new WaitForSeconds(1f);
        }
    }

    void PickVeggies(GameObject vegetable) {
        if (!pickedItemsCanvas.activeSelf)
            pickedItemsCanvas.SetActive(true);

        if (pickUps.Count < 2) {
            GameObject vPicked = Resources.Load<GameObject>("Prefabs/" + vegetable.name + "Icon");      //Instantiating prefabs for HUD directly from resources directory
            vegPicked = Instantiate(vPicked, transform.position, Quaternion.identity);
            vegPicked.transform.SetParent(pickedItemsCanvas.transform);

            pickUps.Enqueue(vegetable.name);
            pickUpsLen = pickUps.Count;
        }
    }

    void StartChopping() {
        if (pickUps.Count > 0) {
            StartCoroutine(Chop(pickUps.Dequeue().ToString()));
            if (pickUps.Count == 0) {
                pickedItemsCanvas.SetActive(false);
            }
        }
    }

    void SatisfyCustomer(GameObject customer) {
        if (customer.GetComponent<Customer>().demands.Count == choppedItems.Count) {
            customer.GetComponent<Customer>().demands.Sort();
            choppedItems.Sort();

            int demandCounter = 0;

            for (int i = 0; i < choppedItems.Count; i++) {
                if (choppedItems[i] == customer.GetComponent<Customer>().demands[i]) {
                    demandCounter++;
                }
                else {
                    customer.GetComponent<Customer>().decrementRate = 1;
                    customer.GetComponent<Customer>().isAngry = true;
                    customer.GetComponent<Customer>().playersWronglySatisfied.Add(playerType);

                    break;
                }
            }

            if (demandCounter == choppedItems.Count) {
                customer.GetComponent<Customer>().satisfied = true;
                customer.GetComponent<Customer>().playerTypeSatisfied = playerType;
                customer.SendMessage("GeneratePickups");
                score += choppedItems.Count * 5;
                timer += 5;
                GameObject[] choppedItemsHUD = new GameObject[choppedItems.Count];
                for (int i = 0; i < choppedItems.Count; i++) {
                    Destroy(choppedItemsCanvas.transform.GetChild(i).gameObject);
                }

                choppedItems.Clear();
                choppedItemsCanvas.SetActive(false);
            }
        }
        else {
            customer.GetComponent<Customer>().decrementRate = 1;
            customer.GetComponent<Customer>().isAngry = true;
            customer.GetComponent<Customer>().playersWronglySatisfied.Add(playerType);
        }
    }

    void PlaceItemOnPlate(GameObject plate) {
        if (plate.GetComponent<Plate>().itemPlaced == null) {
            Destroy(pickedItemsCanvas.transform.GetChild(0).gameObject);
            plateInteraction[0] = pickUps.Dequeue().ToString();
            plateInteraction[1] = plate.name;
            plate.SendMessage("PlaceItem", plateInteraction);

            if (pickUps.Count == 0) {
                pickedItemsCanvas.SetActive(false);
            }
        }
    }

    void ThrowInTrash() {
        GameObject[] choppedItemsHUD = new GameObject[choppedItems.Count];
        for (int i = 0; i < choppedItems.Count; i++) {
            Destroy(choppedItemsCanvas.transform.GetChild(i).gameObject);
        }

        score--;

        choppedItems.Clear();
        choppedItemsCanvas.SetActive(false);
    }

    void PickItemFromPlate() {
        RaycastHit[] raycastHits = Physics.RaycastAll(firepoint.transform.position, rotationAngle, 10000f);

        foreach (var e in raycastHits) {
            if (e.collider.gameObject.tag.Equals("VegOnPlate")) {
                if (!pickedItemsCanvas.activeSelf)
                    pickedItemsCanvas.SetActive(true);

                if (pickUps.Count < 2) {
                    GameObject vPicked = Resources.Load<GameObject>("Prefabs/" + e.collider.gameObject.name + "Icon");      //spawning chopped veggie icon at player's HUD
                    vegPicked = Instantiate(vPicked, transform.position, Quaternion.identity);
                    vegPicked.transform.SetParent(pickedItemsCanvas.transform);

                    pickUps.Enqueue(e.collider.gameObject.name);
                    pickUpsLen = pickUps.Count;
                }

                Destroy(e.collider.gameObject);
            }
        }
    }
}