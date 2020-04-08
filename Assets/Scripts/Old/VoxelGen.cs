using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGen : MonoBehaviour
{
    public Material material;
    public Transform viewer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewerPos = viewer.transform.position;

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                Cube cube = new Cube(new Vector3(x, 0, z), transform, material);

            }
        }
    }
}