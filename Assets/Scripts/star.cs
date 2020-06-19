using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamLab.Unity;

// TODO (visual) Davis: maybe add variations in colors of stars
// TODO (visual) Davis: blend between frames in sprite sheet instead of jump

// About StateMachine
// > protected variables accessible by subclasses -> Star has access to StateMachine
// > virtual functions = subclass can redefine function as needed

public class Star : StateMachine
{
    public Vector3 targetPos; // target position of the star
    public float size; // size of the star
    public float velocity; // velocity of the star
    private float initializationTime; // time when star was initialized
    private float timeInFade = 0f; // timer for star in Die state

    public enum StarState : int
    {
        Born = 1,
        Live = 2,
        Die = 3
    }

    private float delay;
    private Renderer renderer;
    private TextureHelper tex;
    
    // called just once for script; first thing called once game object is created
    // > still called even if script disabled
    void Awake()
    {
        // Tried to add this to singleton, but would make velocity same for all Stars (velocity should be unique to each Star)
        velocity = Random.Range(5.0f, 20.0f);//StarManager.Instance.velocity;

        // set initial state to born
        SetState((int)StarState.Born);
    }

    // Below only called if script enabled
    protected override void Start()
    {
        // make each star have a random size
        size = Random.Range(0.5f, 2.0f);
        transform.localScale *= size;

        initializationTime = Time.timeSinceLevelLoad; // establish time at which object was instantiated

        renderer = GetComponent<Renderer>();

        // start index with random offset so twinkling is scattered
        delay = Random.Range(0.0f, 1.0f);

        // helper method called for each star on instantiation
        tex = new TextureHelper();
        tex.NewStarTex(renderer, delay);
    }

    // called once per frame in StateMachine's Update() function
    protected override void StateUpdateCallback()
    {
        // Twinkle should happen regardless of state
        tex.Twinkle();

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
                }
                break;
            case (int)StarState.Live:
                // Live state: wait until lifespan is elapsed
                // once we are outside of lifespan, switch to Die state
                if (Time.timeSinceLevelLoad - initializationTime >= StarManager.Instance.lifespan)
                {
                    SetState((int)StarState.Die);
                }
                break;
            case (int)StarState.Die:
                UpdateDie();
                break;
        }
    }

    private void UpdateBorn()
    {
        float step = velocity * Time.deltaTime + StarManager.Instance.acceleration * Mathf.Pow(Time.deltaTime, 2.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    private void UpdateDie()
    {
        timeInFade += Time.deltaTime; // counter for time spent in this method

        // alpha goes from 1 to 0 over roughly timeToFade
        float alpha = Mathf.Lerp(1, 0, timeInFade / StarManager.Instance.timeToFade);

        // try to change material as alpha updates
        renderer.material.SetFloat("_Transparency", alpha);

        // if alpha is low enough, destroy the star
        if (alpha <= 0.01)
        {
            Destroy(this.gameObject);
        }
    }
}