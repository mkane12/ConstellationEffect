using System;
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

    // create a GameObject constellationShape to record the shape of the constellation
    public GameObject constellationShape;

    private MeshHelper edge;
    private List<Vector3> meshEdgePositions;
    private List<Vector3> meshVertexPositions;

    private StarData[] starData;

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

    // spawn stars at start
    void Start()
    {
        skinnedMesh = this.GetComponentInChildren<SkinnedMeshRenderer>();

        starData = new StarData[numStars];

        // generate stars on mesh
        for (int i = 0; i < numStars; i++)
        {
            starData[i] = new StarData();

            // save values unique to each star in StarData
            starData[i].idx = i;
            starData[i].velocity = UnityEngine.Random.Range(GUIData.minVelocity, GUIData.maxVelocity);
            starData[i].size = UnityEngine.Random.Range(GUIData.minSize, GUIData.maxSize);
            starData[i].twinkleSpeed = GUIData.twinkleSpeed;
            starData[i].lifespan = GUIData.lifespan;
            starData[i].timeToFade = GUIData.timeToFade;
            starData[i].color = GUIData.starColor;
            starData[i].minDistance = GUIData.vertexStarMinDistance;

            // begin by instantiating stars at center of constellation gameobject
            GameObject s = Instantiate(Star, this.GetComponentInChildren<Renderer>().bounds.center, Quaternion.identity);
            s.transform.parent = this.transform;
            Star star = s.GetComponent<Star>();

            // start by just sending stars to random points on the mesh
            // > get pseudo-random point based on star's unique id
            Vector3 targetPosition;

            // if mesh is animated
            if (skinnedMesh != null)
            {
                // bake a "snapshot" of the skinned mesh renderer and store in bakedMesh
                skinnedMesh.BakeMesh(bakedMesh);

                targetPosition = edge.GetRandomPointOnAnimatedConstellationMesh(
                    bakedMesh,
                    this.constellationShape,
                    (float)i / numStars);
            }
            else // mesh is static
            {
                targetPosition = edge.GetRandomPointOnStaticConstellationMesh(
                    this.GetComponentInChildren<MeshFilter>().sharedMesh,
                    this.constellationShape,
                    (float)i / numStars);
            }

            // assign remaining starData values here
            starData[i].triangleIndex = edge.thisEdge.triangleIndex;
            starData[i].position = targetPosition;

            // assign values to this star as necessary
            star.starData = starData[i];
        }
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
            starData[i].position = edge.GetRandomPointOnAnimatedConstellationMesh(bakedMesh, this.constellationShape, (float)i / numStars);
        }
    }

}
