using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> tris = new List<int>();

    int numFaces;

    public Cube ()
    {

    }

    public void createFullCube(Vector3 pos, Transform parent, Material material)
    {/*
        GameObject cube = new GameObject("Cube - X: " + pos.x + " Y: " + pos.y + " Z: " + pos.z);
        cube.transform.parent = parent;
        cube.transform.position = pos;
        meshFilter = cube.AddComponent<MeshFilter>();
        drawCube();

        MeshRenderer mr = cube.AddComponent<MeshRenderer>();
        mr.material = material;
        cube.transform.position = pos;
        */
    }

    public List<Vector3> getVerts()
    {
        return vertices;
    }

    public List<int> getTriangles()
    {
        return tris;
    }

    public void drawFullCube()
    {
        drawFront();
        drawLeft();
        drawRight();
        drawBack();
        drawTop();
        drawBottom();
    }

    public void calculateTriangles()
    {
        int tl = vertices.Count - 4 * numFaces;
        for (int i = 0; i < numFaces; i++)
        {
            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });

        }
    }

    public void drawLarge()
    {
        vertices.Add(new Vector3(16, 0, 16));
        vertices.Add(new Vector3(16, 16, 16));
        vertices.Add(new Vector3(0, 16, 16));
        vertices.Add(new Vector3(0, 0, 16));
    }

    public void drawFront()
    {
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 1, 0));
        vertices.Add(new Vector3(1, 0, 0));

        numFaces ++;
    }

    public void drawBack()
    {
        vertices.Add(new Vector3(1, 0, 1));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(0, 0, 1));

        numFaces ++;
    }

    public void drawTop()
    {
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(1, 1, 0));

        numFaces++;
    }

    public void drawRight()
    {
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 1, 0));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(1, 0, 1));

        numFaces++;
    }

    public void drawLeft()
    {
        vertices.Add(new Vector3(0, 0, 1));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(0, 0, 0));

        numFaces++;
    }

    public void drawBottom()
    {
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 0, 1));
        vertices.Add(new Vector3(0, 0, 1));

        numFaces++;
    }
}