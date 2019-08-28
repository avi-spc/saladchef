using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;
    public int playerType;
    public Queue pickUps;

    public bool chopping;
    // Start is called before the first frame update
    void Start() {
        pickUps = new Queue();
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

            if (Input.GetKeyDown(KeyCode.Q)) {
                Pick();
            }
        }
        
    }

    void Pick() {

    }

    void Drop() {
    }

    void Throw() {
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag.Equals("Veg")) {
            if (pickUps.Count < 2) {
                pickUps.Enqueue(col.gameObject.name);
            }

            Debug.Log(pickUps.Count);
        }

        if (col.gameObject.tag.Equals("ChopBoard")) {
            StartCoroutine(Chopping());
        }
    }

    IEnumerator Chopping() {
        Debug.Log("chopping");
        chopping = true;
        yield return new WaitForSeconds(2f);
        chopping = false;
    }
}
