using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Generator : MonoBehaviour
{
    public enum DimensionEnum { Noise3D, Noise2D };
    public enum NoiseTypes {Value, ValueFractal, Perlin, PerlinFractal, Simplex, SimplexFractal, Cellular, WhiteNoise, Cubic, CubicFractal };

    public Vector3 chunkSize;
    public Vector3 regionSize;
    public int seed;
    public Vector3 offset;
    [Range(-1, 1)]
    public float threshold;
    public Material material;
    public bool drawDebugSqaures;
    public DimensionEnum DimensionType;
    public NoiseTypes NoiseType;
    public int scale2D;
    public bool removeFullChunks;

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
        for(int j = 1; j <= regionSize.x; j ++)
        {
            for (int i = 1; i <= regionSize.y; i++)
            {
                for (int k = 1; k <= regionSize.z; k++)
                {
                    position = new Vector3(chunkWidth * j, chunkHeight * i, chunkDepth * k);
                    if(DimensionType == DimensionEnum.Noise3D)
                    {
                        load3DMesh();
                    } else if(DimensionType == DimensionEnum.Noise2D)
                    {
                        load2Dmesh();
                    }
                }
            }
        }

    }

    void Update()
    {

    }

    void load3DMesh()
    {
        FastNoise noise3D = new FastNoise();
        noise3D.SetNoiseType(getNoiseType());
        noise3D.SetSeed(seed);
        float[,,] heightMap = new float[chunkWidth + 2, chunkHeight + 2, chunkDepth + 2];
        bool lessThenThreshold = false;

        List<CombineInstance> blockData = new List<CombineInstance>();
        //Load heightmap
        for (int x = 0; x < chunkWidth + 2; x++)
        {
            for (int y = 0; y < chunkHeight + 2; y++)
            {
                for (int z = 0; z < chunkDepth + 2; z++)
                {
                    heightMap[x, y, z] = noise3D.GetNoise(x + position.x + offset.x, y + position.y + offset.y, z + position.z + offset.z);
                }
            }
        }

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkDepth; z++)
                {
                    if(heightMap[x,y,z] < threshold)
                    {
                        lessThenThreshold = true;
                    }
                }
            }
        }

        //Evaluate heightmap
        for (int x = 1; x < chunkWidth + 2; x++)
        {
            for (int y = 1; y < chunkHeight + 2; y++)
            {
                for (int z = 1; z < chunkDepth + 2; z++)
                {
                    float densityValue = heightMap[x, y, z];
                    if (densityValue >= threshold)
                    {
                        Cube cube = new Cube();
                        if (y <= chunkHeight && heightMap[x, y + 1, z] < threshold) //Top
                        {
                            cube.drawTop();
                        }

                        if (y >= 0 && heightMap[x, y - 1, z] < threshold) //Bottom
                        {
                            cube.drawBottom();
                        }

                        if (z <= chunkDepth && heightMap[x, y, z + 1] < threshold) //Back
                        {
                            cube.drawBack();
                        }

                        if (z >= 0 && heightMap[x, y, z - 1] < threshold) //Front
                        {
                            cube.drawFront();
                        }

                        if (x <= chunkWidth && heightMap[x + 1, y, z] < threshold) //Right
                        {
                            cube.drawRight();
                        }

                        if (x >= 0 && heightMap[x - 1, y, z] < threshold) //left
                        {
                            cube.drawLeft();
                        }

                        if (cube.getVerts().Count > 0)
                        {
                            GameObject gCube = new GameObject("SubCube");
                            cube.calculateTriangles();
                            MeshFilter meshFilter = gCube.AddComponent<MeshFilter>();
                            MeshRenderer mr = gCube.AddComponent<MeshRenderer>();
                            gCube.transform.position = new Vector3(x - 1 + position.x, y - 1 + position.y, z - 1 + position.z);

                            meshFilter.mesh.vertices = cube.getVerts().ToArray();
                            meshFilter.mesh.triangles = cube.getTriangles().ToArray();
                            CombineInstance ci = new CombineInstance
                            {
                                mesh = gCube.GetComponent<MeshFilter>().mesh,
                                transform = gCube.transform.localToWorldMatrix,
                            };
                            blockData.Add(ci);
                            Destroy(gCube);
                        }
                    }
                    else
                    {
                        if (drawDebugSqaures)
                        {
                            GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            testCube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            testCube.GetComponent<MeshRenderer>().material.color = Color.gray;
                            testCube.transform.position = new Vector3(x + position.x + 0.5f, y + position.y + 0.5f, z + position.z + 0.5f);
                        }
                    }
                }
            }
        }


      
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

        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject("Chunk");
            g.transform.parent = transform;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.material = mr.material = material;
            mf.mesh.CombineMeshes(data.ToArray());
            mf.mesh.RecalculateNormals();
            //g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
        }
    }

    void load2Dmesh()
    {
        FastNoise noise2D = new FastNoise();
        noise2D.SetNoiseType(getNoiseType());
        noise2D.SetSeed(seed);

        float[,] heightMap = new float[chunkWidth, chunkDepth];

        List<CombineInstance> blockData = new List<CombineInstance>();
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkDepth; y++)
            {
                heightMap[x, y] = noise2D.GetNoise(x + position.x + offset.x, y + position.z + offset.y);
            }
        }

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkDepth; y++)
            {
                Cube cube = new Cube();
                cube.drawFullCube();

                GameObject gCube = new GameObject("SubCube");
                cube.calculateTriangles();
                MeshFilter meshFilter = gCube.AddComponent<MeshFilter>();
                MeshRenderer mr = gCube.AddComponent<MeshRenderer>();
                float heightValue = heightMap[x, y] * scale2D;

                gCube.transform.position = new Vector3(x + position.x, heightValue, y + position.z);

                meshFilter.mesh.vertices = cube.getVerts().ToArray();
                meshFilter.mesh.triangles = cube.getTriangles().ToArray();
                CombineInstance ci = new CombineInstance
                {
                    mesh = gCube.GetComponent<MeshFilter>().mesh,
                    transform = gCube.transform.localToWorldMatrix,
                };
                blockData.Add(ci);
                Destroy(gCube);
            }
        }

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

        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject("Chunk");
            g.transform.parent = transform;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.material = mr.material = material;
            mf.mesh.CombineMeshes(data.ToArray());
            mf.mesh.RecalculateNormals();
            //g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
        }
    }

    FastNoise.NoiseType getNoiseType()
    {
        if (NoiseType == NoiseTypes.Cellular)
        {
            return FastNoise.NoiseType.Cellular;
        }
        else if (NoiseType == NoiseTypes.Cubic)
        {
            return FastNoise.NoiseType.Cubic;
        }
        else if (NoiseType == NoiseTypes.CubicFractal)
        {
            return FastNoise.NoiseType.CubicFractal;
        }
        else if (NoiseType == NoiseTypes.Perlin)
        {
            return FastNoise.NoiseType.Perlin;
        }
        else if (NoiseType == NoiseTypes.PerlinFractal)
        {
            return FastNoise.NoiseType.PerlinFractal;
        }
        else if (NoiseType == NoiseTypes.Simplex)
        {
            return FastNoise.NoiseType.Simplex;
        }
        else if (NoiseType == NoiseTypes.SimplexFractal)
        {
            return FastNoise.NoiseType.SimplexFractal;
        }
        else if (NoiseType == NoiseTypes.Value)
        {
            return FastNoise.NoiseType.Value;
        }
        else if (NoiseType == NoiseTypes.ValueFractal)
        {
            return FastNoise.NoiseType.ValueFractal;
        }
        else if (NoiseType == NoiseTypes.WhiteNoise)
        {
            return FastNoise.NoiseType.WhiteNoise;
        }
        else
        {
            return FastNoise.NoiseType.Perlin;
        }
    }
}
/*
 * if (y < chunkHeight - 1 && heightMap[x, y + 1, z] < threshold) //Top
                            {
                                cube.drawTop();
                            }
                            else if (y == chunkHeight - 1)
                            {
                                cube.drawTop();
                            }

                            if (y > 0 && heightMap[x, y - 1, z] < threshold) //Bottom
                            {
                                cube.drawBottom();
                            }
                            else if (y == 0)
                            {
                                cube.drawBottom();
                            }

                            if (z < chunkDepth - 1 && heightMap[x, y, z + 1] < threshold) //Back
                            {
                                cube.drawBack();
                            }
                            else if (z == chunkDepth - 1)
                            {
                                cube.drawBack();
                            }

                            if (z > 0 && heightMap[x, y, z - 1] < threshold) //Front
                            {
                                cube.drawFront();
                            }
                            else if (z == 0)
                            {
                                cube.drawFront();
                            }

                            if (x < chunkWidth - 1 && heightMap[x + 1, y, z] < threshold) //Right
                            {
                                cube.drawRight();
                            }
                            else if (x == chunkWidth - 1)
                            {
                                cube.drawRight();
                            }

                            if (x > 0 && heightMap[x - 1, y, z] < threshold) //left
                            {
                                cube.drawLeft();
                            }
                            else if (x == 0)
                            {
                                cube.drawLeft();
                            }
*/