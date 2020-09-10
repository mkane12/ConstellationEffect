using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using TeamLab.Unity;
using UnityMeshSimplifier;
using FlowerPackage;
using System;

[System.Serializable]
public class Sky : MonoBehaviour {

    public GameObject Star;
    static public Data data = ConstellationGUI.data;
    
    private MeshHelper edge;
    private Vector3 meshPos;

    // made static to be accessable in Data class
    public GameObject Ursa;
    public GameObject Leo;
    public GameObject Tiger;

    //make public dictionary of GameObjects for constellations
    public Dictionary<ConstellationType, GameObject> constellationList = new Dictionary<ConstellationType, GameObject>();

    // make public enum to determine what visual mode constellation will use
    // enums cannot be changed at runtime -> defacto way of knowing what all of the values are
    // Mesh = stars go to random point on constellation mesh
    // Edge = stars go to random point on constellation edge
    // Vertex = stars go to random vertex
    public enum ConstellationMode
    {
        Mesh,
        Edge,
        Vertex
    }

    public enum ConstellationType
    {
        Ursa,
        Leo,
        Tiger
    }

    // time when constellation was initialized
    private float constellationInitializationTime;

    private GameObject ConstellationShape;

    private Renderer constellationRenderer;

    void Start()
    {
        // Get random position on mesh
        edge = new MeshHelper();

        constellationList.Add(ConstellationType.Ursa, Ursa);
        constellationList.Add(ConstellationType.Leo, Leo);
        constellationList.Add(ConstellationType.Tiger, Tiger);

        // get a constellation to start to avoid initialization errors
        ConstellationShape = edge.GetRandomConstellation(constellationList, data.constellationNames);

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
                ConstellationShape = edge.GetRandomConstellation(constellationList, data.constellationNames);

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

                    switch (data.mode)
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

    public void UpdateConstellationList(bool toggle, ConstellationType constellationName)
    {
        // toggle for a given constellation is off
        if (!toggle)
        {
            data.constellationNames.Remove(constellationName);
        }
        // toggle is on and the name is not present in the data list of constellation names
        else if (toggle & !data.constellationNames.Contains(constellationName))
        {
            data.constellationNames.Add(constellationName);
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
