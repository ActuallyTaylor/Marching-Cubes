using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Generator : MonoBehaviour
{
    public Vector3 chunkSize;
    public int seed;
    public Vector3 offset;
    [Range(-1, 1)]
    public float threshold;

    int chunkWidth;
    int chunkHeight;
    int chunkDepth;
    Vector3 position;

    void Start()
    {
        chunkWidth = (int) chunkSize.x;
        chunkHeight = (int)chunkSize.y;
        chunkDepth = (int)chunkSize.z;

        //For testing position will be a custom value,  but in actualy runs it should be turned into the position of the chunk
        //position = transform.position;
        position = new Vector3(0, 0, 0);
        fastNoiseMesh();
        position = new Vector3(chunkWidth, 0, 0);
        fastNoiseMesh();
        position = new Vector3(chunkWidth, chunkHeight, 0);
        fastNoiseMesh();
        position = new Vector3(0, chunkHeight, 0);
        fastNoiseMesh();
    }

    void Update()
    {

    }

    void fastNoiseMesh()
    {
        FastNoise myNoise = new FastNoise();
        myNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        myNoise.SetSeed(seed);
        float[,,] heightMap = new float[chunkWidth, chunkHeight, chunkDepth];

        List<CombineInstance> blockData = new List<CombineInstance>();
        MeshFilter blockMesh = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkDepth; z++)
                {
                    heightMap[x, y, z] = myNoise.GetNoise(x + position.x + offset.x, y + position.y + offset.y, z + position.z + offset.z);
                    float densityValue = heightMap[x, y, z];
                    if (densityValue >= threshold)
                    {
                        blockMesh.transform.position = new Vector3(x + position.x, y + position.y, z + position.z);
                        CombineInstance ci = new CombineInstance
                        {
                            mesh = blockMesh.sharedMesh,
                            transform = blockMesh.transform.localToWorldMatrix,
                        };
                        blockData.Add(ci);
                    }
                }
            }
        }
        Destroy(blockMesh.gameObject);

        List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());
        for (int i = 0; i < blockData.Count; i++)
        {
            vertexCount += blockData[i].mesh.vertexCount;
            if (vertexCount > 65536)
            {
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            }
            else
            {
                blockDataLists.Last().Add(blockData[i]);
            }
        }

        Transform container = new GameObject("World").transform;
        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject("Chunk");
            g.transform.parent = container;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.material = blockMesh.GetComponent<MeshRenderer>().material;
            mf.mesh.CombineMeshes(data.ToArray());
            //g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
        }
    }
}
/*
 * 
    void createMesh()
    {
        float[,,] map = Noise.Generate3DNoiseMap(chunkSize, seed, scale, octaves, persistance, lacunarity, offset + transform.position);

        List<CombineInstance> blockData = new List<CombineInstance>();
        MeshFilter blockMesh = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();
        for (int x = 0; x < chunkSize; x++)
        {
            for(int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    float densityValue = map[x, y, z];
                    if (densityValue >= threshold)
                    {
                        blockMesh.transform.position = new Vector3(x + transform.position.x, y + transform.position.y, z + transform.position.z);
                        CombineInstance ci = new CombineInstance
                        {
                            mesh = blockMesh.sharedMesh,
                            transform = blockMesh.transform.localToWorldMatrix,
                        };
                        blockData.Add(ci);
                    }
                }
            }
        }
        Destroy(blockMesh.gameObject);

        List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());
        for (int i = 0; i < blockData.Count; i++)
        {
            vertexCount += blockData[i].mesh.vertexCount;
            if (vertexCount > 65536)
            {
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            }
            else
            {
                blockDataLists.Last().Add(blockData[i]);
            }
        }

        Transform container = new GameObject("World").transform;
        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject("Chunk");
            g.transform.parent = container;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.material = blockMesh.GetComponent<MeshRenderer>().material;
            mf.mesh.CombineMeshes(data.ToArray());
            //g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
        }
        transform.position = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(chunkSize, 0, 0), 10000 * Time.deltaTime);
        if(created == false)
        {
            created = true;
            createMesh();
        }
    }
}
*/