using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sphere : MonoBehaviour
{

    public GameObject blockPrefab;
    public int chunkSize = 50;
    public float noiseScale = .05f;
    [Range(0, 1)]
    public float threshold = .5f;
    public Material material;
    public bool sphere = false;
    public int seed;
    private List<Mesh> meshes = new List<Mesh>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Destroy(GameObject.Find("World"));
            foreach (Mesh m in meshes)
                Destroy(m);
            Generate();
        }
    }

    public void Start()
    {
        Destroy(GameObject.Find("World"));
        foreach (Mesh m in meshes)
            Destroy(m);
        Generate();
    }

    private void Generate()
    {
        List<CombineInstance> blockData = new List<CombineInstance>();
        MeshFilter blockMesh = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000);
        float offsetY = prng.Next(-100000, 100000);
        float offsetZ = prng.Next(-100000, 100000);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {

                    float noiseValue = Perlin3D(x * noiseScale + offsetX, y * noiseScale + offsetY, z * noiseScale + offsetZ);
                    if (noiseValue >= threshold)
                    {
                        float raduis = chunkSize / 2;
                        if (sphere && Vector3.Distance(new Vector3(x, y, z), Vector3.one * raduis) > raduis)
                            continue;

                        blockMesh.transform.position = new Vector3(x, y, z);
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
            mr.material = material;
            mf.mesh.CombineMeshes(data.ToArray());
            meshes.Add(mf.mesh);
            //g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
        }

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