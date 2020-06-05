using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour {

    public GameObject Star;
    public int numStars = 12; // number of stars to spawn
    private EdgeHelper edge;
    private Mesh mesh;

    //make public list of GameObjects for constellations
    List<GameObject> constellationList = new List<GameObject>();
    public GameObject Ursa;
    public GameObject Leo;
    public GameObject Tiger;

    void Start()
    {
        // Get random position on mesh
        edge = new EdgeHelper();

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
            int index = UnityEngine.Random.Range(0, constellationList.Count - 1);

            GameObject ConstellationShape = constellationList[index];

            // instantiate new constellation
            GameObject c = Instantiate(ConstellationShape, 
               new Vector3(hit.point.x, hit.point.y, hit.point.z - 0.1f), 
               ConstellationShape.transform.rotation);

            MeshFilter mf = (MeshFilter)c.GetComponent("MeshFilter");
            mesh = mf.sharedMesh;

            for (int i = 0; i < numStars; i++)
            {
                GameObject s = Instantiate(Star, hit.point, Quaternion.identity);
                Star star = s.GetComponent<Star>();

                // multiply by scale of constellation shape, move by transform, then rotate
                Vector3 meshPos = edge.GetRandomPointOnMesh(mesh);

                // scale by scale
                meshPos = Vector3.Scale(meshPos, c.transform.localScale);

                // rotate by rotation
                meshPos = c.transform.rotation * meshPos;
                
                // transform by position
                meshPos = meshPos + c.transform.position;

                star.targetPos = meshPos;
            }
        }

    }
}
