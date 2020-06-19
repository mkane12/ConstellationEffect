using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour {

    public GameObject Star;
    public int numStars = 1; // number of stars to spawn
    private MeshHelper edge;

    //make public list of GameObjects for constellations
    public List<GameObject> constellationList = new List<GameObject>();
    public GameObject Ursa;
    public GameObject Leo;
    public GameObject Tiger;

    void Start()
    {
        // Get random position on mesh
        edge = new MeshHelper();

        // Add GameObjects to constellationList
        constellationList.Add(Ursa);
        constellationList.Add(Leo);
        constellationList.Add(Tiger);
    }

    // call on click
    void OnMouseDown()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {

            GameObject ConstellationShape = edge.GetRandomConstellation(constellationList);

            // instantiate new constellation
            GameObject c = Instantiate(ConstellationShape, 
               hit.point, //new Vector3(hit.point.x, hit.point.y, hit.point.z - 0.1f), 
               ConstellationShape.transform.rotation) 
               as GameObject;

            for (int i = 0; i < numStars; i++)
            {
                GameObject s = Instantiate(Star, hit.point, Quaternion.identity);
                Star star = s.GetComponent<Star>();

                // multiply by scale of constellation shape, move by transform, then rotate
                Vector3 meshPos = edge.GetRandomPointOnConstellation(c);

                star.targetPos = meshPos;
            }

            Destroy(c);
        }

    }
}
