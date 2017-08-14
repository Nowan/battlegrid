using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class GridGenerator : MonoBehaviour {

    public TextAsset dataFile;
    public Vector2 tileSize = new Vector2(1, 1);

    private GridModel _gridData;
    private Vector2 _gridSize;
    private int _tilesetTileResolution = 256;

    public Vector2 GetTileAt(Vector3 point)
    {
        return new Vector2(Mathf.Floor(point.x / tileSize.x), Mathf.Floor(point.z / tileSize.y));
    }

	private void Start () {
        Invalidate();
    }

    private void OnValidate()
    {
        Invalidate();
    }

    private void Invalidate()
    {
        if (dataFile)
        {
            _gridData = new GridModel(dataFile.text);
            _gridSize = new Vector2(_gridData.Width * tileSize.x, _gridData.Height * tileSize.y);
            BuildMesh();
        }
        else
        {
            Debug.LogError("Grid data file is not assigned!");
        }
    }

    private void BuildMesh()
    {
        int vertNum = _gridData.Width * _gridData.Height * 4;
        int triangleNum = Mathf.RoundToInt(vertNum * 0.5f);

        Material groundMat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/ground.mat", typeof(Material));
        float uvTileSizeX = 1 / ((float)groundMat.mainTexture.width / _tilesetTileResolution);
        float uvTileSizeY = 1 / ((float)groundMat.mainTexture.height / _tilesetTileResolution);
        int uvRowNum = (int)(1 / uvTileSizeX);

        Vector3[] vertices = new Vector3[vertNum];
        int[] triangles = new int[triangleNum * 3];
        Vector3[] normals = new Vector3[vertNum];
        Vector2[] uv = new Vector2[vertNum];

        for(int r = 0; r < _gridData.Height; r++)
        {
            for(int c = 0; c < _gridData.Width; c++)
            {
                int squareIndex = r * _gridData.Width + c;
                int vertIndex = squareIndex * 4;
                int triangleIndex = squareIndex * 6;

                // Set up vertexes & normals
                vertices[vertIndex] = new Vector3(c * tileSize.x, 0, (_gridData.Height - r - 1) * tileSize.y);
                vertices[vertIndex + 1] = new Vector3((c + 1) * tileSize.x, 0, (_gridData.Height - r - 1) * tileSize.y);
                vertices[vertIndex + 2] = new Vector3(c * tileSize.x, 0, (_gridData.Height - r) * tileSize.y);
                vertices[vertIndex + 3] = new Vector3((c + 1) * tileSize.x, 0, (_gridData.Height - r) * tileSize.y);

                for (int i = 0; i < 4; i++)
                    normals[vertIndex + i] = Vector3.up;

                // Set up UV coordinates for vertexes 
                int ti = _gridData.Layers["ground"].Tiles[r, c];
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

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.sharedMaterial = groundMat;
    }
}
