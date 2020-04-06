using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int chunkSize;
    public int seed;
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public Vector3 offset;
    [Range(0, 1)]
    public float threshold;

    // Start is called before the first frame update
    void Start()
    {
        createMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createMesh()
    {
        float[,,] map3D = Noise.Generate3DNoiseMap(chunkSize, seed, scale, octaves, persistance, lacunarity, offset);

        for (int x = 0; x < chunkSize; x++)
        {
            for(int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    float densityValue = map3D[x, y, z];
                    if (densityValue >= threshold)
                    {
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.transform.localScale = new Vector3(1f, 1f, 1f);
                        block.transform.parent = transform;
                        block.transform.position = new Vector3(x + transform.position.x, y + transform.position.y, z + transform.position.z);
                    }
                }
            }
        }
    }
}