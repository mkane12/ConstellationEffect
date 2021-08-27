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
        public float alpha;
        public float lifespan;
        public float timeToFade;
    };

    //public GameObject Star;
    static public Data GUIData = ConstellationGUI.data;

    // create a GameObject constellationShape to record the shape of the constellation
    public GameObject constellationShape;

    private MeshHelper edge;

    // this will measure the amount of time the constellation has been in existence
    float duration;

    [SerializeField, Range(10, 1000)]
    public int numStars = 1000;

    // TODO: we start by just placing stars on every vertex
    static readonly int
        verticesId = Shader.PropertyToID("_Vertices"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress"),
        lifespanId = Shader.PropertyToID("_Lifespan"),
        timeToFadeId = Shader.PropertyToID("_TimeToFade"),
        durationId = Shader.PropertyToID("_Duration"),
        alphaId = Shader.PropertyToID("_Alpha"),
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
    Texture2D texture;

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
        computeShader.SetFloat(lifespanId, GUIData.lifespan);
        computeShader.SetFloat(timeToFadeId, GUIData.timeToFade);

        instanceMaterial.SetFloat("_Size", GUIData.size);

        computeShader.SetFloat(alphaId, 1f);

        Color starColor;
        if (ColorUtility.TryParseHtmlString(GUIData.color, out starColor))
            instanceMaterial.SetColor("_Color", starColor);
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

    // Called every frame
    private void Update()
    {
        duration += Time.deltaTime;

        UpdatePositionOnGPU();
    }

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

        computeShader.SetFloat(durationId, duration);

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

    /*void UpdateFade()
    {
        float alpha = Mathf.Lerp(1f, 0f,
             Mathf.Clamp01((duration - GUIData.lifespan) / GUIData.timeToFade));

        instanceMaterial.SetFloat("_Transparency", alpha);
    }*/

    protected virtual void SetMaterialBuffer()
    {
        // material needs to know positions where to draw points
        instanceMaterial.SetBuffer(instanceDataId, instanceDataBuffer);
    }

}
