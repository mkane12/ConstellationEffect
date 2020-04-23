using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class star : MonoBehaviour
{
    public Vector3 targetPos; // target position of the star
    public int size; // size of the star
    public float velocity; // velocity of the star
    public float lifespan = 30.0f; // number of seconds star lasts
    public float acceleration = -0.5f; // rate of deceleration of star

    // called just once for script
    void Awake()
    {
        targetPos = new Vector3(Random.Range(-10.0f, 10.0f), 
            Random.Range(-10.0f, 10.0f), transform.position.z);
        velocity = Random.Range(10.0f, 30.0f);
        Destroy(gameObject, lifespan);
    }

    // Update is called once per frame
    void Update()
    {
        float step = velocity * Time.deltaTime 
            + acceleration * Mathf.Pow(Time.deltaTime, 2.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

    }
}