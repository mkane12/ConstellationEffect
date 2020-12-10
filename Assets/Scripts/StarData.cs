using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarData // data for each instance of a star
{
    public int idx; // star's unique id
    public int triangleIndex; // index for triangle star is in
    public Vector3 position; // star's position on mesh
    public float velocity; // velocity of star moving to position on mesh
    public float size; // size of star
    public int twinkleSpeed; // speed at which star twinkles
    public float lifespan; // star's lifespan
    public float timeToFade; // time over which star fades
    public string color; // star's HEX color
    public float minDistance; // minimum distance btw stars (tbd if we need this)
}