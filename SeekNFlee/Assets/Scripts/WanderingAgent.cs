using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAgent : MonoBehaviour {

    private Agent agent;

    public Vector3 targetPosition;

    // Use this for initialization
    void Start () {
        targetPosition = transform.position;
        agent = GetComponent<Agent>();
        wander = false;
        time = Time.time;
        pursueTime = Time.time;
        pursueAgent = false;
    }

    float time;
    float pursueTime;
    bool wander;
    Vector3 m;
    // Update is called once per frame
    void Update () {
        if(agent.agentType == Agent.AgentType.WANDERING) {
            WanderLogic();
        }
    }

    public void WanderLogic() {
        //Debug.Log("Pursuing Agent:" + pursueAgent);
        if (!pursueAgent) {
            //Debug.Log("wandering:" + wander);
            if (!wander) {
                agent.SetWander();
                wander = true;
                time = Time.time;
            }

            if (wander && Time.time - time < 2) {
                agent.Wander();
            }
            else {
                wander = !wander;
            }
            if (Time.time - pursueTime > 5 ) {
                if (SetPursuitAgent()) {
                    time = Time.time;
                    pursueAgent = true;
                }
            }

        }
        else {
            if (Vector3.Distance(transform.position, agentToPursue.transform.position) < 4) {
                m = agent.Pursuit(agentToPursue);
                agent.ApplyDesiredVelocity(m);
            }
            else {
                agentToPursue = null;
                pursueAgent = false;
                pursueTime = Time.time;
            }
        }
    }

    public float viewDistance;
    bool pursueAgent;
    Agent agentToPursue;

    private bool SetPursuitAgent() {
        Agent a = FindClosetAgent();
        if (a != null) {
            if (Vector3.Distance(a.transform.position, transform.position) < viewDistance) {
                agentToPursue = a;
                return true;
            }
        }
        return false;
    }

    private Agent FindClosetAgent() {
        float smallestDistance = 100; int index = -1;
        List<Agent> a = WorldState.GetInstance().agents;
        for (int i = 0; i < a.Count; i++) { 
        //foreach(Agent a in WorldState.GetInstance().agents) {
            if(a[i].agentType == Agent.AgentType.TRAVELLING) {
                if(smallestDistance > Vector3.Distance(a[i].transform.position, transform.position)) {
                    index = i;
                    smallestDistance = Vector3.Distance(a[i].transform.position, transform.position);
                }
            }
        }
        if (index == -1)
            return null;
        else
            return a[index];
    }


    //private void OnDrawGizmos() {
        //Gizmos.color = Color.black;
        //Gizmos.DrawSphere(transform.position + m, 0.2f);
    //}

}
