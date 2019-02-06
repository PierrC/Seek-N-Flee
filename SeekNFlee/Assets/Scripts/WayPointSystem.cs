

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WayPointSystem : MonoBehaviour {

    public static WayPointSystem pointSystem;

    public GameObject waypointPrefab;

    public WayPoint StartPoint;
    public WayPoint[] endPoints;

    public List<GameObject> waypoints;
    public List<WayPoint> paths;
	// Use this for initialization
	void Start () {
        pointSystem = this;
        //CreateWaypointGrid();
        //CreatePaths();
    }

    public void ClearWaypoints() {
        //Debug.Log("Clearing waypoint grid");
        if (StartPoint == null)
            StartPoint = Instantiate(waypointPrefab, new Vector3(14.5f, 1.5f, 0), new Quaternion()).GetComponent<WayPoint>();
        if (endPoints[0] == null)
            endPoints[0] = Instantiate(waypointPrefab, new Vector3(-14.5f, 1.5f, 5), new Quaternion()).GetComponent<WayPoint>();
        if (endPoints[1] == null)
            endPoints[1] = Instantiate(waypointPrefab, new Vector3(-14.5f, 1.5f, -5), new Quaternion()).GetComponent<WayPoint>();
        waypoints.Remove(StartPoint.gameObject);
        paths.Remove(StartPoint);
        while (waypoints.Count != 0){
            Destroy(waypoints[0]);
            waypoints.RemoveAt(0);
        }
        while (paths.Count != 0) {
            //Destroy(paths[0].gameObject);
            //paths.RemoveAt(0);
            paths.Clear();
        }
    }

    public void CreateWaypointGrid() {
        //Debug.Log("Creating waypoint grid");
        if (StartPoint == null)
            StartPoint = Instantiate(waypointPrefab, new Vector3(14.5f, 1.5f, 0), new Quaternion()).GetComponent<WayPoint>();
        if (endPoints[0] == null)
            endPoints[0] = Instantiate(waypointPrefab, new Vector3(-14.5f, 1.5f, 5), new Quaternion()).GetComponent<WayPoint>();
        if (endPoints[1] == null)
            endPoints[1] = Instantiate(waypointPrefab, new Vector3(-14.5f, 1.5f, -5), new Quaternion()).GetComponent<WayPoint>();

        for (int i = -14; i < 14; i++) {
            for(int j = -9; j <= 9; j++) {
                waypoints.Add(Instantiate(waypointPrefab, new Vector3(i, 1.5f, j), new Quaternion()));
            }
        }
        //Debug.Log("Adding start waypoint");
        waypoints.Add(StartPoint.gameObject);
        //Debug.Log("Created waypoint grid");
    }

    public bool CreatePaths() {
        //Debug.Log("Starting Path method");
        while (endPoints[0].heuristic.Length != 2 &&
            endPoints[1].heuristic.Length != 2) {
            Debug.Log("endPoints[0].heuristic.Length: " + endPoints[0].heuristic.Length);
            Debug.Log("endPoints[1].heuristic.Length: " + endPoints[1].heuristic.Length);

        }
        for(int i = 0; i < 2; i++) {
            if (endPoints[i] == null)
                return false;
            endPoints[i].GetComponent<WayPoint>().heuristic[0] = 0;
            endPoints[i].GetComponent<WayPoint>().heuristic[1] = 0;
        }
        // Connect all possible to end loop
        for (int i = 1; i >= 0; i--) {
            foreach (GameObject w in waypoints) {
                WaypointAreConnected(endPoints[i].gameObject, w, i);
            }
        }

        //while (StartPoint.heuristic[0] == 0 && StartPoint.heuristic[1] == 0) {
        //    for (int i = 1; i >= 0; i--) {
        //        foreach (GameObject w in waypoints) {
        //            WaypointAreConnected(endPoints[i].gameObject, w, i);
        //        }
        //    }
        //}

        // other connections
        List<GameObject> w3 = waypoints;
        for (int i = 0; i < 2; i++) {
            Debug.Log("Path:" + i);
            for (int a = 0; a < waypoints.Count; a++) {

                for (int b = a + 1; b < waypoints.Count; b++) {
                    WaypointAreConnected(waypoints[a], waypoints[b], i);

                }
            }
        }
        
        paths = NodesRoutes(StartPoint);
        
        foreach (GameObject wp in waypoints) {
            if (!paths.Contains(wp.GetComponent<WayPoint>())) {
                //waypoints.Remove(wp.gameObject);
                Destroy(wp.gameObject);
            }
        }
        //return true;

        //foreach (GameObject w in waypoints) {
        //    WaypointAreConnected(endPoints[i].gameObject, w, i);
        //}
        if (StartPoint.nextPoint[0] == null ||
            StartPoint.nextPoint[1] == null) {
            Debug.Log("path does not exists");
            return false;
        }
        Debug.Log("path does exists");
        return true;
    }

    List<WayPoint> NodesRoutes(WayPoint wayPoint) {
        if (wayPoint == null) {
            return new List<WayPoint>() { wayPoint };
        }
        if (wayPoint.nextPoint[0] == null && wayPoint.nextPoint[1] == null)
            return new List<WayPoint>() { wayPoint };
        List<WayPoint> l = NodesRoutes(wayPoint.nextPoint[0]);
        l.AddRange(NodesRoutes(wayPoint.nextPoint[1]));
        l.Add(wayPoint);
        return l;
    }

    public GameObject wrongSayer;
    public LayerMask layer;
    RaycastHit hit;
    void WaypointAreConnected(GameObject start, GameObject endPoint, int door) {
        if (start == endPoint)
            return;
        
        Physics.SphereCast(start.transform.position, 0.3f, Vector3.Normalize(endPoint.transform.position - start.transform.position),
            out hit, Vector3.Distance(endPoint.transform.position, start.transform.position));

        //Physics.Raycast(start.transform.position, Vector3.Normalize(endPoint.transform.position - start.transform.position), 
        //    out hit, Vector3.Distance(endPoint.transform.position , start.transform.position), layer);

        if (hit.transform == null) { 
            start.GetComponent<WayPoint>().neighbours.Add(endPoint.GetComponent<WayPoint>());
            endPoint.GetComponent<WayPoint>().neighbours.Add(start.GetComponent<WayPoint>());
            if (endPoint.GetComponent<WayPoint>().heuristic != null) {
                // Catch the motherfucker whos fucking up my code
                if (start.GetComponent<WayPoint>().heuristic.Length == 0) {
                    wrongSayer = start;
                }
                float heuritic = start.GetComponent<WayPoint>().heuristic[door] + Vector3.Distance(start.transform.position, endPoint.transform.position);
                if(heuritic < endPoint.GetComponent<WayPoint>().heuristic[door]) {
                    endPoint.GetComponent<WayPoint>().heuristic[door] = heuritic;
                    endPoint.GetComponent<WayPoint>().nextPoint[door] = start.GetComponent<WayPoint>();
                }
                //endPoint.GetComponent<WayPoint>().heuristic[door] = start.GetComponent<WayPoint>().heuristic[door] + Vector3.Distance(start.transform.position, endPoint.transform.position);
            }
        }
    }

	// Update is called once per frame
	void Update () {

    }

    private void OnDrawGizmos() {
        //Gizmos.color = Color.green;
        //foreach (WayPoint wp in paths) {
        //    Gizmos.DrawSphere(wp.transform.position, 0.7f);
        //}

        //    GUIStyle style = new GUIStyle();
        //    style.normal.textColor = Color.black;
        //    style.fontSize = 20;
        //    if (waypoints.Count == 0)
        //        Debug.Log("waypoints is empty");
        //    Handles.Label(StartPoint.transform.position ,"Over here", style);
        //    foreach (GameObject w in waypoints) {
        //        //WaypointAreConnected(StartPoint.gameObject, w);

        //        //Debug.Log("Normalize vector:" + Vector3.Normalize(w.transform.position - StartPoint.transform.position));
        //        // Debug.Log("Distance position:" + Vector3.Distance(w.transform.position, StartPoint.transform.position));
        //        //Gizmos.DrawLine(StartPoint.gameObject.transform.position, w.transform.position);
        //        Gizmos.color = Color.black;
        //        Gizmos.DrawLine(StartPoint.gameObject.transform.position, w.transform.position);
        //        Gizmos.color = Color.blue;
        //        Gizmos.DrawLine(StartPoint.gameObject.transform.position, StartPoint.gameObject.transform.position 
        //            + Vector3.Normalize(w.transform.position - StartPoint.transform.position) * Vector3.Distance(w.transform.position, StartPoint.transform.position));
        //    }
    }
}
