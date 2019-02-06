using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    
    public AgentType agentType;

    public float maxVelocity;

    public SocialAgent socialAgent;
    public TravellingAgent travellingAgent;
    public WanderingAgent wanderingAgent;

    public Vector3 currentVelocity;

    // Use this for initialization
    void Start() {
        ChooseMaterial();
        socialAgent = GetComponent<SocialAgent>();
        travellingAgent = GetComponent<TravellingAgent>();
        wanderingAgent = GetComponent<WanderingAgent>();
    }

    // Update is called once per frame
    void Update() {
        if(GetComponent<Rigidbody>().velocity.magnitude > 10) {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        ChooseMaterial();
    }

    float agentTimer;
    public void TempWanderer() {
        StartCoroutine(AgentIsTempWanderer());
    }

    IEnumerator AgentIsTempWanderer() {
        agentType = AgentType.WANDERING;
        yield return new WaitForSeconds(Random.value * 5 + 5f);
        agentType = AgentType.SOCIAL;
        socialAgent.isSocial = true;
    }
    public void StandStill() {
        currentVelocity *= 0.4f;
        GetComponent<Rigidbody>().MovePosition(transform.position + maxVelocity * currentVelocity * Time.fixedDeltaTime);
    }

    public void ApplyDesiredVelocity(Vector3 desiredVelocity) {
        //Debug.Log("Current velocity:" + currentVelocity);
        Vector3 steering = Vector3.Normalize(desiredVelocity * 2 * Time.deltaTime + currentVelocity);
        currentVelocity = steering;
        //Debug.Log("Steering:" + steering);
        //GetComponent<Rigidbody>().velocity = (maxVelocity * steering);
        GetComponent<Rigidbody>().MovePosition(transform.position + maxVelocity * steering * Time.fixedDeltaTime);
    }

    public Vector3 Seek(Vector3 target) {
        return maxVelocity * Vector3.Normalize(target - transform.position);
    }

    public Vector3 NormalizeSeek(Vector3 target) {
        return Vector3.Normalize(target - transform.position);
    }

    public Vector3 NormalizeFlee(Vector3 target) {
        return Vector3.Normalize(transform.position - target);
    }

    public Vector3 Flee(Vector3 target) {
        return maxVelocity * Vector3.Normalize(transform.position - target);
    }

    private Vector3[] walls = new Vector3[4] {
        new Vector3(-14.5f, 1, 0),
        new Vector3(14.5f, 1, 0),
        new Vector3(0, 1, -9.5f),
        new Vector3(0, 1, 9.5f),
    };

    public Vector3 v;
    int AngleChange = 5;
    public Vector3 necessaryVel;
    public Vector3 AvoidWallsVel;
    // Not tested yet
    public void SetWander() {
        Vector3 circlePosition = Vector3.Normalize(currentVelocity);
        //Vector3.RotateTowards(circlePosition, )
        int turn = Random.Range(0, 36) * AngleChange;
        Quaternion rotation = Quaternion.Euler(0, turn, 0);
        Vector3 rotatedVector;
        if (Random.Range(0, 2) == 0) {
            rotatedVector = rotation * Vector3.right ;
        }
        else {
            rotatedVector = rotation * Vector3.right * -1 ;
        }
        //Debug.Log("Rotated Vector:" + rotatedVector);

        Vector3 print;

        AvoidWallsVel = Vector3.zero;
        necessaryVel = Vector3.zero;
        foreach (Obstacle o in WorldState.GetInstance().obstacles) {
            for (int i = 0; i < o.mesh.vertexCount; i++) {
                if (i % 2 == 0)
                    necessaryVel += Flee(o.mesh.vertices[i] + Vector3.up) / (1 * Vector3.Distance(o.mesh.vertices[i] + Vector3.up, transform.position));
            }
            for (int i = 0; i < o.mesh.vertexCount / 2 - 1; i++) {
                necessaryVel += Flee((o.mesh.vertices[(i + 1) * 2] - o.mesh.vertices[i * 2]) + Vector3.up) /
                       (10 * Vector3.Distance((o.mesh.vertices[(i + 1) * 2] - o.mesh.vertices[i * 2]) + Vector3.up, transform.position));
            }

        }
        for (int i = 0; i < walls.Length; i++) {

            print = Flee(walls[i] / (5 * Vector3.Distance(walls[i], transform.position)));
            print.y = 0;
            //Debug.Log("print vector:" + print);
            AvoidWallsVel -= print;
                 /// Vector3.Distance(walls[i], transform.position)
        }

        //AvoidWallsVel = Vector3.Normalize(AvoidWallsVel);
        //necessaryVel = Vector3.Normalize(necessaryVel);
        rotatedVector *= 2;
        //Debug.Log("Turn: " + turn + "  Rotated Vector: " + rotatedVector);
        v = (rotatedVector + 0.1f * necessaryVel + 0.3f * AvoidWallsVel);

    }

    public Vector3 Wander() {
        ApplyDesiredVelocity(v + currentVelocity);
        return v;
    }

    // Not tested
    public Vector3 Pursuit(Agent agent) {
        if (agent != null) {
            Vector3 v = agent.transform.position +
                        agent.currentVelocity * Vector3.Distance(agent.transform.position, transform.position) / maxVelocity; // / ((agent.maxVelocity + maxVelocity)/2)

            return Seek(v);
        }
        return Vector3.zero;
    }

    // Not tested
    public Vector3 Evade(Agent agent) {
        Vector3 v = agent.transform.position +
            agent.currentVelocity * Vector3.Distance(agent.transform.position, transform.position) / maxVelocity; //  / ((agent.maxVelocity + maxVelocity) / 2)
        return Flee(v);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "ExitDoorway" && agentType == AgentType.TRAVELLING) {
        Debug.Log("respawn 2");
            Debug.Log("Agent On Collision enter Exit doorway");
            GameManager.GetInstance().RespawnTraveller(this);
        }

    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "ExitDoorway" && agentType == AgentType.TRAVELLING) {
        Debug.Log("respawn 1");
            Debug.Log("Agent On trigger enter Exit doorway");
            GameManager.GetInstance().RespawnTraveller(this);
        }
    }

    void ChooseMaterial() {
        if (materials.Count == 3) {
            if (agentType == AgentType.TRAVELLING)
                GetComponent<Renderer>().material = materials[0];
            else if(agentType == AgentType.WANDERING)
                GetComponent<Renderer>().material = materials[1];
            else if (agentType == AgentType.SOCIAL)
                GetComponent<Renderer>().material = materials[2];
        }
    }

    private void OnDrawGizmos() {
        //Debug.Log("Current velocirt:" + Vector3.Normalize(agent.currentVelocity));
        //Debug.Log("desired Velocity:" + Vector3.Normalize(desiredVelocity));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + currentVelocity);
        //Debug.Log("Current velocirt:" + Vector3.Normalize(agent.currentVelocity));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + v);
        Gizmos.color = Color.green;
        //Debug.Log("necessaryVel:" + agent.necessaryVel);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.Normalize(necessaryVel)*2);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + AvoidWallsVel);
        //Debug.Log("desired Velocity:" + Vector3.Normalize(agent.v));
    }


    [SerializeField]
    private List<Material> materials;

    public enum AgentType {
        TRAVELLING,
        WANDERING,
        SOCIAL,
    }
}
