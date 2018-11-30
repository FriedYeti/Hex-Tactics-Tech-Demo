using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshArrow : MonoBehaviour {

    public Vector3[] newVertices;
    public int[] newTriangles;

	// Use this for initialization
	void Start () {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }
	
	// Update is called once per frame
	void Update () {
        foreach (Vector3 vertex in newVertices) {
            Debug.DrawRay(vertex, Vector3.up, Color.red);
        }
	}
}
