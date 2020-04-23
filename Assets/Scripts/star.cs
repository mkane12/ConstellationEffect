using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class star : MonoBehaviour
{
    public Vector3 initPos; // initial position of the star
    public Vector3 targetPos; // target position of the star
    public int size; // size of the star
    public float velocity = 100.0f; // initial velocity of the star

    // called just once for script
    void Awake()
    {
        initPos = transform.position;
        Debug.Log("initPos: " + initPos);
        targetPos = new Vector3(Random.Range(-5.0f, 5.0f), 
            Random.Range(-5.0f, 5.0f), 10);
        Debug.Log("targetPos: " + targetPos);
    }

    // Update is called once per frame
    void Update()
    {
        float step = velocity * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }
}