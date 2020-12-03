using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct StarData // data for each instance of a star
{
    public int idx; // star's unique id
    public int triangleIndex; // index for triangle star is in
    public Vector3 position; // star's position on mesh
    public float minVelocity; // minimum velocity of star getting to target position on mesh
    public float maxVelocity; // maximum velocity of star getting to target position on mesh
    public float minSize; // minimum size of star
    public float maxSize; // maximum size of star
    public int twinkleSpeed; // speed at which star twinkles
    public float lifespan; // star's lifespan
    public float timeToFade; // time over which star fades
    public Vector4 color; // star's color
    public float minDistance; // minimum distance btw stars (tbd if we need this)
}