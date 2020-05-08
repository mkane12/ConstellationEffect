using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamLab.Unity;

// TODO Davis: maybe add variations in colors of stars
// TODO Davis: blend between frames in sprite sheet instead of jump

// About StateMachine
// > protected variables accessible by subclasses -> Star has access to StateMachine
// > virtual functions = subclass can redefine function as needed

public class Star : StateMachine
{
    public Vector3 targetPos; // target position of the star
    public int size; // size of the star
    public float velocity; // velocity of the star
    public float lifespan = 5.0f; // number of seconds star lasts
    public float acceleration = -1.0f; // rate of deceleration of star
    public float fadeTime = 1.0f; // time for star to fade
    private float initializationTime; // time when star was initialized

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

    public enum StarState : int
    {
        Born = 1,
        Live = 2,
        Die = 3
    }

    private Renderer renderer;

    // called just once for script; first thing called once game object is created
    // > still called even if script disabled
    void Awake()
    {
        // TODO Davis: create new class starProperties/starManager with variables for these hardcoded numbers
        // > as a brand new .cs file
        targetPos = new Vector3(Random.Range(-10.0f, 10.0f),
            Random.Range(-10.0f, 10.0f), transform.position.z - 1.0f);
        velocity = Random.Range(10.0f, 30.0f);

        // set initial state to born
        SetState((int)StarState.Born);
        Debug.Log("in born state");
    }

    // Below only called if script enabled
    protected override void Start()
    {
        // make each star have a random size
        size = Random.Range(1, 3);
        transform.localScale *= size;

        // start index with random offset so twinkling is scattered
        delay = Random.Range(0.0f, 1.0f);

        initializationTime = Time.timeSinceLevelLoad; // establish time at which object was instantiated

        renderer = GetComponent<Renderer>();
    }

    // called once per frame in StateMachine's Update() function
    protected override void StateUpdateCallback()
    {
        // Twinkle should happen regardless of state

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

        // call method depending on current state
        switch (GetStateID())
        {
            case (int)StarState.Born:
                // Born state: move from initPos to targetPos
                UpdateBorn();
                // check if current position is close enough to target position
                if (Vector3.Distance(transform.position, targetPos) <= 0.01)
                {
                    // if it's close enough, exit born state, enter live state
                    SetState((int)StarState.Live);
                    Debug.Log("in live state");
                }
                break;
            case (int)StarState.Live:
                // Live state: wait until lifespan is elapsed
                // once we are outside of lifespan
                if (Time.timeSinceLevelLoad - initializationTime >= lifespan)
                {
                    SetState((int)StarState.Die);
                    Debug.Log("in die state");
                }
                break;
            case (int)StarState.Die:
                UpdateDie();
                break;
        }
    }

    private void UpdateBorn()
    {
        float step = velocity * Time.deltaTime + acceleration * Mathf.Pow(Time.deltaTime, 2.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    private void UpdateDie()
    {
        float alpha = Mathf.Lerp(1, 0, 
            (Time.timeSinceLevelLoad - initializationTime) / fadeTime);
        // need better way to calculate alpha
        Debug.Log(alpha);
        renderer.material.color = new Color(
            renderer.material.color.r,
            renderer.material.color.g,
            renderer.material.color.b,
            alpha);
        if(renderer.material.color.a <= 0.01)
        {
            Destroy(gameObject);
        }
    }
}