using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Map2DGenerator : MonoBehaviour
{
    public Vector2 chunkSize;
    public int seed;
    public Vector2 offset;
    public int scale;
    public FastNoise.NoiseType NoiseType;
    public float frequency;

    [Range(-1, 1)]
    public float threshold;
    int chunkWidth;
    int chunkHeight;
    int chunkDepth;

    Queue<MapThreadInfo<Map2DData>> map2DDataThreadInfoQueue = new Queue<MapThreadInfo<Map2DData>>();
    Queue<MapThreadInfo<VoxelMeshData>> mesh2DDataThreadInfoQueue = new Queue<MapThreadInfo<VoxelMeshData>>();

    void Start()
    {
        chunkWidth = (int)chunkSize.x;
        chunkDepth = (int)chunkSize.y;
    }

    void Update()
    {
        if (map2DDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < map2DDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<Map2DData> threadInfo = map2DDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (mesh2DDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mesh2DDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<VoxelMeshData> threadInfo = mesh2DDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void request2DMapData(Action<Map2DData> callback, Vector2 center)
    {
        ThreadStart threadStart = delegate
        {
            map2DDataThread(callback, center);
        };

        new Thread(threadStart).Start();
    }

    void map2DDataThread(Action<Map2DData> callback, Vector2 center)
    {
        Map2DData mapData = generate2DMapData(center);
        lock (map2DDataThreadInfoQueue)
        {
            map2DDataThreadInfoQueue.Enqueue(new MapThreadInfo<Map2DData>(callback, mapData));
        }
    }

    public void request2DMeshData(Map2DData mapData, Action<VoxelMeshData> callback)
    {
        ThreadStart threadStart = delegate {
            mesh2DDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    void mesh2DDataThread(Map2DData mapData, Action<VoxelMeshData> callback)
    {
        VoxelMeshData meshData = CreateMesh(mapData.heightMap);
        lock (mesh2DDataThreadInfoQueue)
        {
            mesh2DDataThreadInfoQueue.Enqueue(new MapThreadInfo<VoxelMeshData>(callback, meshData));
        }
    }

    Map2DData generate2DMapData(Vector2 center)
    {
        FastNoise noise2D = new FastNoise();
        noise2D.SetNoiseType(NoiseType);
        noise2D.SetSeed(seed);
        noise2D.SetFrequency(frequency);

        float[,] heightMap = new float[chunkWidth, chunkDepth];

        List<CombineInstance> blockData = new List<CombineInstance>();
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkDepth; y++)
            {
                heightMap[x, y] = noise2D.GetNoise(x + center.x + offset.x, y + center.y + offset.y);
            }
        }
        return new Map2DData(heightMap);
    }


    public VoxelMeshData CreateMesh(float[,] heightMap)
    {
        VoxelMeshData meshData = new VoxelMeshData();

        //Evaluate heightmap
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int z = 0; z < chunkDepth; z++)
            {
                float heightValue = heightMap[x, z] * scale;

                Vector3 currPos = new Vector3(x, heightValue, z);
                meshData.drawTop(currPos);
                meshData.drawBack(currPos);
                meshData.drawFront(currPos);
                meshData.drawRight(currPos);
                meshData.drawLeft(currPos);
            }
        }
        return meshData;
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

public struct Map2DData
{
    public readonly float[,] heightMap;

    public Map2DData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}


/*
//Paste to use 3D noise 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Map2DGenerator : MonoBehaviour
{
    public Vector3 chunkSize;
    public int seed;
    public Vector3 offset;
    public int scale;

    [Range(-1, 1)]
    public float threshold;
    int chunkWidth;
    int chunkHeight;
    int chunkDepth;

    Queue<MapThreadInfo<Map2DData>> map2DDataThreadInfoQueue = new Queue<MapThreadInfo<Map2DData>>();
    Queue<MapThreadInfo<VoxelMeshData>> mesh2DDataThreadInfoQueue = new Queue<MapThreadInfo<VoxelMeshData>>();

    void Start()
    {
        chunkWidth = (int)chunkSize.x;
        chunkHeight = (int)chunkSize.y;
        chunkDepth = (int)chunkSize.z;
    }

    void Update()
    {
        if (map2DDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < map2DDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<Map2DData> threadInfo = map2DDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (mesh2DDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mesh2DDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<VoxelMeshData> threadInfo = mesh2DDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void request2DMapData(Action<Map2DData> callback, Vector2 center)
    {
        ThreadStart threadStart = delegate
        {
            map2DDataThread(callback, center);
        };

        new Thread(threadStart).Start();
    }

    void map2DDataThread(Action<Map2DData> callback, Vector2 center)
    {
        Map2DData mapData = generate2DMapData(center);
        lock (map2DDataThreadInfoQueue)
        {
            map2DDataThreadInfoQueue.Enqueue(new MapThreadInfo<Map2DData>(callback, mapData));
        }
    }

    public void request2DMeshData(Map2DData mapData, Action<VoxelMeshData> callback)
    {
        ThreadStart threadStart = delegate {
            mesh2DDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    void mesh2DDataThread(Map2DData mapData, Action<VoxelMeshData> callback)
    {
        VoxelMeshData meshData = CreateMesh(mapData.heightMap);
        lock (mesh2DDataThreadInfoQueue)
        {
            mesh2DDataThreadInfoQueue.Enqueue(new MapThreadInfo<VoxelMeshData>(callback, meshData));
        }
    }

    Map2DData generate2DMapData(Vector2 center)
    {
        FastNoise noise3D = new FastNoise();
        noise3D.SetNoiseType(FastNoise.NoiseType.Cubic);
        noise3D.SetSeed(seed);
        //noise3D.SetFrequency(0.05f);

        float[,,] heightMap = new float[chunkWidth + 2, chunkHeight + 2, chunkDepth + 2];

        for (int x = 0; x < chunkWidth + 2; x++)
        {
            for (int y = 0; y < chunkHeight + 2; y++)
            {
                for (int z = 0; z < chunkDepth + 2; z++)
                {
                    heightMap[x, y, z] = noise3D.GetNoise(x + center.x + offset.x, y + 0 + offset.y, z + center.y + offset.z);
                }
            }
        }
        return new Map2DData(heightMap);
    }

    public VoxelMeshData CreateMesh(float[,,] heightMap)
    {
        VoxelMeshData meshData = new VoxelMeshData();

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
                        Vector3 currPos = new Vector3(x - 1, y - 1, z - 1);
                        if (y <= chunkHeight && heightMap[x, y + 1, z] < threshold) //Top
                        {
                            meshData.drawTop(currPos);
                        }

                        if (y >= 0 && heightMap[x, y - 1, z] < threshold) //Bottom
                        {
                            meshData.drawBottom(currPos);
                        }

                        if (z <= chunkDepth && heightMap[x, y, z + 1] < threshold) //Back
                        {
                            meshData.drawBack(currPos);
                        }

                        if (z >= 0 && heightMap[x, y, z - 1] < threshold) //Front
                        {
                            meshData.drawFront(currPos);
                        }

                        if (x <= chunkWidth && heightMap[x + 1, y, z] < threshold) //Right
                        {
                            meshData.drawRight(currPos);
                        }

                        if (x >= 0 && heightMap[x - 1, y, z] < threshold) //left
                        {
                            meshData.drawLeft(currPos);
                        }
                    }
                }
            }
        }
        return meshData;
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

public struct Map2DData
{
    public readonly float[,,] heightMap;

    public Map2DData(float[,,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
*/