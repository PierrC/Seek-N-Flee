using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : MonoBehaviour {

    private Agent agent;
    public int door;
    public Vector3 targetPosition;

    bool s;
    float time;
    private void Awake() {
        targetPosition = transform.position;

        agent = GetComponent<Agent>();
        door = Random.Range(0, 2);
        s = false; ;
        time = 0;
    }

    // Update is called once per frame
    void Update () {
        if (s) {

            agent.GetComponent<Rigidbody>().MovePosition(transform.position + new Vector3(2, 0, 0) * Time.deltaTime);
            agent.currentVelocity = new Vector3(1, 0, 0);
            if (Time.time - time > 4) {
                Debug.Log("switch");
                s = !s;
                time = Time.time;
            }
        }
        else {

            agent.GetComponent<Rigidbody>().MovePosition(transform.position + new Vector3(-2, 0, 0) * Time.deltaTime);
            agent.currentVelocity = new Vector3(-1, 0, 0);
            if (Time.time - time > 4) {
                Debug.Log("switch");
                s = !s;
                time = Time.time;
            }

        }
    }
}
