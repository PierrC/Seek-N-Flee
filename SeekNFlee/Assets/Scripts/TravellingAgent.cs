using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellingAgent : MonoBehaviour {

    private Agent agent;
    public int door;
    public Vector3 targetPosition;

    float cantReachDoorTimer;

    private void Awake() {
        targetPosition = transform.position;
        agent = GetComponent<Agent>();
        door = Random.Range(0, 2);
        cantReachDoorTimer = Time.time;
    }
    // Use this for initialization
    void Start() {
    }

    

    public void SetTargetPosition(Vector3 pTargetPosition) {
        targetPosition = pTargetPosition;
    }

    Vector3 desiredVelocity;

    void Update() {
        if (agent.agentType == Agent.AgentType.TRAVELLING) {
            desiredVelocity = CalculateDesiredVelocity();
            agent.ApplyDesiredVelocity(desiredVelocity);

        }

        if(Time.time - cantReachDoorTimer > 30) {
            SwitchDoor();
        }

    }

    void SwitchDoor() {
        door = (door + 1) % 2;
    }

    Vector3 CalculateDesiredVelocity() {
        // final goal
        Vector3 desiredVelocity = 2f * agent.Seek(targetPosition);

        Vector3 avoidAgents = Vector3.zero;
        foreach (Agent a in WorldState.GetInstance().agents) {
            //need to test function &might want to implement avaid for agents
            if (a != agent)
                avoidAgents += agent.Evade(a); // .transform.position) / Vector3.Distance(a.transform.position, transform.position);
        }

        Vector3 avoidObstacles = Vector3.zero;
        Vector3 AvoidWallsVel = Vector3.zero;
        Vector3 print = Vector3.zero;
        foreach (Obstacle o in WorldState.GetInstance().obstacles) {
            desiredVelocity += agent.Flee(o.transform.position) / (Vector3.Distance(o.transform.position, transform.position) - 1);
            for (int i = 0; i < o.mesh.vertexCount; i++) {
                if (i % 2 == 0)
                    avoidObstacles += agent.Flee(o.mesh.vertices[i] + Vector3.up) / (10 * Vector3.Distance(o.mesh.vertices[i] + Vector3.up, transform.position));
            }
            for (int i = 0; i < o.mesh.vertexCount / 2 - 1; i++) {
                avoidObstacles += agent.Flee((o.mesh.vertices[(i + 1) * 2] - o.mesh.vertices[i * 2]) + Vector3.up) /
                       (10 * Vector3.Distance((o.mesh.vertices[(i + 1) * 2] - o.mesh.vertices[i * 2]) + Vector3.up, transform.position));
            }
            for (int i = 0; i < walls.Length; i++) {

                print = agent.Flee(walls[i] / (20 * Vector3.Distance(walls[i], transform.position)));
                print.y = 0;
                //Debug.Log("print vector:" + print);
                AvoidWallsVel -= print;
                // Vector3.Distance(walls[i], transform.position)
            }


        }

        return desiredVelocity 
            + 0.5f * avoidObstacles + 0.1f * AvoidWallsVel + 0.1f * avoidAgents;
    }
    private Vector3[] walls = new Vector3[4] {
        new Vector3(-14.5f, 1, 0),
        new Vector3(14.5f, 1, 0),
        new Vector3(0, 1, -9.5f),
        new Vector3(0, 1, 9.5f),
    };

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Waypoint") {
            SetTargetPosition(other.gameObject.GetComponent<WayPoint>().nextPoint[door].gameObject.transform.position);
        }
    }

    //private void OnDrawGizmos() {
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.Normalize(desiredVelocity));
    //}
}
