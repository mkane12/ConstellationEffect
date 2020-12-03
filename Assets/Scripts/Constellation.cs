using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamLab.Unity;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    public GameObject Star;
    public GameObject EdgeStar;
    static public Data GUIData = ConstellationGUI.data;

    private MeshHelper edge;
    private List<Vector3> meshEdgePositions;
    private List<Vector3> meshVertexPositions;

    private List<StarData> starData;

    void Awake()
    {
        // Get random position on mesh
        edge = new MeshHelper();
    }

    // method to spawn stars on mesh given the number of stars and constellation gameobject
    public void SpawnStars(GameObject c, int numStars)
    {
        // generate stars on mesh
        for (int i = 0; i < numStars; i++)
        {
            // begin by instantiating stars at center of constellation gameobject
            GameObject s = Instantiate(Star, c.GetComponentInChildren<Renderer>().bounds.center, Quaternion.identity);
            s.transform.parent = c.transform;
            Star star = s.GetComponent<Star>();

            // start by just generating stars at random position on mesh
            Vector3 targetPosition = edge.GetRandomPointOnConstellationMesh(c);

            star.targetPos = targetPosition;
        }
    }

    // if skinned mesh (animated) bake stuff

    // if regular mesh (static) just get that mesh info

    /*_skin.BakeMesh(_baked);
    // _vertices assigned to mesh vertices
     _vertices = _baked.vertices;
     _triangles = _baked.triangles;*/
}
