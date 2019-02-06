using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    //MeshFilter filter;
    //Mesh mesh;
    // Use this for initialization
    void Start () {
        //filter = GetComponent<MeshFilter>();
        //mesh = filter.mesh;
        //mesh.Clear();
        gameObject.tag = "Obstacle";
        gameObject.layer = 9;


    }

    Vector3 height = new Vector3(0, 2, 0);
    bool addPoint;
    public Mesh mesh; MeshFilter filter;
    public void GenerateObstacle(int numofPoints) {
        filter = GetComponent<MeshFilter>();
        mesh = filter.mesh;
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> upperVertices = new List<Vector3>();
        while (vertices.Count != numofPoints) {
            Vector3 v = GeneratePoint();
            addPoint = true;
            // To make sure the points have enough of a distance
            foreach (Vector3 p in vertices) {
                if(Vector3.Distance(v, p) < 0.75f) {
                    addPoint = false;
                    break;
                }
            }
            if (addPoint)
                vertices.Add(v);

        }
        for (int i = 0; i < vertices.Count; i++) {
       //     Debug.Log("Vertice[" + i + "]: x:" + vertices[i].x + " y:" + vertices[i].y + " z:" + vertices[i].z);
        }
      //  Debug.Log("-----------------------------------------------");

        vertices = ConvexHull(vertices);
        for (int i = 0; i < vertices.Count; i++) {
            Debug.Log("Vertice[" + i + "]: x:" + vertices[i].x + " y:" + vertices[i].y + " z:" + vertices[i].z);
        }
      //  Debug.Log("-----------------------------------------------");
        int indexAgain = vertices.Count;
     //   List<Vector3> original = vertices;
        for (int i = 0; i < indexAgain; i++) {
            vertices.Insert(i * 2 + 1, vertices[i * 2] + height);
        }
        //for(int i = 0; i < vertices.Count; i++) {
        //    Debug.Log("Vertice[" + i + "]: x:" + vertices[i].x + " y:" + vertices[i].y + " z:" + vertices[i].z);
        //}

        List<int> triangles = new List<int>();

        //for(int i = 0; i < vertices.Count; i++) {
        //    for (int j = i+1; j < vertices.Count; j++) {
        //        for (int k = j+1; k < vertices.Count; k++) {
        //            triangles.Add(i);
        //            triangles.Add(j);
        //            triangles.Add(k);
        //            triangles.Add(k);
        //            triangles.Add(j);
        //            triangles.Add(i);
        //        }
        //    }
        //}

        // bottom
        for (int i = 0; i < (vertices.Count / 2) - 2; i++) {
            triangles.Add(0);
            triangles.Add(2 * (i + 1));
            triangles.Add(2 * (i + 2));
            triangles.Add(2 * (i + 2));
            triangles.Add(2 * (i + 1));
            triangles.Add(0);
        }

        // top
        for (int i = 0; i < (vertices.Count / 2) - 2; i++) {
            triangles.Add(1 + 2 * (i));
            triangles.Add(1 + 2 * (i + 1));
            triangles.Add(1 + 2 * (i + 2));
            triangles.Add(1 + 2 * (i + 2));
            triangles.Add(1 + 2 * (i + 1));
            triangles.Add(1 + 2 * (i));
        }

        // Sides
        for (int i = 0; i < vertices.Count - 2; i++) {
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
            triangles.Add(i + 2);
            triangles.Add(i + 1);
            triangles.Add(i);
        }
        triangles.Add(vertices.Count - 2); triangles.Add(vertices.Count - 1);triangles.Add(0);
        triangles.Add(vertices.Count - 1); triangles.Add(0); triangles.Add(1);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        
        mesh.RecalculateBounds();
        
        // This must be put at the end
        MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshc.sharedMesh = mesh; // Give it your mesh here.
        

    }

    public void GenerateObstacle2(int numOfPoints) {

        filter = GetComponent<MeshFilter>();
        mesh = filter.mesh;
        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float angle = 360 / numOfPoints;
        vertices.Add(Vector3.zero);
        for(int i = 0; i < numOfPoints; i++) {
            vertices.Add(RandomCircle(Vector3.zero, 1 + Random.value* 3.5f, angle * i));
        }
        int indexAgain = vertices.Count;
        //   List<Vector3> original = vertices;
        for (int i = 0; i < indexAgain; i++) {
            vertices.Insert(i * 2 + 1, vertices[i * 2] + height);
        }

        //for (int i = 0; i < vertices.Count-2; i++) {
        //    triangles.Add(i);
        //    triangles.Add(i+1);
        //    triangles.Add(i + 2);
        //    //triangles.Add(i + 2);
        //    //triangles.Add(i + 1);
        //    //triangles.Add(i);
        //}

        //triangles.Add(vertices.Count - 2);
        //triangles.Add(vertices.Count - 1);
        //triangles.Add(0);
        //triangles.Add(vertices.Count - 1);
        //triangles.Add(0);
        //triangles.Add(1);

        for (int i = 0; i < (vertices.Count / 2) - 1; i++) {
            triangles.Add(1);
            triangles.Add(1 + 2 * (i ));
            triangles.Add(1 + 2 * (i + 1));
            //triangles.Add(1 + 2 * (i + 1));
            //triangles.Add(1 + 2 * (i ));
            //triangles.Add(1);
        }
        triangles.Add(1);
        triangles.Add(vertices.Count - 1);
        triangles.Add(3);

        for (int i = 0; i < (vertices.Count)-2; i++) {
            triangles.Add(i);
            triangles.Add(1 + i);
            triangles.Add(2 + i);
            triangles.Add(2 + i);
            triangles.Add(1 + i);
            triangles.Add(i);
        }
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(2);
        triangles.Add(2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 1);
        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(3);
        triangles.Add(2);
        triangles.Add(vertices.Count - 1);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        // This must be put at the end
        MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshc.sharedMesh = mesh; // Give it your mesh here.
        meshc.convex = true;

    }

    Vector3 RandomCircle(Vector3 center, float radius, float a) {
       // Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }
    List<Vector3> ConvexHull(List<Vector3> vertices) {
        List<Vector3> hull = new List<Vector3>();
        int index = 0;
        float leastX = vertices[0].x;
        int index2 = 0;
        for (int i = 1; i < vertices.Count; i++) {
            if(vertices[i].x < leastX) {
                index2 = index;
                leastX = vertices[i].x;
                index = i;
            }
        }
        hull.Add(vertices[index]);
        hull.Add(vertices[index2]);
        vertices.Remove(vertices[index]);
        vertices.Remove(vertices[index2]);
        float angle; float maxAngle;
        while (vertices.Count != 0) {
            angle = 0;
            maxAngle = Vector3.Angle(hull[hull.Count - 2] - hull[hull.Count - 1],
                    hull[hull.Count - 1] - vertices[0]);
            index = 0;
            for(int i = 1; i < vertices.Count; i++) {
                angle = Vector3.Angle(hull[hull.Count - 2] - hull[hull.Count - 1],
                    hull[hull.Count - 1] - vertices[i]);

                if (angle < maxAngle) {
                    maxAngle = angle;
                    index = i;
                }
                else if(angle == maxAngle) {
                    if(Vector3.Distance(vertices[i], hull[hull.Count - 1]) 
                        < Vector3.Distance(vertices[index], hull[hull.Count - 1])) {
                        maxAngle = angle;
                        index = i;
                    }
                }
            }
            hull.Add(vertices[index]);
            vertices.Remove(vertices[index]);
        }
        //angle = 0;
        //maxAngle = Vector3.Angle(Vector3.zero - hull[hull.Count - 1],
        //        hull[hull.Count - 1] - hull[0]);
        //for (int i =1 ; i < hull.Count-1; i++) {
        //    maxAngle = Vector3.Angle(Vector3.zero - hull[hull.Count - 1],
        //            hull[hull.Count - 1] - hull[i]);
        //    if (angle > maxAngle) {
        //        maxAngle = angle;
        //        index = i;
        //    }
        //}
        //hull.Add(hull[index]);
        return hull;
    }
    

    Vector3 GeneratePoint() {
    //    float x = (4 * Random.value) + 1;
        float x = Random.Range(1, 7);
        if (Random.value > 0.5f)
            x *= -1;
       //   float z = (5 * Random.value) + 1;
        float z = Random.Range(1, 7);
        if (Random.value > 0.5f)
            z *= -1;

        return new Vector3(x, 0, z);
    }

	// Update is called once per frame
	void Update () {
        if(mesh != null) {
            foreach (Vector3 v in mesh.vertices) {
                Debug.DrawLine(gameObject.transform.position + v, gameObject.transform.position + v + height, Color.blue);
            }
        }
	}

    private void OnDrawGizmos() {
        //if(mesh != null) {
        //    for (int i = 0; i < mesh.vertices.Length; i+=2) {
        //        Handles.Label(gameObject.transform.position + mesh.vertices[i], (i/2).ToString());
        //    }
        //}
    }
}
