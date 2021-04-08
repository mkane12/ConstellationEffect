using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamLab.Unity;
using UnityEditor;
using UnityEngine;

public class GPUConstellation : MonoBehaviour
{
    //public GameObject Star;
    static public Data GUIData = ConstellationGUI.data;

    // create a GameObject constellationShape to record the shape of the constellation
    public GameObject constellationShape;

    private MeshHelper edge;
    //private List<Vector3> meshEdgePositions;
    //private List<Vector3> meshVertexPositions;

    //private StarData[] vertexStarData;
    //private StarData[] edgeStarData;

    //private List<Vector3> vertexStarPositions;

    //public int numVertexStars; // this is the original number of vertex stars
    // the number of vertex stars changes after vertex positions are calculated, so this just stores that value

    //public int numVertexStarsNew; 
    //public int numEdgeStars;

    [SerializeField, Range(10, 1000)]
    public int numStars = 1000;

    static readonly int
        starPositionsId = Shader.PropertyToID("_StarPositions"),
        numStarsId = Shader.PropertyToID("_NumStars");

    Mesh bakedMesh;
    SkinnedMeshRenderer skinnedMesh;

    private int subMeshIndex = 0;

    ComputeBuffer starPositionsBuffer;
    ComputeBuffer argsBuffer;
    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    Material material;

    private void OnEnable()
    {
        // Get random position on mesh
        edge = new MeshHelper();

        // Mesh "screenshots" for animated mesh
        bakedMesh = new Mesh();

        skinnedMesh = this.GetComponentInChildren<SkinnedMeshRenderer>();

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        // storing numStars Vector3 positions, each Vector3 is 3 floats 4 bytes each
        starPositionsBuffer = new ComputeBuffer(numStars, 3 * 4);
    }

    // invoked when component is disabled (if constellation destroyed and right before hot reload)
    private void OnDisable()
    {
        if (starPositionsBuffer != null)
        {
            starPositionsBuffer.Release(); // free GPU memory allocated for the buffer
        }
        starPositionsBuffer = null; // possible for object to be reclaimed by Unity's memory garbage collection process

        if (argsBuffer != null)
        {
            argsBuffer.Release();
        }
        argsBuffer = null;
    }

    // spawn stars at start
    /*void Start()
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
        //numVertexStarsNew = vertexStarPositions.Count;

        //vertexStarData = new StarData[numVertexStarsNew];
        //edgeStarData = new StarData[numEdgeStars];

        // generate stars on mesh
        for (int i = 0; i < numVertexStarsNew; i++)
        {
            makeVertexStar(i);
        }

        for (int i = 0; i < numEdgeStars; i++)
        {
            makeEdgeStar(i);
        }
    }*/

    // Function to create vertex stars
    /*private void makeVertexStar(int i)
    {
        vertexStarData[i] = new StarData();

        // save values unique to each star in StarData
        vertexStarData[i].idx = i;
        vertexStarData[i].velocity = UnityEngine.Random.Range(GUIData.minVelocity, GUIData.maxVelocity);
        vertexStarData[i].size = UnityEngine.Random.Range(GUIData.minSizeVertex, GUIData.maxSizeVertex);
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
    }*/

    // Function to create edge stars
    /*private void makeEdgeStar(int i)
    {
        edgeStarData[i] = new StarData();

        // save values unique to each star in StarData
        edgeStarData[i].idx = i;
        edgeStarData[i].velocity = UnityEngine.Random.Range(GUIData.minVelocity, GUIData.maxVelocity);
        edgeStarData[i].size = UnityEngine.Random.Range(GUIData.minSizeEdge, GUIData.maxSizeEdge);
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
    }*/

    // Called every frame
    private void Update()
    {
        //UpdatePosition();
        UpdatePositionOnGPU();
    }

    // function to update stars' positions if mesh is animated
    /*public void UpdatePosition()
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
    }*/

    void UpdatePositionOnGPU()
    {
        // Indirect args for ComputeBuffer
        if (skinnedMesh != null)
        {
            skinnedMesh.BakeMesh(bakedMesh);

            args[0] = (uint)bakedMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)(numStars);
            args[2] = (uint)bakedMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)bakedMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            args[0] = args[1] = args[2] = args[3] = 0;
        }
        argsBuffer.SetData(args);

        computeShader.SetInt(numStarsId, numStars);

        var kernelIndex = 0;
        computeShader.SetBuffer(kernelIndex, starPositionsId, starPositionsBuffer); // link buffer to kernel

        // TODO: what's a good number of work groups?
        int groups = Mathf.CeilToInt(Mathf.Sqrt(numStars) / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(starPositionsId, starPositionsBuffer);

        // create bounds box so unity knows where to draw the graph
        // TODO: just made a very big area to start
        var bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);

        // render
        Graphics.DrawMeshInstancedIndirect(
            bakedMesh, subMeshIndex, material, bounds, argsBuffer
        );
    }

}
