using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class GridGenerator : MonoBehaviour
{
    public Vector2 TileSize = new Vector2(1, 1);

    private int _tilesetTileResolution = 256;

    private GridController _controller;
    private GameObject _terrainObj;

    private void OnValidate()
    {
        Invalidate();
        if(_controller && _controller.Model != null) RebuildMesh();
    }

    public void Invalidate()
    {
        _controller = GetComponent<GridController>();
        _terrainObj = transform.Find("Terrain").gameObject;
    }

    public void RebuildMesh()
    {
        var model = _controller.Model;
        int vertNum = model.Width * model.Height * 4;
        int triangleNum = Mathf.RoundToInt(vertNum * 0.5f);

        Material groundMat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/ground.mat", typeof(Material));
        float uvTileSizeX = 1 / ((float)groundMat.mainTexture.width / _tilesetTileResolution);
        float uvTileSizeY = 1 / ((float)groundMat.mainTexture.height / _tilesetTileResolution);
        int uvRowNum = (int)(1 / uvTileSizeX);

        Vector3[] vertices = new Vector3[vertNum];
        int[] triangles = new int[triangleNum * 3];
        Vector3[] normals = new Vector3[vertNum];
        Vector2[] uv = new Vector2[vertNum];

        for (int r = 0; r < model.Height; r++)
        {
            for (int c = 0; c < model.Width; c++)
            {
                int squareIndex = r * model.Width + c;
                int vertIndex = squareIndex * 4;
                int triangleIndex = squareIndex * 6;

                // Set up vertexes & normals
                vertices[vertIndex] = new Vector3(c * TileSize.x, 0, (model.Height - r - 1) * TileSize.y);
                vertices[vertIndex + 1] = new Vector3((c + 1) * TileSize.x, 0, (model.Height - r - 1) * TileSize.y);
                vertices[vertIndex + 2] = new Vector3(c * TileSize.x, 0, (model.Height - r) * TileSize.y);
                vertices[vertIndex + 3] = new Vector3((c + 1) * TileSize.x, 0, (model.Height - r) * TileSize.y);

                for (int i = 0; i < 4; i++)
                    normals[vertIndex + i] = Vector3.up;

                // Set up UV coordinates for vertexes 
                int ti = model.Layers["ground"].Tiles[r, c];
                int tiX = ti % uvRowNum;
                int tiY = uvRowNum - Mathf.FloorToInt((float)ti / uvRowNum) - 1;

                uv[vertIndex] = new Vector2(tiX * uvTileSizeX, tiY * uvTileSizeY);
                uv[vertIndex + 1] = new Vector2((tiX + 1) * uvTileSizeX, tiY * uvTileSizeY);
                uv[vertIndex + 2] = new Vector2(tiX * uvTileSizeX, (tiY + 1) * uvTileSizeY);
                uv[vertIndex + 3] = new Vector2((tiX + 1) * uvTileSizeX, (tiY + 1) * uvTileSizeY);

                // Set up triangles
                triangles[triangleIndex] = vertIndex;
                triangles[triangleIndex + 1] = vertIndex + 3;
                triangles[triangleIndex + 2] = vertIndex + 1;

                triangles[triangleIndex + 3] = vertIndex;
                triangles[triangleIndex + 4] = vertIndex + 2;
                triangles[triangleIndex + 5] = vertIndex + 3;
            }
        }

        Mesh mesh = new Mesh()
        {
            vertices = vertices,
            triangles = triangles,
            normals = normals,
            uv = uv
        };

        MeshFilter meshFilter = _terrainObj.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = _terrainObj.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = _terrainObj.GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.sharedMaterial = groundMat;
    }
}
