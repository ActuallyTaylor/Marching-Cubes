using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    bool created = false;

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
