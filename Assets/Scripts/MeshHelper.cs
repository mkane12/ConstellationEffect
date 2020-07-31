using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Following Thread on edges
// > https://answers.unity.com/questions/1019436/get-outeredge-vertices-c.html

// And this thread on instantiating prefabs on mesh surface:
// > https://answers.unity.com/questions/52155/instantiate-a-prefab-along-the-surface-of-a-mesh.html

// Adapted from this github code:
// > https://gist.github.com/v21/5378391

public class MeshHelper
{

    public GameObject GetRandomConstellation(List<GameObject> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);

        GameObject ConstellationShape = list[index];

        return ConstellationShape;
    }

    public Vector3 GetRandomPointOnConstellationMesh(GameObject Constellation)
    {
        Debug.Log("Mesh Mode");
        MeshFilter mf = (MeshFilter)Constellation.GetComponent("MeshFilter");
        Mesh mesh = mf.sharedMesh;

        float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }

        //so everything above this point wants to be factored out

        float randomsample = Random.value * total;

        int triIndex = -1;

        for (int i = 0; i < sizes.Length; i++)
        {
            if (randomsample <= cumulativeSizes[i])
            {
                triIndex = i;
                break;
            }
        }

        if (triIndex == -1) Debug.LogError("triIndex should never be -1");

        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        //generate random barycentric coordinates

        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }
        //and then turn them back to a Vector3

        // BUT this doesn't account for Mesh's scale, position, or rotation
        Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);

        // scale by scale
        pointOnMesh = Vector3.Scale(pointOnMesh, Constellation.transform.localScale);

        // rotate by rotation
        pointOnMesh = Constellation.transform.rotation * pointOnMesh;

        // translate by position
        pointOnMesh = pointOnMesh + Constellation.transform.position;

        return pointOnMesh;

    }

    public Vector3 GetRandomPointOnConstellationEdge(GameObject Constellation)
    {
        Debug.Log("Edge Mode");
        MeshFilter mf = (MeshFilter)Constellation.GetComponent("MeshFilter");
        Mesh mesh = mf.sharedMesh;

        float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }

        //so everything above this point wants to be factored out

        float randomsample = Random.value * total;

        int triIndex = -1;

        for (int i = 0; i < sizes.Length; i++)
        {
            if (randomsample <= cumulativeSizes[i])
            {
                triIndex = i;
                break;
            }
        }

        if (triIndex == -1) Debug.LogError("triIndex should never be -1");

        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        //generate random barycentric coordinates

        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }
        //and then turn them back to a Vector3

        // BUT this doesn't account for Mesh's scale, position, or rotation
        Vector3 pointOnMesh = a + b + c;// a + r * (b - a) + s * (c - a);

        // scale by scale
        pointOnMesh = Vector3.Scale(pointOnMesh, Constellation.transform.localScale);

        // rotate by rotation
        pointOnMesh = Constellation.transform.rotation * pointOnMesh;

        // translate by position
        pointOnMesh = pointOnMesh + Constellation.transform.position;

        return pointOnMesh;

    }

    float[] GetTriSizes(int[] tris, Vector3[] verts)
    {
        int triCount = tris.Length / 3;
        float[] sizes = new float[triCount];
        for (int i = 0; i < triCount; i++)
        {
            sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
        }
        return sizes;
    }
}
