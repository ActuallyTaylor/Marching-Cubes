using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessVoxel : MonoBehaviour
{

    public const float maxViewDst = 50;
    public Transform viewer;

    public static Vector3 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDst;

    static GenerateTerrrain mapGenerator;

    Dictionary<Vector3, VoxelChunk> voxelChunkDictionary = new Dictionary<Vector3, VoxelChunk>();
    List<VoxelChunk> voxelChunksVisibleLastUpdate = new List<VoxelChunk>();

    private void Start()
    {
        mapGenerator = FindObjectOfType<GenerateTerrrain>();
        chunkSize = (int) mapGenerator.chunkSize.x;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }

    private void Update()
    {
        viewerPosition = new Vector3(viewer.position.x, viewer.position.y, viewer.position.z);
        updateVisibleChunks();
    }

    void updateVisibleChunks()
    {
        for(int i = 0; i < voxelChunksVisibleLastUpdate.Count; i ++)
        {
            voxelChunksVisibleLastUpdate[i].SetVisible(false);
        }
        voxelChunksVisibleLastUpdate.Clear();

        int currentChunkCordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        int currentChunkCordZ = Mathf.RoundToInt(viewerPosition.z / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                for (int zOffset = -chunksVisibleInViewDst; zOffset <= chunksVisibleInViewDst; zOffset++)
                {
                    Vector3 viewedChunkCord = new Vector3(currentChunkCordX + xOffset, currentChunkCordY + yOffset, currentChunkCordZ + zOffset); 
                    if (voxelChunkDictionary.ContainsKey(viewedChunkCord))
                    {
                        voxelChunkDictionary[viewedChunkCord].UpdateTerrainChunk();
                        if (voxelChunkDictionary[viewedChunkCord].isVisible())
                        {
                            voxelChunksVisibleLastUpdate.Add(voxelChunkDictionary[viewedChunkCord]);
                        }
                    }
                    else
                    {
                        voxelChunkDictionary.Add(viewedChunkCord, new VoxelChunk(viewedChunkCord, chunkSize, transform));
                    }
                }
            }
        }
    }

    public class VoxelChunk
    {
        GameObject meshObject;
        Vector3 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public VoxelChunk(Vector3 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 positionV3 = new Vector3(position.x, position.z, position.y);

            meshObject = new GameObject("Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();

            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one;
            meshObject.transform.parent = parent;

            meshObject.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);
            meshObject.name = " X: " + coord.x * 16 + " Y: " + coord.y * 16 + " Z: " + coord.z * 16;
            SetVisible(false);

            mapGenerator.RequestVMapData(position, OnMapDataReceived);
        }

        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }

        void OnMapDataReceived(mData mData)
        {
            mapGenerator.RequestVMeshData(mData, OnMeshDataReceived);
        }

        void OnMeshDataReceived(VoxelMeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }
    }
}
