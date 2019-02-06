using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour {

    private static WorldState instance;

    public static WorldState GetInstance() {
        return instance;
    }

    public GameObject startDoor;
    public List<GameObject> leavingDoors;
    public List<Obstacle> obstacles;
    public List<Agent> agents;

    public Dictionary<Vector3, List<SocialAgent>> socialCircles;
    private void Awake() {
        socialCircles = new Dictionary<Vector3, List<SocialAgent>>();
        instance = this;
    }
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (socialCircles != null) {
            List<Vector3> lists = new List<Vector3>(socialCircles.Keys);
            foreach (Vector3 v in lists) {
                if (socialCircles[v].Count == 0) {
                    socialCircles.Remove(v);
                }
            }

        }
	}

    private void OnDrawGizmos() {
        //foreach(Vector3 v in socialCircles.Keys) {
        //    Gizmos.color = Color.grey;
        //    Gizmos.DrawWireSphere(v, 0.5f);
        //}
    }

}
