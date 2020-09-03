using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using System;
using TeamLab.Unity;

[System.Serializable]
public class Data : MonoBehaviour {

    public int numStars = 100; // number of stars to spawn

    public float minVelocity = 5.0f; // minimum velocity of a star
    public float maxVelocity = 20.0f; // maximum velocity of a star

    public float minSize = 0.5f; // minimum size of a star
    public float maxSize = 2.0f; // maximum size of a star

    public int twinkleSpeed = 8; // twinkle speed of stars

    public float lifespan = 5.0f; // number of seconds star lasts

    public float timeToFade = 5.0f; // time for star to fade

    //make public list of GameObjects for constellations
    // TODO: this exists in Unity, but can't store a GameObject in a setting file --> need to store its name, string, etc.
    // Data should always be able to stand alone - let another class access this data and update GameObject lists etc.
    public List<GameObject> constellationList = new List<GameObject>();

    public int constellationCount = 1; // number of constellations to spawn per click

    public float quality = 0.5f; // constant to determine mesh quality/complexity

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

    // Utilizes SharedVariableColor helper class in TeamLabUnityFrameworks
    // TODO: just store RGB values, move this fancy stuff back to script
    public ShaderVariableColor starColorUpdate = new ShaderVariableColor("_Color"); // star color

    public string DataToString()
    {
        string outputNumStars = this.numStars.ToString();
        string outputMinVelocity = this.minVelocity.ToString();
        string outputMaxVelocity = this.maxVelocity.ToString();

        string output = outputNumStars + "," + outputMinVelocity + "," + outputMaxVelocity;

        return output;
}

    // make it static to return a brand new instance of Data class
    public static Data StringToData(string input)
    {
        Data output = new Data();

        string [] values = input.Split(',');

        output.numStars = Int32.Parse(values[0]);
        output.minVelocity = float.Parse(values[1]);
        output.maxVelocity = float.Parse(values[2]);

        return output;
    }

}
