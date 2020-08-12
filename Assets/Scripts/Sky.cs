﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour {

    public GameObject Star;
    public int numStars = 100; // number of stars to spawn
    private MeshHelper edge;
    private Vector3 meshPos;

    //make public list of GameObjects for constellations
    public List<GameObject> constellationList = new List<GameObject>();
    public GameObject Ursa;
    public GameObject Leo;
    public GameObject Tiger;

    // public bool to see if user wants to show constellation mesh
    public bool toggleMeshVisibility;

    // time when constellation was initialized
    private float constellationInitializationTime;

    // make public enum to determine what visual mode constellation will use
    // Mesh = stars go to random point on constellation mesh
    // Edge = stars go to random point on constellation edge
    public enum ConstellationMode
    {
        Mesh,
        Edge,
        Vertex
    }

    // Set constellationMode as Mesh to start
    public ConstellationMode mode = ConstellationMode.Mesh;

    void Start()
    {
        // Get random position on mesh
        edge = new MeshHelper();

        constellationList.Add(Ursa);
        constellationList.Add(Leo);
        constellationList.Add(Tiger);
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

            GameObject ConstellationShape = edge.GetRandomConstellation(constellationList);

            Renderer constellationRenderer = ConstellationShape.GetComponent<Renderer>();

            Debug.Log(toggleMeshVisibility);
            // if user wants to make constellation mesh visible
            if (toggleMeshVisibility)
            {
                // make constellation mesh visible
                constellationRenderer.enabled = true;
            }
            else
            {
                // make constellation mesh invisible
                constellationRenderer.enabled = false;
            }

            // instantiate new constellation
            GameObject c = Instantiate(ConstellationShape, 
               hit.point, 
               ConstellationShape.transform.rotation) 
               as GameObject;
            
            for (int i = 0; i < numStars; i++)
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
            Destroy(c, StarManager.Instance.lifespan + StarManager.Instance.timeToFade + 1.0f);
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


}
