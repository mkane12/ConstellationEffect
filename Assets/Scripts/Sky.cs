using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour {

    public GameObject Star;
    public int numStars = 12; // number of stars to spawn
    public GameObject ConstellationShape;
    private EdgeHelper edge;
    private Mesh mesh;

    void Start()
    {
        MeshFilter mf = (MeshFilter)ConstellationShape.GetComponent("MeshFilter");
        mesh = mf.sharedMesh;

        // Get random position on mesh
        edge = new EdgeHelper();
    }

    // call on click
    void OnMouseDown()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            for (int i = 0; i < numStars; i++)
            {
                GameObject s = Instantiate(Star, hit.point, Quaternion.identity);
                Star star = s.GetComponent<Star>();
                // multiply by scale of constellation shape and move by transform
                star.targetPos = Vector3.Scale(edge.GetRandomPointOnMesh(mesh), ConstellationShape.transform.localScale) 
                    + ConstellationShape.transform.position;
            }
        }

    }
}
