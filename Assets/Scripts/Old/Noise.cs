using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public static float[,,] Generate3DNoiseMap(int chunkSize, int seed, float scale, int octaves, float persistance, float lacurnarity, Vector3 offset)
    {
        float[,,] noiseMap = new float[chunkSize, chunkSize, chunkSize];

        System.Random prng = new System.Random(seed);
        Vector3[] octaveOffsets = new Vector3[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) - offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            float offsetZ = prng.Next(-100000, 100000) - offset.z;
            octaveOffsets[i] = new Vector3(offsetX, offsetY, offsetZ);
            print("X: " + offsetX + " Y: " + offsetY + " Z " + offsetZ);
        }

        if (scale <= 0)
            scale = 0.0001f;

        float maxNoiseDensity = float.MinValue;
        float minNoiseDensity = float.MaxValue;

        for (int y = 0; y < chunkSize; y++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseDensity = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x + seed)  / scale * frequency;
                        float sampleY = (y + seed)  / scale * frequency;
                        float sampleZ = (z + seed)  / scale * frequency;

                        float perlinValue = Perlin3D(sampleX, sampleY, sampleZ);

                        noiseDensity = perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacurnarity;

                    }

                    if (noiseDensity > maxNoiseDensity)
                        maxNoiseDensity = noiseDensity;
                    if (noiseDensity < minNoiseDensity)
                        minNoiseDensity = noiseDensity;


                    noiseMap[x, y, z] = noiseDensity;
                }
            }
        }

        for (int y = 0; y < chunkSize; y++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    noiseMap[x, y, z] = Mathf.InverseLerp(minNoiseDensity, maxNoiseDensity, noiseMap[x, y, z]);
                }
            }
        }

        return noiseMap;
    }

    public static float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}

/*
 *  public static float[,] Generate2DNoiseMap(int chunkSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector3 offset)
    {
        float[,] noiseMap = new float[chunkSize, chunkSize];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        float maxLocalNoiseDensity = float.MinValue;
        float minLocalNoiseDensity = float.MaxValue;

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseDensity = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (z + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseDensity += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseDensity > maxLocalNoiseDensity)
                {
                    maxLocalNoiseDensity = noiseDensity;
                }
                if (noiseDensity < minLocalNoiseDensity)
                {
                    minLocalNoiseDensity = noiseDensity;
                }
                noiseMap[x, z] = noiseDensity;
            }
        }



        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                float density;
                int height = Mathf.RoundToInt(noiseMap[x, z] * chunkSize);
                for(int y = 0; y < chunkSize; y++)
                {
                    if(z < height)
                    {
                        density = 0;
                    } else if(z == height)
                    {
                        density = .51f;
                    } else
                    {
                        density = 1;
                    }
                    //noiseMap3D[x, y, z] = density;
                }
            }
        }


        return noiseMap;
    }
*/