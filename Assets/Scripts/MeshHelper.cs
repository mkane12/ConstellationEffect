using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Following Thread on edges
// > https://answers.unity.com/questions/1019436/get-outeredge-vertices-c.html
// >> Didn't use FindBoundary or SortEdges functions because (1) I don't think I care about shared edges and (2) I don't care about having a single path

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

    // new structure for Edges of a mesh
    public struct Edge
    {
        public int v1;
        public int v2;
        public int triangleIndex;
        public Edge(int aV1, int aV2, int aIndex)
        {
            v1 = aV1;
            v2 = aV2;
            triangleIndex = aIndex;
        }
    }

    public Vector3 GetRandomPointOnConstellationMesh(GameObject Constellation)
    {
        MeshFilter mf = (MeshFilter)Constellation.GetComponent("MeshFilter");
        Mesh mesh = mf.sharedMesh;

        // get random triangle in mesh
        int triIndex = GetRandomTriangle(mesh);

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
        MeshFilter mf = (MeshFilter)Constellation.GetComponent("MeshFilter");
        Mesh mesh = mf.sharedMesh;

        // this gives a list of Edge structs, which have 2 vertex components
        var boundary = GetEdges(mesh.triangles);

        // get a random triangle from the mesh
        int a = GetRandomTriangle(mesh);



        Vector3 pointOnEdge = new Vector3(boundary[a].v1, boundary[a].v2, 0);

        return pointOnEdge;

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

    int GetRandomTriangle(Mesh m)
    {
        // gets list of sizes of mesh triangles
        float[] sizes = GetTriSizes(m.triangles, m.vertices);

        // list of sum of triangle sizes
        // so if index is 1, that's the size of the first triangle. If 2, it's the sum of the first two, etc.
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        // calculate sum of triangle sizes
        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }


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

        return triIndex;
    }

    public static List<Edge> GetEdges(int[] aIndices)
    {
        List<Edge> result = new List<Edge>();
        for (int i = 0; i < aIndices.Length; i += 3)
        {
            int v1 = aIndices[i];
            int v2 = aIndices[i + 1];
            int v3 = aIndices[i + 2];
            result.Add(new Edge(v1, v2, i));
            result.Add(new Edge(v2, v3, i));
            result.Add(new Edge(v3, v1, i));
        }
        return result;
    }
}
