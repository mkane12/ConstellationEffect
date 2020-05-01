using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class star : MonoBehaviour
{
    public Vector3 targetPos; // target position of the star
    public int size; // size of the star
    public float velocity; // velocity of the star
    public float lifespan = 30.0f; // number of seconds star lasts
    public float acceleration = -1.0f; // rate of deceleration of star

    // these variables are for cycling through texture map
    // Reference: https://www.youtube.com/watch?v=cMiY6svKt-s
    public int columns = 4;
    public int rows = 2;
    public int fps = 5;
    private int index;
    private Vector2 tileSize;
    private Vector2 offset;
    private Renderer renderer;

    private void Start()
    {
        // make each star have a random size
        size = Random.Range(1, 3);
        transform.localScale *= size;
        renderer = GetComponent<Renderer>();
    }

    // called just once for script
    void Awake()
    {
        targetPos = new Vector3(Random.Range(-10.0f, 10.0f), 
            Random.Range(-10.0f, 10.0f), transform.position.z - 1.0f);
        velocity = Random.Range(10.0f, 30.0f);
        Destroy(gameObject, lifespan);
    }

    // Update is called once per frame
    void Update()
    {
        // calculate index for texture iteration
        // add random time offset, so starts twinkle at varying rates
        index = (int)(Time.time * fps) % (columns * rows);
        tileSize = new Vector2(1.0f / columns, 1.0f / rows);
        // split into horizontal and vertical indices
        var uIndex = index % rows;
        var vIndex = index / rows;
        // build offset
        offset = new Vector2(uIndex * tileSize.x,
            1.0f - tileSize.y - vIndex * tileSize.y);

        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", tileSize);

        float step = velocity * Time.deltaTime + acceleration * Mathf.Pow(Time.deltaTime, 2.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

    }
}