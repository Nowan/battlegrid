using System;
using UnityEngine;

[ExecuteInEditMode]
public class GridController : MonoBehaviour
{

    public TextAsset DataFile;
    public GameObject HightlightPrefab;

    public GridModel Model { get; private set; }
    public GridHighlighter Highlighter { get; private set; }
    public Vector2 TileSize { get { return _gridGenerator.TileSize; } }

    private GridGenerator _gridGenerator;

	private void Start ()
    {
        Invalidate();
        Highlighter = GetComponent<GridHighlighter>();
    }

    private void OnValidate()
    {
        Invalidate();
    }

    private void Invalidate()
    {
        if (DataFile)
        {
            Model = new GridModel(DataFile.text);

            _gridGenerator = GetComponent<GridGenerator>();
            _gridGenerator.Invalidate();
            _gridGenerator.RebuildMesh();
        }
        else
        {
            Debug.LogError("Grid data file is not assigned!");
        }
    }

    public Vector2 GetTileAtPoint(Vector3 point)
    {
        return new Vector2(Mathf.Floor(point.x / TileSize.x), Mathf.Floor(point.z / TileSize.y));
    }

    
}
