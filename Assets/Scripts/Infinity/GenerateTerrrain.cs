using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

public class GenerateTerrrain: MonoBehaviour
{
    public Vector3 chunkSize;
    public int seed = 0;
    public Vector3 offset;
    [Range(-1, 1)]
    public float threshold;
    public Material material;

    int chunkWidth;
    int chunkHeight;
    int chunkDepth;

    Queue<MapThreadInfo<mData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<mData>>();
    Queue<MapThreadInfo<VoxelMeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<VoxelMeshData>>();

    void Start()
    {
        chunkWidth = (int)chunkSize.x;
        chunkHeight = (int)chunkSize.y;
        chunkDepth = (int)chunkSize.z;

    }
    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<mData> threadInfo = mapDataThreadInfoQueue.Dequeue();
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


    mData generateMData(Vector3 center) {
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
        return new mData(heightMap);
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
                        Vector3 center = new Vector3(x, y, z);
                        if (y < chunkHeight - 1 && heightMap[x, y + 1, z] < threshold) //Top
                        {
                            meshData.drawTop(center);
                        }
                        else if (y == chunkHeight - 1)
                        {
                            meshData.drawTop(center);
                        }

                        if (y > 0 && heightMap[x, y - 1, z] < threshold) //Bottom
                        {
                            meshData.drawBottom(center);
                        }
                        else if (y == 0)
                        {
                            meshData.drawBottom(center);
                        }

                        if (z < chunkDepth - 1 && heightMap[x, y, z + 1] < threshold) //Back
                        {
                            meshData.drawBack(center);
                        }
                        else if (z == chunkDepth - 1)
                        {
                            meshData.drawBack(center);
                        }

                        if (z > 0 && heightMap[x, y, z - 1] < threshold) //Front
                        {
                            meshData.drawFront(center);
                        }
                        else if (z == 0)
                        {
                            meshData.drawFront(center);
                        }

                        if (x < chunkWidth - 1 && heightMap[x + 1, y, z] < threshold) //Right
                        {
                            meshData.drawRight(center);
                        }
                        else if (x == chunkWidth - 1)
                        {
                            meshData.drawRight(center);
                        }

                        if (x > 0 && heightMap[x - 1, y, z] < threshold) //left
                        {
                            meshData.drawLeft(center);
                        }
                        else if (x == 0)
                        {
                            meshData.drawLeft(center);
                        }
                    }
                }
            }
        }
        return meshData;
    }

    public void RequestVMapData(Vector3 center, Action<mData> callback)
    {
        ThreadStart threadStart = delegate {
            VMapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void VMapDataThread(Vector3 center, Action<mData> callback)
    {
        mData mData = generateMData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<mData>(callback, mData));
        }
    }

    public void RequestVMeshData(mData mData, Action<VoxelMeshData> callback)
    {
        ThreadStart threadStart = delegate {
            VMeshDataThread(mData,callback);
        };

        new Thread(threadStart).Start();
    }

    void VMeshDataThread(mData mData, Action<VoxelMeshData> callback)
    {
        VoxelMeshData meshData = CreateMesh(mData.heightMap);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<VoxelMeshData>(callback, meshData));
        }
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

public struct mData
{
    public readonly float[,,] heightMap;

    public mData(float[,,] heightMap)
    {
        this.heightMap = heightMap;
    }
}