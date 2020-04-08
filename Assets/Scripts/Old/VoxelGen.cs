using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGen : MonoBehaviour
{
    public Material material;
    public Transform viewer;

    public bool drawTop;
    public bool drawBottom;
    public bool drawFront;
    public bool drawBack;
    public bool drawLeft;
    public bool drawRight;
    GameObject gCube;

    // Start is called before the first frame update
    void Start()
    {
        drawCube();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Destroy(gCube);
            drawCube();
        }
    }

    public void drawCube()
    {
        Cube cube = new Cube();
        gCube = new GameObject("Cube");
        gCube.transform.parent = transform;
        gCube.transform.position = transform.position;

        if (drawTop)
        {
            cube.drawTop();
        }
        if (drawBottom)
        {
            cube.drawBottom();
        }
        if (drawFront)
        {
            cube.drawFront();
        }
        if (drawBack)
        {
            cube.drawBack();
        }
        if (drawLeft)
        {
            cube.drawLeft();
        }
        if (drawRight)
        {
            cube.drawRight();
        }
        if (cube.getVerts().Count > 0)
        {
            cube.calculateTriangles();

        }
        MeshFilter meshFilter = gCube.AddComponent<MeshFilter>();
        MeshRenderer mr = gCube.AddComponent<MeshRenderer>();
        gCube.transform.position = transform.position;

        mr.material = material;
        meshFilter.mesh.vertices = cube.getVerts().ToArray();
        meshFilter.mesh.triangles = cube.getTriangles().ToArray();
        meshFilter.mesh.RecalculateNormals();
    }
}