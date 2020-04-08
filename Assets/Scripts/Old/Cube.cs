using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube
{

    MeshFilter meshFilter;

    List<Vector3> vertices = new List<Vector3>();
    List<int> tris = new List<int>();

    int numFaces;

    public Cube (Vector3 pos, Transform parent, Material material)
    {
        GameObject cube = new GameObject("Cube - X: " + pos.x + " Y: " + pos.y + " Z: " + pos.z);
        cube.transform.parent = parent;
        cube.transform.position = pos;
        meshFilter = cube.AddComponent<MeshFilter>();
        drawCube();

        MeshRenderer mr = cube.AddComponent<MeshRenderer>();
        mr.material = material;
        cube.transform.position = pos;
    }

    public List<Vector3> getVerts()
    {
        drawCube();
        return vertices;
    }

    public List<int> getTriangles()
    {
        drawCube();
        return tris;
    }

    void drawCube()
    {
        Mesh mesh = new Mesh();
        drawFront();
        drawLeft();
        drawRight();
        drawBack();
        drawTop();
        drawBottom();

        calculateTriangles();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();

        meshFilter.mesh = mesh;
        mesh.RecalculateNormals();
    }

    void calculateTriangles()
    {
        int tl = vertices.Count - 4 * numFaces;
        for (int i = 0; i < numFaces; i++)
        {
            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });

        }
    }

    void drawFront()
    {
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 1, 0));
        vertices.Add(new Vector3(1, 0, 0));

        numFaces ++;
    }

    void drawBack()
    {
        vertices.Add(new Vector3(1, 0, 1));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(0, 0, 1));

        numFaces ++;
    }

    void drawTop()
    {
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(1, 1, 0));

        numFaces++;
    }

    void drawRight()
    {
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 1, 0));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(1, 0, 1));

        numFaces++;
    }

    void drawLeft()
    {
        vertices.Add(new Vector3(0, 0, 1));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(0, 0, 0));

        numFaces++;
    }

    void drawBottom()
    {
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 0, 1));
        vertices.Add(new Vector3(0, 0, 1));

        numFaces++;
    }
}