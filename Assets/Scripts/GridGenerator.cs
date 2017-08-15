using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class GridGenerator : MonoBehaviour {

    public TextAsset dataFile;
    public Vector2 tileSize = new Vector2(1, 1);
    public GameObject hightlightPrefab;

    private GridModel _gridData;
    private Vector2 _gridSize;
    private int _tilesetTileResolution = 256;

    private int _hlLength = 1;
    private bool _isHlVertical = false;
    private GameObject _highlighter;

    public Vector2 GetTileAt(Vector3 point)
    {
        return new Vector2(Mathf.Floor(point.x / tileSize.x), Mathf.Floor(point.z / tileSize.y));
    }

    public Vector3 GetHighlighterCenter(Vector2 tile)
    {
        float hlen = Mathf.Floor(_hlLength * .5f);
        float odd = _hlLength % 2 == 0 ? 0 : 1;

        // Apply restrictions to row & col - prevents object going outside grid boundaries
        float posX = _isHlVertical ? tile.x : Mathf.Min(Mathf.Max(tile.x, hlen), _gridData.Width - hlen - odd);
        float posY = _isHlVertical ? Mathf.Min(Mathf.Max(tile.y, hlen), _gridData.Height - hlen - odd) : tile.y;

        posX *= tileSize.x;
        posY *= tileSize.y;

        return new Vector3(posX, 0, posY);
    }

    public void SetHighlighter(int length, bool isVertical)
    {
        _hlLength = length;
        _isHlVertical = isVertical;
        if (_highlighter) InstantiateHighlighter(); // refresh highlighter if already displayed
    }

    public void HighlightTile(int rowIndex, int colIndex) { HighlightTile(new Vector2(colIndex, rowIndex)); }

    public void HighlightTile(Vector2 tile)
    {
        if (_highlighter)
        {
            var highlighterPos = GetHighlighterCenter(tile);
            float hlen = Mathf.Floor(_hlLength * .5f);

            highlighterPos.x -= _isHlVertical ? 0 : hlen * tileSize.x;
            highlighterPos.y = .01f;
            highlighterPos.z -= _isHlVertical ? hlen * tileSize.y : 0;

            _highlighter.transform.localPosition = highlighterPos;
        }
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

    private void InstantiateHighlighter()
    {
        if (_highlighter) Destroy(_highlighter);
        if (_hlLength <= 0) return;

        _highlighter = new GameObject("Highlighter");
        _highlighter.transform.parent = transform.parent;
        _highlighter.transform.localScale = new Vector3(1, 1, 1);
        _highlighter.transform.localPosition = new Vector3(0, 0, 0);

        if (hightlightPrefab)
        {
            for (int i = 0; i < _hlLength; i++)
            {
                var instance = Instantiate(hightlightPrefab, new Vector3(), Quaternion.identity, _highlighter.transform);
                instance.transform.localPosition = _isHlVertical ? new Vector3(0, 0, i * tileSize.y) : new Vector3(i * tileSize.x, 0, 0);
            }
        }
        else
            Debug.LogError("Highlight object is not provided");
    }
    
    public void OnMouseEnter()
    {
        InstantiateHighlighter();
    }

    public void OnMouseExit()
    {
        if (_highlighter) Destroy(_highlighter);
    }
}
