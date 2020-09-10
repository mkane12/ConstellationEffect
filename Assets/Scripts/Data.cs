﻿using System.Collections;
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

    // list of ConstellationTypes for constellations user is choosing to select from
    public List<Sky.ConstellationType> constellationNames = new List<Sky.ConstellationType>();

    public int constellationCount = 1; // number of constellations to spawn per click

    public float meshAlpha = 1.0f; // alpha for constellation mesh

    public float quality = 0.5f; // constant to determine mesh quality/complexity

    // list of ConstellationModes for how stars appear on constellation
    public Sky.ConstellationMode mode = Sky.ConstellationMode.Mesh;

    public string starColor = "#AAFFFF"; // hex star color

}
