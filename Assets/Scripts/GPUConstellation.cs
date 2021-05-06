using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TeamLab.Unity;
using UnityEditor;
using UnityEngine;

public class GPUConstellation : MonoBehaviour
{ 
    public struct InstanceData
    {
        public Vector3 startPosition;
        public Vector3 position;
    };

    //public GameObject Star;
    static public Data GUIData = ConstellationGUI.data;

    // create a GameObject constellationShape to record the shape of the constellation
    public GameObject constellationShape;

    private MeshHelper edge;

    // this will measure the amount of time the constellation has been in existence
    float duration;

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

    // TODO: we start by just placing stars on every vertex
    static readonly int
        verticesId = Shader.PropertyToID("_Vertices"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress"),
        instanceDataId = Shader.PropertyToID("_InstanceDataBuffer");

    // get kernel ids for compute shader
    int _init_kernel_idx;

    Mesh mesh;

    Mesh bakedMesh;
    SkinnedMeshRenderer skinnedMesh;

    private int subMeshIndex = 0;

    protected ComputeBuffer vertexBuffer;
    protected ComputeBuffer instanceDataBuffer;

    protected ComputeBuffer argsBuffer;
    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    Material material;

    public Material instanceMaterial;

    [SerializeField]
    Mesh starMesh;

    protected InstanceData[] instanceDataArray; // this is the array of InstanceData that will be passed to the shader

    private void OnEnable()
    {
        // Get random position on mesh
        edge = new MeshHelper();

        instanceMaterial = new Material(material);

        // Mesh "screenshots" for animated mesh
        bakedMesh = new Mesh();

        skinnedMesh = this.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMesh != null) // mesh is animated
        {
            // bake a "snapshot" of the skinned mesh renderer and store in bakedMesh
            skinnedMesh.BakeMesh(bakedMesh);
            mesh = bakedMesh;
        }
        else // mesh is static
        {
            mesh = this.GetComponent<MeshFilter>().sharedMesh;
        }

        instanceDataArray = new InstanceData[mesh.vertices.Length];

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        // getting kernels in compute shader
        _init_kernel_idx = computeShader.FindKernel("Init");

        // storing numStars Vector3 positions, each Vector3 is 3 floats 4 bytes each
        vertexBuffer = new ComputeBuffer(mesh.vertices.Length, 3 * sizeof(float));
        instanceDataBuffer = new ComputeBuffer(mesh.vertices.Length, Marshal.SizeOf(typeof(InstanceData)));

        computeShader.SetFloat(timeId, Time.time);

        instanceMaterial.SetFloat("_Size", GUIData.size);
    }

    // invoked when component is disabled (if constellation destroyed and right before hot reload)
    private void OnDisable()
    {
        if (vertexBuffer != null)
        {
            vertexBuffer.Release(); // free GPU memory allocated for the buffer
        }
        vertexBuffer = null; // possible for object to be reclaimed by Unity's memory garbage collection process

        if (instanceDataBuffer != null)
        {
            instanceDataBuffer.Release();
        }
        instanceDataBuffer = null; 

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
        duration += Time.deltaTime;

        //UpdatePosition();
        UpdatePositionOnGPU();

        if (duration >= GUIData.lifespan)
        {
            UpdateFade();
        }
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
        // set transforms for constellation (position, rotation, scale)
        computeShader.SetVector("constellationPosition", this.transform.position);
        Vector4 rotation = new Vector4(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
        computeShader.SetVector("constellationRotation", rotation);

        // Indirect args for ComputeBuffer
        if (skinnedMesh != null) // mesh is animated
        {
            skinnedMesh.BakeMesh(bakedMesh);
            mesh = bakedMesh;
            // Note: animated meshes don't need to be scaled
            computeShader.SetVector("constellationScale", Vector3.one);
        }
        else // mesh is static
        {
            computeShader.SetVector("constellationScale", this.transform.localScale);
        }

        computeShader.SetFloat(timeId, Time.time);

        // update the progress stars are making to end positions
        computeShader.SetFloat(transitionProgressId,
            Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(duration / GUIData.timeToMove))
        );

        // index count per instance, instance count, start index location, base vertex location, start instance location
        args[0] = (uint)starMesh.GetIndexCount(subMeshIndex);
        args[1] = (uint)(mesh.vertices.Length);
        args[2] = (uint)starMesh.GetIndexStart(subMeshIndex);
        args[3] = (uint)starMesh.GetBaseVertex(subMeshIndex);

        argsBuffer.SetData(args);

        Vector3[] vertices = mesh.vertices;

        vertexBuffer.SetData(vertices);

        computeShader.SetBuffer(_init_kernel_idx, verticesId, vertexBuffer); // link buffer to kernel

        // declares space and "passes" data, but doesn't CALL anything
        // Note: might as well set in Update, since it's not a heavy operation
        computeShader.SetBuffer(_init_kernel_idx, instanceDataId, instanceDataBuffer);

        // this actually CALLS the function
        computeShader.Dispatch(_init_kernel_idx, Mathf.CeilToInt(mesh.vertices.Length / 16f), 1, 1);

        SetMaterialBuffer();

        // create bounds box so unity knows where to draw the stars
        // TODO: just made a very big area to start
        var bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);

        // render
        Graphics.DrawMeshInstancedIndirect(
            starMesh, subMeshIndex, instanceMaterial, bounds, argsBuffer
        );
    }

    void UpdateFade()
    {
        instanceMaterial.SetFloat("_Transparency",
             Mathf.Lerp(1f, 0f, 
             Mathf.Clamp01((duration - GUIData.lifespan) / GUIData.timeToFade))
        );
    }

    protected virtual void SetMaterialBuffer()
    {
        // material needs to know positions where to draw points
        instanceMaterial.SetBuffer(instanceDataId, instanceDataBuffer);
    }

}
