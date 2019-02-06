using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    static GameManager instance;

    [SerializeField]
    private GameObject ObstaclePrefab;
    [SerializeField]
    private GameObject AgentPrefab;

    public int numOfObstacles;
    public int numOfTravelers;
    public int numOfWanders;
    public int numOfSocials;

     int currentNumOfWanders;
     int currentNumOfSocials;
    int currentNumberOfTravellers;

    public List<GameObject> obstacles;
    public WayPointSystem wayPointSystem;



    private WorldState worldState;

    public static GameManager GetInstance() {
        return instance;
    }


	// Use this for initialization
	void Awake () {
        instance = this;

        worldState = GetComponent<WorldState>();

        wayPointSystem = GetComponent<WayPointSystem>();


        wayPointSystem.CreateWaypointGrid();
        Debug.Log("Creating obstacles");
        for (int i = 0; i < numOfObstacles; i++) {
            obstacles.Add(Instantiate(ObstaclePrefab, GetObstacleCoordinate(), new Quaternion()));
            obstacles[i].transform.SetParent(gameObject.transform);
            obstacles[i].GetComponent<Obstacle>().GenerateObstacle2(Random.Range(4, 17));
            worldState.obstacles.Add(obstacles[i].GetComponent<Obstacle>());
        }
        timer = 0;
        doorInt = 0;
        currentNumOfWanders = 0;
        currentNumOfSocials = 0;
        currentNumberOfTravellers = 0;
        canStart = true;
    }
    private void Start() {
        //GetComponent<GizmosHelps>().methodOne();
    }

    int doorInt;
    bool t;
    bool t2;
    float timer;
    bool worked;
    bool canStart;
    RaycastHit hit;
    // Update is called once per frame
    void Update () {
        
        if (!t && Time.time - timer > 1) {
            worked = UseWayPoint();
            t = true;
            //    t2 = true;
            timer = Time.time;
        }
        if (!worked && Time.time - timer > 1) {
            ClearLevel();

            wayPointSystem.CreateWaypointGrid();
            for (int i = 0; i < numOfObstacles; i++) {
                obstacles.Add(Instantiate(ObstaclePrefab, GetObstacleCoordinate(), new Quaternion()));
                obstacles[i].transform.SetParent(gameObject.transform);
                obstacles[i].GetComponent<Obstacle>().GenerateObstacle2(Random.Range(4, 17));
                worldState.obstacles.Add(obstacles[i].GetComponent<Obstacle>());
            }
            timer = Time.time;
            t = false;
        }
        if (worked && canStart) {
            // will try to spawn agent in empty space unitl all are intialized
            if (currentNumOfWanders < numOfWanders) {
                Vector3 pt = GetAgentCoordinate();

                // This is to prevent crashes if you ask for too many 
                //if (numOfWanders < 8) {
                //    do {
                //        Physics.SphereCast(pt + Vector3.up * 4, 0.3f, Vector3.down, out hit, 3f);
                //        if (hit.transform != null)
                //            Debug.Log("Hit:" + hit.transform.tag);
                //    } while (hit.transform != null);

                //}

                //do {
                Physics.SphereCast(pt + Vector3.up * 4, 0.3f, Vector3.down, out hit, 3f);
                //if (hit.transform != null)
                //    Debug.Log("Hit:" + hit.transform.tag);
                //} while (hit.transform != null);

                if (hit.transform == null) {

                    GameObject go = Instantiate(AgentPrefab, pt, new Quaternion());
                    WorldState.GetInstance().agents.Add(go.GetComponent<Agent>());
                    go.GetComponent<Agent>().maxVelocity = 1 + Random.value * 2;
                    go.GetComponent<Agent>().agentType = Agent.AgentType.WANDERING;
                    currentNumOfWanders++;
                }
            }

            //for (int i = 0; i < numOfSocials; i++) {
            if (currentNumOfSocials < numOfSocials) {
                    Vector3 pt2 = GetAgentCoordinate();

                //do {
                Physics.SphereCast(pt2 + Vector3.up * 4, 0.3f, Vector3.down, out hit, 3f);
                //    if (hit.transform != null)
                //        Debug.Log("Hit:" + hit.transform.tag);
                //} while (hit.transform != null);
                if (hit.transform == null) {

                    GameObject go = Instantiate(AgentPrefab, pt2, new Quaternion());
                    WorldState.GetInstance().agents.Add(go.GetComponent<Agent>());
                    go.GetComponent<Agent>().maxVelocity = 1 + Random.value * 2;
                    go.GetComponent<Agent>().agentType = Agent.AgentType.SOCIAL;
                    currentNumOfSocials++;
                }

            }
            if (currentNumOfSocials == numOfSocials &&
                currentNumOfWanders == numOfWanders) {
                foreach (Obstacle O in worldState.obstacles) {
                    if (Random.value < 0.5) {
                        O.GetComponent<MeshCollider>().convex = false;
                    }
                }

            }
            //timer = Time.time;
        }
        if (worked && Time.time - timer > 2.0f) {
            Debug.Log("Instatiating travelling agents");
            t = true;

            // //Instatiate travelling agents
            if (currentNumberOfTravellers < numOfTravelers) {
                GameObject go = Instantiate(AgentPrefab, worldState.startDoor.transform.position, new Quaternion());
                WorldState.GetInstance().agents.Add(go.GetComponent<Agent>());
                go.GetComponent<Agent>().maxVelocity = 1 + Random.value * 2;
                go.GetComponent<Agent>().agentType = Agent.AgentType.TRAVELLING;

                Vector3 target = worldState.startDoor.GetComponentInChildren<WayPoint>().nextPoint[go.GetComponent<TravellingAgent>().door].transform.position;
                go.GetComponent<TravellingAgent>().SetTargetPosition(target);
                currentNumberOfTravellers++;

            }
            timer = Time.time;
            //t2 = false;

        }


    }
    int index;

 //   public List<GameObject> obstacles;
 //   public WayPointSystem wayPointSystem;
    void ClearLevel() {
        while(obstacles.Count != 0) {
            Destroy(obstacles[0]);
            obstacles.RemoveAt(0);
        }
        worldState.obstacles.Clear();
        wayPointSystem.ClearWaypoints();
        // Clear waypoint system
    }

    private bool UseWayPoint() {
        return wayPointSystem.CreatePaths();
    }

    private Vector3 GetObstacleCoordinate() {
        return new Vector3(Random.Range(-11, 12), 0, Random.Range(-8, 9));
    }
    private Vector3 GetAgentCoordinate() {
        Vector3 v = new Vector3(Random.value * 13, 1, Random.value * 9);
        if (Random.value < 0.5)
            v.x *= -1;
        if (Random.value < 0.5)
            v.z *= -1;

        return v;
    }

    public void RespawnTraveller(Agent a) {
        if (a.agentType != Agent.AgentType.TRAVELLING)
            return;
        a.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        a.currentVelocity = Vector3.zero;
        a.necessaryVel = Vector3.zero;
        a.maxVelocity = 1 + Random.value * 2;
        // a.GetComponent<Rigidbody>().MovePosition(worldState.startDoor.transform.position);
        a.gameObject.transform.position = worldState.startDoor.transform.position;
        Vector3 target = worldState.startDoor.GetComponentInChildren<WayPoint>().nextPoint[a.GetComponent<TravellingAgent>().door].transform.position;
        a.GetComponent<TravellingAgent>().SetTargetPosition(target);


    }

    public GameObject travellingAgentPrefab;
}
