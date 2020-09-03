using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using TeamLab.Unity;
using UnityMeshSimplifier;
using FlowerPackage;

[System.Serializable]
public class Sky : MonoBehaviour {

    public GameObject Star;
    static public Data data = ConstellationGUI.data;
    
    private MeshHelper edge;
    private Vector3 meshPos;

    public GameObject Ursa;
    public GameObject Leo;
    public GameObject Tiger;

    //make public list of GameObjects for constellations
    public List<GameObject> constellationList = new List<GameObject>();

    // make public enum to determine what visual mode constellation will use
    // Mesh = stars go to random point on constellation mesh
    // Edge = stars go to random point on constellation edge
    // Vertex = stars go to random vertex
    public enum ConstellationMode
    {
        Mesh,
        Edge,
        Vertex
    }

    // Set constellationMode as Mesh to start
    public ConstellationMode mode = ConstellationMode.Mesh;

    // time when constellation was initialized
    private float constellationInitializationTime;

    private GameObject ConstellationShape;

    private Renderer constellationRenderer;

    void Start()
    {
        // Get random position on mesh
        edge = new MeshHelper();

        constellationList.Add(Ursa);
        constellationList.Add(Leo);
        constellationList.Add(Tiger);

        // get a constellation to start to avoid initialization errors
        ConstellationShape = edge.GetRandomConstellation(constellationList);

        constellationRenderer = ConstellationShape.GetComponent<Renderer>();
}

    // call on click
    void OnMouseDown()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        // attempt to simplify constellation meshes
        // https://github.com/Whinarn/UnityMeshSimplifier
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();

        if (Physics.Raycast(ray, out hit))
        {

            // establish time at which constellation was instantiated
            constellationInitializationTime = Time.timeSinceLevelLoad;

            // potentially create multiple constellations with one click
            for(int q = 0; q < data.constellationCount; q++)
            {
                ConstellationShape = edge.GetRandomConstellation(constellationList);

                constellationRenderer = ConstellationShape.GetComponent<Renderer>();

                // instantiate new constellation
                GameObject c = Instantiate(ConstellationShape,
                   hit.point,
                   ConstellationShape.transform.rotation)
                   as GameObject;

                // TODO: PREPROCESS don't simplify the mesh every time it's created
                meshSimplifier.Initialize(c.GetComponent<MeshFilter>().sharedMesh);
                meshSimplifier.SimplifyMesh(data.quality);

                c.GetComponent<MeshFilter>().sharedMesh = meshSimplifier.ToMesh();

                for (int i = 0; i < data.numStars; i++)
                {
                    GameObject s = Instantiate(Star, hit.point, Quaternion.identity);
                    Star star = s.GetComponent<Star>();

                    switch (mode)
                    {
                        case ConstellationMode.Mesh:
                            {
                                meshPos = edge.GetRandomPointOnConstellationMesh(c);
                                break;
                            }
                        case ConstellationMode.Edge:
                            {
                                meshPos = edge.GetRandomPointOnConstellationEdge(c);
                                break;
                            }
                        case ConstellationMode.Vertex:
                            {
                                meshPos = edge.GetRandomPointOnConstellationVertex(c);
                                break;
                            }
                        default: // because might as well?
                            {
                                Debug.Log("We're in Default I guess.");
                                break;
                            }
                    }

                    star.targetPos = meshPos;
                }

                // Destroy constellation game object 1 second after stars fade
                Destroy(c, data.lifespan + data.timeToFade + 1.0f);
            }
           
        }
    }

    public void UpdateConstellationList(bool toggle, GameObject constellation)
    {
        if (!toggle)
        {
            constellationList.Remove(constellation);
        }
        else if (toggle & !constellationList.Contains(constellation))
        {
            constellationList.Add(constellation);
        }
    }

    public void UpdateConstellationAlpha(float alpha, float oldAlpha)
    {
        // only update shader if alpha has changed
        if (alpha != oldAlpha)
        { 
            constellationRenderer.sharedMaterial.SetFloat("_Transparency", alpha);
        }
    }
}
