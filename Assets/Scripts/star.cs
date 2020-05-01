using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Davis: maybe add variations in colors of stars
// TODO Davis: blend between frames in sprite sheet instead of jump

public class star : MonoBehaviour
{
    public Vector3 targetPos; // target position of the star
    public int size; // size of the star
    public float velocity; // velocity of the star
    public float lifespan = 30.0f; // number of seconds star lasts
    public float acceleration = -1.0f; // rate of deceleration of star
    public float fadeTime = 0.1f; // time for star to fade

    // these variables are for cycling through texture map
    // Reference: https://www.youtube.com/watch?v=cMiY6svKt-s
    // TODO Davis: Rather than have separate tex____ for texture variables, create new class textureData, textureHelper, whatever
    // > Adds uniformity to code; can add helper functions to class
    public int columns = 4;
    public int rows = 2;
    public int fps = 8;
    private int index;
    private float delay;
    private Vector2 tileSize;
    private Vector2 offset;

    private Renderer renderer;

    // called just once for script; first thing called once game object is created
    // ***still called even if script disabled
    void Awake()
    {
        // TODO Davis: create new class starProperties/starManager with variables for these hardcoded numbers
        // > as a brand new .cs file
        targetPos = new Vector3(Random.Range(-10.0f, 10.0f),
            Random.Range(-10.0f, 10.0f), transform.position.z - 1.0f);
        velocity = Random.Range(10.0f, 30.0f);
    }

    // Below only called if script enabled
    private void Start()
    {
        // make each star have a random size
        size = Random.Range(1, 3);
        transform.localScale *= size;

        // start index with random offset so twinkling is scattered
        delay = Random.Range(0.0f, 1.0f);

        // prep star to be destroyed
        // TODO Davis: generally prefer to avoid Coroutines
        // > If there are too many going on at once, it can cause problems
        // > Dangerous to keep calling Coroutines repeatedly - if a GO is destroyed, have to manually disable coroutines
        // > Look into State Machines
        // > Look into switch statements -> used with State Machines
        // > See the teamLab Unity Frameworks

        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO Davis: move some of this to helper function in new texture class
        // TODO Davis: don't call "new" in update functions; performance issue with cleaning up after that
        // calculate index for texture iteration
        // add random time offset, so stars twinkle at varying rates
        index = (int)(Time.time * fps * delay);
        index = index % (columns * rows);
        tileSize = new Vector2(1.0f / columns, 1.0f / rows);
        // split into horizontal and vertical indices
        var uIndex = index % rows;
        var vIndex = index / rows;
        // build offset
        // TODO Davis: how to update this without calling new
        offset = new Vector2(uIndex * tileSize.x,
            1.0f - tileSize.y - vIndex * tileSize.y);

        // TODO Davis: only pass new value to shader when it's changed rather than pass every frame
        // > cache previous value sent to shader, then check if new value is different before passing
        // TODO Davis: cache id number for shader instead of forcing it to look up id from string every time
        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", tileSize);

        float step = velocity * Time.deltaTime + acceleration * Mathf.Pow(Time.deltaTime, 2.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Time.deltaTime >= lifespan)
        {
            // TODO: Fade() is never called
            Fade();
            Destroy(gameObject);
        }
    }

    // function for star to fade away after lifetime
    void Fade()
    {
        Debug.Log("we're in Fade");
        var color = renderer.material.color;
        while (color.a >= 0)
        {
            renderer.material.color = new Color(color.r, color.g, color.b, color.a - 
                (fadeTime * (Time.deltaTime)));
            Debug.Log(color.a);
        }
        Destroy(gameObject);
    }
}