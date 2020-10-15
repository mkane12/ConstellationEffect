using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using System;
using TeamLab.Unity;

[System.Serializable]
public class Data : MonoBehaviour {

    public int numStars = 100; // number of stars to spawn on vertices
    public int numEdgeStars = 50; // number of stars to spawn on edges

    public float minVelocity = 5.0f; // minimum velocity of a vertex star
    public float maxVelocity = 20.0f; // maximum velocity of a vertex star
    public float minVelocityEdge = 5.0f; // minimum velocity of a vertex star
    public float maxVelocityEdge = 20.0f; // maximum velocity of a vertex star

    public float minSize = 0.5f; // minimum size of a vertex star
    public float maxSize = 2.0f; // maximum size of a vertex star
    public float minSizeEdge = 2.0f; // minimum size of an edge star
    public float maxSizeEdge = 3.0f; // maximum size of an edge star

    public int twinkleSpeed = 8; // twinkle speed of vertex stars
    public int twinkleSpeedEdge = 3; // twinkle speed of edge stars

    public float lifespan = 5.0f; // number of seconds star lasts

    public float timeToFade = 5.0f; // time for star to fade

    // list of ConstellationTypes for constellations user is choosing to select from
    public List<Sky.ConstellationType> constellationNames = new List<Sky.ConstellationType>();

    public int constellationCount = 1; // number of constellations to spawn per click

    public float meshAlpha = 1.0f; // alpha for constellation mesh

    public float quality = 0.5f; // constant to determine mesh quality/complexity

    // list of ConstellationModes for how stars appear on constellation
    //public Sky.ConstellationMode mode = Sky.ConstellationMode.Mesh;

    // float to determine percentage of stars to send to edge rather than vertex
    public float percentEdge = 0.9f;

    public string starColor = "#AAFFFF"; // hex star color
    public string edgeStarColor = "#AAFFFF"; // hex edge star color

    public float vertexStarMinDistance = 0.1f; // minimum distance btw vertex stars

}
