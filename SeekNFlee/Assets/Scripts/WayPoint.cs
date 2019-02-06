using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WayPoint : MonoBehaviour {

    public float[] heuristic;
    public List<WayPoint> neighbours;
    public WayPoint[] nextPoint;
    private void Awake() {
        heuristic = new float[2] { 1000f, 1000f };
        neighbours = new List<WayPoint>();
        gameObject.layer = 2;
        nextPoint = new WayPoint[2];


    }
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnTriggerStay(Collider other) {
     //   Debug.Log("Trigger with " + other.tag);
        if (other.tag == "Obstacle") {
            WayPointSystem.pointSystem.waypoints.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision) {
        Debug.Log("collision with " + collision.gameObject.tag);
    }

    private void OnDrawGizmos() {

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>().radius);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        //if (heuristic[0] != 1000f)
        //    Handles.Label(gameObject.transform.position, "0", style);
        //if (heuristic[1] != 1000f)
        //    Handles.Label(gameObject.transform.position + new Vector3(0.2f, 0, 0), "1", style);

        if (nextPoint != null && nextPoint.Length == 2) {
            if (nextPoint[0] != null) {
                Gizmos.DrawLine(transform.position, nextPoint[0].transform.position);
            }
            Gizmos.color = Color.cyan;
            if (nextPoint[1] != null) {
                Gizmos.DrawLine(transform.position, nextPoint[1].transform.position);
            }
        }
    }
}
