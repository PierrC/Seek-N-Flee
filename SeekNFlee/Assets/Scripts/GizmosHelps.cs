using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosHelps : MonoBehaviour {

    List<Vector3> desriedVector;
	// Use this for initialization
	void Start () {

    }
    public void methodOne() {
        desriedVector = new List<Vector3>();
        for (int i = -13; i < 14; i++) {
            for (int j = -8; j < 9; j++) {
                desriedVector.Add(CalculateDesiredVelocity(new Vector3(i, 1, j)));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    Vector3 v;
    private void OnDrawGizmos() {
        //int h = 0;
        //for (int i = -13; i < 14; i++) {
        //    for (int j = -8; j < 9; j++) {
        //        v = new Vector3(i, 1, j);
        //        Gizmos.color = Color.black;
        //        Gizmos.DrawLine(v, v + desriedVector[h]);
        //        h++;
        //    }
        //}
    }

    public float maxVelocity;

    Vector3 CalculateDesiredVelocity(Vector3 position) {
        // final goal
        Vector3 desiredVelocity = Seek(position, WorldState.GetInstance().leavingDoors[0].transform.position);

        //foreach (Agent a in WorldState.GetInstance().agents) {
        //    // need to test function & might want to implement avaid for agents
        //        desiredVelocity += Flee(position, a.transform.position) / Vector3.Distance(a.transform.position, transform.position);
        //}

        foreach (Obstacle o in WorldState.GetInstance().obstacles) {
            // need to test function & might want to implement avaid for agents
            desiredVelocity += Flee(position, o.transform.position) / (Vector3.Distance(o.transform.position, transform.position) - 1);

            for (int i = 0; i < o.mesh.vertexCount; i++) {
                if (i % 2 == 0)
                    desiredVelocity += Flee(position, o.mesh.vertices[i] + Vector3.up) / (20 * Vector3.Distance(o.mesh.vertices[i] + Vector3.up, transform.position));
            }
            for (int i = 0; i < o.mesh.vertexCount / 2 - 1; i++) {
                desiredVelocity += Flee(position, (o.mesh.vertices[(i + 1) * 2] - o.mesh.vertices[i * 2]) + Vector3.up) / (9 * Vector3.Distance((o.mesh.vertices[(i + 1) * 2] - o.mesh.vertices[i * 2]) + Vector3.up, transform.position));
            }
        }

        return desiredVelocity;
    }

    public Vector3 Seek(Vector3 start, Vector3 target) {
        return maxVelocity * Vector3.Normalize(target - start);
    }
    public Vector3 Flee(Vector3 start, Vector3 target) {
        return maxVelocity * Vector3.Normalize(start - target);
    }

}
