using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamLab.Unity;

// Singleton pattern ensures a class has only a single globally accessible instance available at all times
// Useful for global management classes with global variables and functions accessible by other classes
// Info on singletons: https://wiki.unity3d.com/index.php/Singleton
public class StarManager : SingletonMonoBehaviour<StarManager> {

    // Create new class starProperties/starManager with variables for these hardcoded numbers
    // > as a brand new .cs file
    // > For your star manager class, I believe a singleton design pattern would be more appropriate than a static class.
    // > In team lab unity frameworks, there is a helpful base class for making singletons. SingletonMonoBehaviour.cs

    //public float velocity;
    public float lifespan = 5.0f; // number of seconds star lasts
    public float acceleration = -1.0f; // rate of deceleration of star
    public float timeToFade = 5.0f; // time for star to fade

    public float minVelocity = 5.0f; // minimum velocity of a star
    public float maxVelocity = 20.0f; // maximum velocity of a star

    public float minSize = 0.5f; // minimum size of a star
    public float maxSize = 2.0f; // maximum size of a star

    public int twinkleSpeed = 8; // rate at which stars twinkle
    
    protected override void Awake()
    {
        base.Awake();
    }

}
   
