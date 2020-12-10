using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamLab.Unity;
using UnityEditor;
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

    public GameObject constellationObject;
    public int numStars;

    Mesh bakedMesh;
    SkinnedMeshRenderer skinnedMesh;

    void Awake()
    {
        // Get random position on mesh
        edge = new MeshHelper();

        // Mesh "screenshots" for animated mesh
        bakedMesh = new Mesh();
    }

    // method to spawn stars on mesh given the number of stars and constellation gameobject
    public void SpawnStars()
    {
        GameObject c = Instantiate(constellationObject,
                   this.transform.position,
                   constellationObject.transform.rotation)
                   as GameObject;

        var path = "Assets/Meshes/" + GUIData.quality.ToString("F1") + "/" + constellationObject.name + ".asset";
        c.GetComponentInChildren<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);


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
            Vector3 targetPosition = edge.GetRandomPointOnConstellationMesh(
                c.GetComponentInChildren<MeshFilter>().sharedMesh, 
                c,
                (float) i/numStars);

            // assign remaining starData values here
            starData[i].triangleIndex = edge.thisEdge.triangleIndex;
            starData[i].position = targetPosition;

            // assign values to this star as necessary
            star.starData = starData[i];
        }

        skinnedMesh = constellationObject.GetComponentInChildren<SkinnedMeshRenderer>();

        Destroy(c, GUIData.lifespan + GUIData.timeToFade + 1.0f);
    }

    // Called every frame
    private void Update()
    {
        // call UpdatePosition if constellation has a skinned mesh (means it's animated)
        if (skinnedMesh != null)
        {
            UpdatePosition();
        }
    }

    // function to update stars' positions if mesh is animated
    public void UpdatePosition()
    {
        // bake a "snapshot" of the skinned mesh renderer and store in bakedMesh
        skinnedMesh.BakeMesh(bakedMesh);

        for(int i = 0; i < numStars; i ++)
        {
            starData[i].position = edge.GetRandomPointOnConstellationMesh(bakedMesh, constellationObject, (float)i / numStars);
        }
    }

}
