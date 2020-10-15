using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public GameObject GetRandomConstellation(Dictionary<Sky.ConstellationType, GameObject> list, List<Sky.ConstellationType> names)
    {

        if(names.Count != 0)
        {
            var randomKey = names[(int)Random.Range(0, names.Count)];

            return list[randomKey];
        }
        else
        {
            Debug.Log("Toggle at least one constellation!");
            return list[0];
        }
        
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


    // TODO: pass in mesh instead of gameobject for these functions - makes more sense
    // TODO: scale/rotate/transform outside of this script
    // >> MeshHelper script shouldn't know about larger game
    public Vector3 GetRandomPointOnConstellationMesh(GameObject Constellation)
    {
        Mesh mesh = Constellation.GetComponent<MeshFilter>().sharedMesh;

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
        // > shifts from model to world space
        pointOnMesh = pointOnMesh + Constellation.transform.position;

        return pointOnMesh;

    }

    // gets a game object mesh and returns a list of Vector3 positions for stars on mesh edge
    public List<Vector3> GetRandomPointsOnConstellationEdge(Mesh mesh, int numStars)
    {
        // this gives a list of Edge structs, which have 2 vertex components
        var boundary = GetEdges(mesh.triangles);
        List<Vector3> pointsOnEdge = new List<Vector3>();

        for (int i = 0; i < numStars; i++)
        {
            // get a random triangle from the mesh
            int triIndex = GetRandomTriangle(mesh);

            Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
            Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];

            Vector3 line = b - a;
            Vector3 edgePos = a + Random.value * line;

            pointsOnEdge.Add(edgePos);
        }

        return pointsOnEdge;
    }

    // gets a game object mesh and returns a list of Vector3 positions for stars on mesh vertices
    public List<Vector3> GetRandomPointsOnConstellationVertices(Mesh mesh, int numStars, float minDist)
    {
        List<Vector3> pointsOnVertices = new List<Vector3>();

        for (int i = 0; i < numStars; i++)
        {
            int iteration = Mathf.Max(mesh.vertices.Length / (numStars + 1), 1);
            int index = (i + 1) * iteration % mesh.vertices.Length;

            // stop calculating positions if number of vertices is exceeded
            /*if(index > mesh.vertices.Length)
            {
                break;
            }*/

            Vector3 vertexPos = mesh.vertices[index];

            // add the latest position now - we will remove it later if it is too close to other stars
            pointsOnVertices.Add(vertexPos);

            // iterate through previous vertices to check that distances exceed a threshold
            // > note we iterate through all but the latest added star, since that dist = 0
            for (int j = 0; j < pointsOnVertices.Count - 1; j++)
            {
                float dist = Vector3.Distance(vertexPos, pointsOnVertices[j]);
                // if distance between new star and any old stars doesn't exceed a threshold, remove the latest star
                if(dist < minDist)
                {
                    pointsOnVertices.Remove(vertexPos);
                    break;
                }
            }
            
        }

        return pointsOnVertices;
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
