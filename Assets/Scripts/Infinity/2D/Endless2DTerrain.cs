using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endless2DTerrain : MonoBehaviour
{
    public const float maxViewDist = 250;
    public Transform viewer;
    public Material material;

    public static Vector3 viewerPosition;
    static Map2DGenerator mapGenerator;

    int chunkSize = 16;
    int chunksVisibleInViewDst; // Chunks to render around the player

    Dictionary<Vector3, chunk2D> chunkDictionary = new Dictionary<Vector3, chunk2D>();
    List<chunk2D> chunksVisibleLastUpdate = new List<chunk2D>();

    void Start()
    {
        mapGenerator = FindObjectOfType<Map2DGenerator>();
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        updateVisibleChunks();
    }

    void updateVisibleChunks()
    {
        for (int i = 0; i < chunksVisibleLastUpdate.Count; i++)
        {
            chunksVisibleLastUpdate[i].SetVisible(false);
        }
        chunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
        {
            for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (chunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    chunkDictionary[viewedChunkCoord].UpdateChunk();
                    if (chunkDictionary[viewedChunkCoord].isVisible())
                    {
                        chunksVisibleLastUpdate.Add(chunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    chunkDictionary.Add(viewedChunkCoord, new chunk2D(viewedChunkCoord, chunkSize, transform, material));
                }
            }
        }
    }

    public class chunk2D
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public chunk2D(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 posV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = posV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            mapGenerator.request2DMapData(onMapDataRecieved, position);
        }

        public void UpdateChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool isVisible = viewerDstFromNearestEdge <= maxViewDist;
            SetVisible(isVisible);
        }

        public void SetVisible(bool Visible)
        {
            meshObject.SetActive(Visible);
        }
        public bool isVisible()
        {
            return meshObject.activeSelf;
        }

        void onMapDataRecieved(Map2DData mapData)
        {
            mapGenerator.request2DMeshData(mapData, onMeshDataReceived);
        }

        void onMeshDataReceived(VoxelMeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }
    }
}