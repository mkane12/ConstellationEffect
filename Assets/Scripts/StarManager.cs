using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamLab.Unity;

// Singleton pattern ensures a class has only a single globally accessible instance available at all times
// Useful for global management classes with global variables and functions accessible by other classes
// Info on singletons: https://wiki.unity3d.com/index.php/Singleton
public class StarManager : SingletonMonoBehaviour<StarManager> {

    // Create new class starManager with variables for these hardcoded numbers
    public float acceleration = -1.0f; // rate of deceleration of star
    
    protected override void Awake()
    {
        base.Awake();
    }

}
   
