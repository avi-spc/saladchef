using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;
    public int playerType;
    public int pickUpsLen;

    public Vector2 rotationAngle;

    public Queue pickUps;

    public bool chopping;


    public GameObject firepoint;
    // Start is called before the first frame update
    void Start() {
        pickUps = new Queue();
        rotationAngle = Vector2.up;
    }

    private void FixedUpdate() {
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
            if(raycastHit.collider.gameObject.tag.Equals("Veg"))
                if (pickUps.Count < 2) {
                    pickUps.Enqueue(raycastHit.collider.gameObject.name);
                    pickUpsLen = pickUps.Count;
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
                if(pickUps.Count > 0)
                    StartCoroutine(Chopping());
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


    IEnumerator Chopping() {
        Debug.Log("chopping");
        chopping = true;
        yield return new WaitForSeconds(2f);
        chopping = false;
    }
}
