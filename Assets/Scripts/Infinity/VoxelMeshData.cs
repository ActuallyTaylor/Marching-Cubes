using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();

    int numFaces;

    public VoxelMeshData()
    {

    }

    public void calculateTriangles()
    {
        int tl = vertices.Count - 4 * numFaces;
        for (int i = 0; i < numFaces; i++)
        {
            triangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });

        }
    }

    public void drawFront(Vector3 pos)
    {
        vertices.Add(pos + new Vector3(0, 0, 0));
        vertices.Add(pos + new Vector3(0, 1, 0));
        vertices.Add(pos + new Vector3(1, 1, 0));
        vertices.Add(pos + new Vector3(1, 0, 0));

        numFaces++;
    }

    public void drawBack(Vector3 pos)
    {
        vertices.Add(pos + new Vector3(1, 0, 1));
        vertices.Add(pos + new Vector3(1, 1, 1));
        vertices.Add(pos + new Vector3(0, 1, 1));
        vertices.Add(pos + new Vector3(0, 0, 1));

        numFaces++;
    }

    public void drawTop(Vector3 pos)
    {
        vertices.Add(pos + new Vector3(0, 1, 0));
        vertices.Add(pos + new Vector3(0, 1, 1));
        vertices.Add(pos + new Vector3(1, 1, 1));
        vertices.Add(pos + new Vector3(1, 1, 0));

        numFaces++;
    }

    public void drawRight(Vector3 pos)
    {
        vertices.Add(pos + new Vector3(1, 0, 0));
        vertices.Add(pos + new Vector3(1, 1, 0));
        vertices.Add(pos + new Vector3(1, 1, 1));
        vertices.Add(pos + new Vector3(1, 0, 1));

        numFaces++;
    }

    public void drawLeft(Vector3 pos)
    {
        vertices.Add(pos + new Vector3(0, 0, 1));
        vertices.Add(pos + new Vector3(0, 1, 1));
        vertices.Add(pos + new Vector3(0, 1, 0));
        vertices.Add(pos + new Vector3(0, 0, 0));

        numFaces++;
    }

    public void drawBottom(Vector3 pos)
    {
        vertices.Add(pos + new Vector3(0, 0, 0));
        vertices.Add(pos + new Vector3(1, 0, 0));
        vertices.Add(pos + new Vector3(1, 0, 1));
        vertices.Add(pos + new Vector3(0, 0, 1));

        numFaces++;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        if (vertices.Count > 0)
        {
            calculateTriangles();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
        }

        return mesh;
    }
}
