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
    static public Data GUIData = ConstellationGUI.data;

    // create a GameObject constellationShape to record the shape of the constellation
    public GameObject constellationShape;

    private MeshHelper edge;
    private List<Vector3> meshEdgePositions;
    private List<Vector3> meshVertexPositions;

    private StarData[] vertexStarData;
    private StarData[] edgeStarData;

    private List<Vector3> vertexStarPositions;

    public int numVertexStars; // this is the original number of vertex stars
    // the number of vertex stars changes after vertex positions are calculated, so this just stores that value

    public int numVertexStarsNew; 
    public int numEdgeStars;

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

        // get list of vertices  to determine how many vertex stars to make
        if (skinnedMesh != null) // mesh is animated
        {
            // bake a "snapshot" of the skinned mesh renderer and store in bakedMesh
            skinnedMesh.BakeMesh(bakedMesh);

            vertexStarPositions = edge.GetVertexListFoAnimatedConstellation(
                bakedMesh,
                this,
                numVertexStars,
                GUIData.vertexStarMinDistance);
        }
        else // mesh is static
        {
            vertexStarPositions = edge.GetVertexListForStaticConstellation(
                this.GetComponent<MeshFilter>().sharedMesh,
                this,
                numVertexStars,
                GUIData.vertexStarMinDistance);
        }

        // if there aren't enough vertices, reduce number of vertex stars generated
        numVertexStarsNew = vertexStarPositions.Count;

        vertexStarData = new StarData[numVertexStarsNew];
        edgeStarData = new StarData[numEdgeStars];

        // generate stars on mesh
        for (int i = 0; i < numVertexStarsNew; i++)
        {
            makeVertexStar(i);
        }

        for (int i = 0; i < numEdgeStars; i++)
        {
            makeEdgeStar(i);
        }
    }

    // Function to create vertex stars
    private void makeVertexStar(int i)
    {
        vertexStarData[i] = new StarData();

        // save values unique to each star in StarData
        vertexStarData[i].idx = i;
        //vertexStarData[i].velocity = UnityEngine.Random.Range(GUIData.minVelocity, GUIData.maxVelocity);
        //vertexStarData[i].size = UnityEngine.Random.Range(GUIData.minSizeVertex, GUIData.maxSizeVertex);
        vertexStarData[i].twinkleSpeed = GUIData.twinkleSpeedVertex;
        vertexStarData[i].lifespan = GUIData.lifespan;
        vertexStarData[i].timeToFade = GUIData.timeToFade;
        vertexStarData[i].color = GUIData.vertexStarColor;
        vertexStarData[i].minDistance = GUIData.vertexStarMinDistance;

        // begin by instantiating stars at center of constellation gameobject
        GameObject s = Instantiate(Star, this.GetComponentInChildren<Renderer>().bounds.center, Quaternion.identity);
        s.transform.parent = this.transform;
        Star star = s.GetComponent<Star>();

        Vector3 targetPosition = vertexStarPositions[i];

        // assign remaining starData values here
        vertexStarData[i].triangleIndex = edge.thisEdge.triangleIndex;
        vertexStarData[i].position = targetPosition;

        // assign values to this star as necessary
        star.starData = vertexStarData[i];
    }

    // Function to create edge stars
    private void makeEdgeStar(int i)
    {
        edgeStarData[i] = new StarData();

        // save values unique to each star in StarData
        edgeStarData[i].idx = i;
        //edgeStarData[i].velocity = UnityEngine.Random.Range(GUIData.minVelocity, GUIData.maxVelocity);
        //edgeStarData[i].size = UnityEngine.Random.Range(GUIData.minSizeEdge, GUIData.maxSizeEdge);
        edgeStarData[i].twinkleSpeed = GUIData.twinkleSpeedEdge;
        edgeStarData[i].lifespan = GUIData.lifespan;
        edgeStarData[i].timeToFade = GUIData.timeToFade;
        edgeStarData[i].color = GUIData.edgeStarColor;

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

            targetPosition = edge.GetRandomPointOnAnimatedConstellationEdge(
                bakedMesh,
                this,
                (float)i / numEdgeStars);
        }
        else // mesh is static
        {
            targetPosition = edge.GetRandomPointOnStaticConstellationEdge(
                this.GetComponent<MeshFilter>().sharedMesh,
                this,
                (float)i / numEdgeStars);
        }

        // assign remaining starData values here
        edgeStarData[i].triangleIndex = edge.thisEdge.triangleIndex;
        edgeStarData[i].position = targetPosition;

        // assign values to this star as necessary
        star.starData = edgeStarData[i];
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

        vertexStarPositions = edge.GetVertexListFoAnimatedConstellation(
                bakedMesh,
                this,
                numVertexStars,
                GUIData.vertexStarMinDistance);

        for (int i = 0; i < numVertexStarsNew; i ++)
        {
            vertexStarData[i].position = vertexStarPositions[i];
        }
        
        for (int i = 0; i < numEdgeStars; i++)
        {
            edgeStarData[i].position = edge.GetRandomPointOnAnimatedConstellationEdge(
                bakedMesh,
                this,
                (float)i / numEdgeStars);
        }
    }

}
