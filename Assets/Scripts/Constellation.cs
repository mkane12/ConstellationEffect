﻿using System;
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

    public int numVertexStars;
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

        vertexStarData = new StarData[numVertexStars];
        edgeStarData = new StarData[numEdgeStars];

        // generate stars on mesh
        for (int i = 0; i < numVertexStars; i++)
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
                this,
                (float)i / numVertexStars);
        }
        else // mesh is static
        {
            targetPosition = edge.GetRandomPointOnStaticConstellationMesh(
                this.GetComponent<MeshFilter>().sharedMesh,
                this,
                (float)i / numVertexStars);
        }

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

            targetPosition = edge.GetRandomPointOnAnimatedConstellationMesh(
                bakedMesh,
                this,
                (float)i / numEdgeStars);
        }
        else // mesh is static
        {
            targetPosition = edge.GetRandomPointOnStaticConstellationMesh(
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

        for(int i = 0; i < numVertexStars; i ++)
        {
            vertexStarData[i].position = edge.GetRandomPointOnAnimatedConstellationMesh(
                bakedMesh, 
                this, 
                (float)i / numVertexStars);
        }
        
        for (int i = 0; i < numEdgeStars; i++)
        {
            edgeStarData[i].position = edge.GetRandomPointOnAnimatedConstellationMesh(
                bakedMesh,
                this,
                (float)i / numEdgeStars);
        }
    }

}
