﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using TeamLab.Unity;
using UnityMeshSimplifier;
using FlowerPackage;
using System;
using System.Linq;
using UnityEditor;

[System.Serializable]
public class Sky : MonoBehaviour {

    //public GameObject Star;
    //public GameObject EdgeStar;
    static public Data data = ConstellationGUI.data;

    public GameObject constellation;
    
    private MeshHelper edge;
    private List<Vector3> meshEdgePositions;
    private List<Vector3> meshVertexPositions;

    public GameObject Ursa;
    public GameObject Leo;
    public GameObject Tiger;
    public GameObject Orion;

    public MeshSimplifier meshSimplifier;

    //make public dictionary of GameObjects for constellations
    public Dictionary<ConstellationType, GameObject> constellationList = new Dictionary<ConstellationType, GameObject>();

    // make public enum to determine what visual mode constellation will use
    // enums cannot be changed at runtime -> defacto way of knowing what all of the values are
    // Mesh = stars go to random point on constellation mesh
    // Edge = stars go to random point on constellation edge
    // Vertex = stars go to random vertex
    /*public enum ConstellationMode
    {
        Mesh,
        Edge,
        Vertex
    }*/

    public enum ConstellationType
    {
        Ursa,
        Leo,
        Tiger,
        Orion
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
        constellationList.Add(ConstellationType.Orion, Orion);

        // attempt to simplify constellation meshes
        // https://github.com/Whinarn/UnityMeshSimplifier
        meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();

        // Preprocess all meshes to have varying levels of complexity
        PreprocessMeshes();

        // get a constellation to start to avoid initialization errors
        ConstellationShape = edge.GetRandomConstellation(constellationList, data.constellationNames);

        constellationRenderer = ConstellationShape.GetComponentInChildren<Renderer>();
    }

    // before artwork starts, standardize all meshes to have the same number of triangles
    // * equal to the number of triangles in the least complex mesh
    public void PreprocessMeshes()
    {
        Mesh conMesh = constellationList.First().Value.GetComponentInChildren<MeshFilter>().sharedMesh;
        float minTriangles = conMesh.triangles.Length;

        // first, identify the mesh with the smallest number of triangles and get that number
        // float minTriangles = constellationList[0].GetComponent<MeshFilter>().sharedMesh.triangles.Length;

        foreach (GameObject c in constellationList.Values)
        {
            if (c.GetComponentInChildren<MeshFilter>().sharedMesh.triangles.Length < minTriangles)
            {
                minTriangles = c.GetComponentInChildren<MeshFilter>().sharedMesh.triangles.Length;
            }
        }

        // create Base folder for standardized meshes (1.0f quality)
        if(!Directory.Exists("Assets/Meshes/1.0"))
        {
            Directory.CreateDirectory("Assets/Meshes/1.0");
        }

        // next, standardize all meshes to have that number of triangles
        foreach (var c in constellationList)
        {
            float conTriangles = c.Value.GetComponentInChildren<MeshFilter>().sharedMesh.triangles.Length;
            float meshQuality = minTriangles / conTriangles;

            // simplify the copy
            meshSimplifier.Initialize(c.Value.GetComponentInChildren<MeshFilter>().sharedMesh);
            meshSimplifier.SimplifyMesh(meshQuality);

            string conName = c.Value.name;

            // save base meshes to Assets/Meshes/1.0
            var savePathBase = "Assets/Meshes/1.0/" + conName + ".asset";
            Debug.Log("Saved base mesh to: " + savePathBase);
            AssetDatabase.CreateAsset(meshSimplifier.ToMesh(), savePathBase);

            // set constellation mesh to new base mesh
            c.Value.GetComponentInChildren<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(savePathBase);

            // Finally, create meshes at varying complexities so they don't have to be calculated in-game
            // NOTE: only creates new meshes if directory doesn't exist
            // >> i.e. if new constellations added, NEED TO RETHINK THIS
            for (float i = 0; i < 1.0; i += 0.1f)
            {
                // create folders for each complexity meshes
                if (!Directory.Exists("Assets/Meshes/" + i.ToString("F1")))
                {
                    Directory.CreateDirectory("Assets/Meshes/" + i.ToString("F1"));
                }

                // e.g. Assets/Meshes/0.1/Ursa.asset"
                var savePath = "Assets/Meshes/" + i.ToString("F1") + "/" + conName + ".asset";

                meshSimplifier.Initialize(c.Value.GetComponentInChildren<MeshFilter>().sharedMesh);
                meshSimplifier.SimplifyMesh(i);

                Debug.Log("Saved " + i.ToString("F1") + " quality mesh to: " + savePath);
                AssetDatabase.CreateAsset(meshSimplifier.ToMesh(), savePath);
            }
        }

        // save changes made
        AssetDatabase.SaveAssets();
    }

    // call on click
    void OnMouseDown()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            // establish time at which constellation was instantiated
            constellationInitializationTime = Time.timeSinceLevelLoad;

            // potentially create multiple constellations with one click
            for(int q = 0; q < data.constellationCount; q++)
            {
                // TODO: reorganize to generate name (constellation type) and get the game object FROM that
                constellation = edge.GetRandomConstellation(constellationList, data.constellationNames);

                // Cast c as a GameObject to access its other properties
                // Instantiate copies all of the scripts, meshes, etc attached to a prefab
                GameObject c = Instantiate(constellation, 
                    hit.point, 
                    constellation.transform.rotation);

                Constellation con = c.GetComponent<Constellation>();

                // pass to constellation the
                // 1) number of stars it has
                // 2) its GameObject shape
                con.numVertexStars = data.numVertexStars;
                con.numEdgeStars = data.numEdgeStars;
                con.constellationShape = constellation;

                constellationRenderer = con.GetComponentInChildren<Renderer>();

                // change constellation mesh to those stored in Assets/Meshes
                var path = "Assets/Meshes/" + data.quality.ToString("F1") + "/" + constellation.name + ".asset";
                con.GetComponentInChildren<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);

                // generates stars on mesh edge
                /*meshEdgePositions = edge.GetRandomPointsOnConstellationEdge(c, data.numEdgeStars);

                for (int i = 0; i < meshEdgePositions.Count; i++)
                {
                    GameObject s = Instantiate(EdgeStar, hit.point, Quaternion.identity);
                    s.transform.parent = c.transform;
                    EdgeStar edgeStar = s.GetComponent<EdgeStar>();
                    Vector3 edgePos = meshEdgePositions[i];

                    edgeStar.targetPos = edgePos;
                }

                meshVertexPositions = edge.GetRandomPointsOnConstellationVertices(c, data.numStars, data.vertexStarMinDistance);

                // generate stars on mesh vertices
                for (int i = 0; i < meshVertexPositions.Count; i++)
                {
                    GameObject s = Instantiate(Star, hit.point, Quaternion.identity);
                    s.transform.parent = c.transform;
                    Star star = s.GetComponent<Star>();

                    Vector3 vertexPos = meshVertexPositions[i];

                    star.targetPos = vertexPos;
                }/*

                // This block makes total number of stars on any location given mode
                /*for (int i = 0; i < data.numStars; i++)
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
                }*/

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
