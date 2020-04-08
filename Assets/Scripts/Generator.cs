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
    public Material material;

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
        //MeshFilter blockMesh = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();
        Transform container = new GameObject("World").transform;
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkDepth; z++)
                {
                    heightMap[x, y, z] = myNoise.GetNoise(x + position.x + offset.x, y + position.y + offset.y, z + position.z + offset.z);
                }
            }
        }

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkDepth; z++)
                {
                    float densityValue = heightMap[x, y, z];
                    if (densityValue >= threshold)
                    {
                        Cube cube = new Cube();
                        if(y == chunkHeight - 1 || x == chunkWidth - 1 || z == chunkDepth - 1)
                        {
                            cube.drawFullCube();
                        } else
                        {
                            if (y < chunkHeight - 1 && heightMap[x, y + 1, z] < threshold) //Top
                            {
                                cube.drawTop();
                            }
                            if (y > 0 && heightMap[x, y - 1, z] < threshold) //Bottom
                            {
                                cube.drawBottom();
                            }
                            if (z < chunkDepth - 1 && heightMap[x, y, z + 1] < threshold) //Back
                            {
                                cube.drawBack();
                            }
                            if (z > 0 && heightMap[x, y, z - 1] < threshold) //Front
                            {
                                cube.drawFront();
                            }
                            if (x < chunkWidth - 1 && heightMap[x + 1, y, z] < threshold) //Right
                            {
                                cube.drawRight();
                            }
                            if (x > 0 && heightMap[x - 1, y, z] < threshold) //left
                            {
                                cube.drawLeft();
                            }
                        }


                        if (cube.getVerts().Count > 0)
                        {
                            GameObject gCube = new GameObject("SubCube");
                            gCube.transform.parent = container;
                            cube.calculateTriangles();
                            MeshFilter meshFilter = gCube.AddComponent<MeshFilter>();
                            MeshRenderer mr = gCube.AddComponent<MeshRenderer>();
                            gCube.transform.position = new Vector3(x + position.x, y + position.y, z + position.z);

                            mr.material = material;
                            meshFilter.mesh.vertices = cube.getVerts().ToArray();
                            meshFilter.mesh.triangles = cube.getTriangles().ToArray();
                            meshFilter.mesh.RecalculateNormals();
                        }


                        /*                        blockMesh.transform.position = new Vector3(x + position.x, y + position.y, z + position.z);
                                                CombineInstance ci = new CombineInstance
                                                {
                                                    mesh = blockMesh.sharedMesh,
                                                    transform = blockMesh.transform.localToWorldMatrix,
                                                };
                                                blockData.Add(ci);
                         */
                    } else
                    {
                        //GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //testCube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        //testCube.GetComponent<MeshRenderer>().material.color = Color.gray;
                        //testCube.transform.position = new Vector3(x + position.x + 0.5f, y + position.y + 0.5f, z + position.z + 0.5f);
                    }
                }
            }
        }

        /*
         * Destroy(blockMesh.gameObject);

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
        */
    }
}