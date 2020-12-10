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

    private StarData[] starData;

    void Awake()
    {
        // Get random position on mesh
        edge = new MeshHelper();
    }

    // method to spawn stars on mesh given the number of stars and constellation gameobject
    public void SpawnStars(GameObject c, int numStars)
    {
        starData = new StarData[numStars];

        // generate stars on mesh
        for (int i = 0; i < numStars; i++)
        {
            starData[i] = new StarData();

            // save values unique to each star in StarData
            starData[i].idx = i;
            starData[i].velocity = Random.Range(GUIData.minVelocity, GUIData.maxVelocity);
            starData[i].size = Random.Range(GUIData.minSize, GUIData.maxSize);
            starData[i].twinkleSpeed = GUIData.twinkleSpeed;
            starData[i].lifespan = GUIData.lifespan;
            starData[i].timeToFade = GUIData.timeToFade;
            starData[i].color = GUIData.starColor;
            starData[i].minDistance = GUIData.vertexStarMinDistance;

            // begin by instantiating stars at center of constellation gameobject
            GameObject s = Instantiate(Star, c.GetComponentInChildren<Renderer>().bounds.center, Quaternion.identity);
            s.transform.parent = c.transform;
            Star star = s.GetComponent<Star>();

            // start by just sending stars to random points on the mesh
            // > get pseudo-random point based on star's unique id
            Vector3 targetPosition = edge.GetRandomPointOnConstellationMesh(c, (float) i/numStars);

            // assign remaining starData values here
            starData[i].triangleIndex = edge.thisEdge.triangleIndex;
            starData[i].position = targetPosition;

            // assign values to this star as necessary
            star.id = starData[i].idx;
            star.velocity = starData[i].velocity;
            star.size = starData[i].size;
            star.twinkleSpeed = starData[i].twinkleSpeed;
            star.lifespan = starData[i].lifespan;
            star.timeToFade = starData[i].timeToFade;
            star.targetPos = starData[i].position;
        }
    }

    // if skinned mesh (animated) bake stuff

    /*_skin.BakeMesh(_baked);
    // _vertices assigned to mesh vertices
     _vertices = _baked.vertices;
     _triangles = _baked.triangles;*/

    // if regular mesh (static) just get that mesh info
}
