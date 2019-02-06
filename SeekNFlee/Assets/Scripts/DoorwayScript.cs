using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision) {
        if(collision.transform.tag == "Agent") {
            Agent temp = collision.gameObject.GetComponent<Agent>();
            if(temp.agentType == Agent.AgentType.TRAVELLING) {
                WorldState.GetInstance().agents.Remove(temp);
                Destroy(temp.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Agent") {
            Agent temp = other.gameObject.GetComponent<Agent>();
            if (temp.agentType == Agent.AgentType.TRAVELLING) {
                GameManager.GetInstance().RespawnTraveller(other.GetComponent<Agent>());
            }
        }
    }
}
