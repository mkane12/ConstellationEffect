using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour {

    public GameObject star;
    public int numStars = 12; // number of stars to spawn

    void Start()
    {
    }

    // call on click
    void OnMouseDown()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            for (int i = 0; i < numStars; i++)
            {
                Instantiate(star, hit.point, Quaternion.identity);
            }
        }

    }
}
