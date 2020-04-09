using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class MapGenerator : MonoBehaviour
{
    public Vector3 chunkSize;
    public int seed;
    public Vector3 offset;
    [Range(-1,1)]
    public float threshold;
    int chunkWidth;
    int chunkHeight;
    int chunkDepth;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<VoxelMeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<VoxelMeshData>>();

    void Start()
    {
        chunkWidth = (int) chunkSize.x;
        chunkHeight = (int)chunkSize.y;
        chunkDepth = (int)chunkSize.z;

    }

    void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<VoxelMeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void requestMapData(Action<MapData> callback, Vector3 center)
    {
        ThreadStart threadStart = delegate
        {
            mapDataThread(callback, center);
        };

        new Thread(threadStart).Start();
    }

    void mapDataThread(Action<MapData> callback, Vector3 center)
    {
        MapData mapData = generateMapData(center);
        lock(mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void requestMeshData(MapData mapData, Action<VoxelMeshData> callback)
    {
        ThreadStart threadStart = delegate {
            meshDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    void meshDataThread(MapData mapData, Action<VoxelMeshData> callback)
    {
        VoxelMeshData meshData = CreateMesh(mapData.heightMap);
        lock(meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<VoxelMeshData>(callback, meshData));
        }
    }

    MapData generateMapData(Vector3 center)
    {
        FastNoise noise3D = new FastNoise();
        noise3D.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        noise3D.SetSeed(seed);
        float[,,] heightMap = new float[chunkWidth, chunkHeight, chunkDepth];

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkDepth; z++)
                {
                    heightMap[x, y, z] = noise3D.GetNoise(x + center.x + offset.x, y + center.y + offset.y, z + center.z + offset.z);
                }
            }
        }
        return new MapData(heightMap);
    }


    public VoxelMeshData CreateMesh(float[,,] heightMap)
    {
        VoxelMeshData meshData = new VoxelMeshData();

        //Evaluate heightmap
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkDepth; z++)
                {
                    float densityValue = heightMap[x, y, z];
                    if (densityValue >= threshold)
                    {
                        Vector3 currPos = new Vector3(x, y, z);
                        if (y < chunkHeight - 1 && heightMap[x, y + 1, z] < threshold) //Top
                        {
                            meshData.drawTop(currPos);
                        }
                        else if (y == chunkHeight - 1)
                        {
                            meshData.drawTop(currPos);
                        }

                        if (y > 0 && heightMap[x, y - 1, z] < threshold) //Bottom
                        {
                            meshData.drawBottom(currPos);
                        }
                        else if (y == 0)
                        {
                            meshData.drawBottom(currPos);
                        }

                        if (z < chunkDepth - 1 && heightMap[x, y, z + 1] < threshold) //Back
                        {
                            meshData.drawBack(currPos);
                        }
                        else if (z == chunkDepth - 1)
                        {
                            meshData.drawBack(currPos);
                        }

                        if (z > 0 && heightMap[x, y, z - 1] < threshold) //Front
                        {
                            meshData.drawFront(currPos);
                        }
                        else if (z == 0)
                        {
                            meshData.drawFront(currPos);
                        }

                        if (x < chunkWidth - 1 && heightMap[x + 1, y, z] < threshold) //Right
                        {
                            meshData.drawRight(currPos);
                        }
                        else if (x == chunkWidth - 1)
                        {
                            meshData.drawRight(currPos);
                        }

                        if (x > 0 && heightMap[x - 1, y, z] < threshold) //left
                        {
                            meshData.drawLeft(currPos);
                        }
                        else if (x == 0)
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

public struct MapData
{
    public readonly float[,,] heightMap;

    public MapData(float[,,] heightMap)
    {
        this.heightMap = heightMap;
    }
}