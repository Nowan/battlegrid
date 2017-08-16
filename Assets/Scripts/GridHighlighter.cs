using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHighlighter : MonoBehaviour
{

    public GameObject RegularHighlight;
    public GameObject AccentHighlight;
    public GameObject BuildingHighlight;

    private int _length = 1;
    private bool _isVertical = false;
    private GameObject _highlighter;

    private GridController _controller;
    private Vector2 _tileSize;
    private Transform _highlightLayer;

    private void Start()
    {
        _controller = GetComponent<GridController>();
        _tileSize = _controller.TileSize;
        _highlightLayer = transform.Find("HighlightLayer");
    }

    private void OnMouseExit()
    {
        if (_highlighter) Destroy(_highlighter);
    }

    public void SetShape(int length, bool isVertical)
    {
        _length = length;
        _isVertical = isVertical;

        // Refresh highlighter if already displayed
        if (_highlighter) InstantiateHighlighter();
    }

    public void HighlightTileAtPoint(Vector3 localPoint)
    {
        var targetTile = _controller.GetTileAtPoint(localPoint);
        HighlightTile(targetTile);
    }

    public void HighlightTile(Vector2 tile)
    {
        if (_highlighter == null) InstantiateHighlighter();

        var highlighterPos = GetHighlighterCenter(tile);
        float hlen = Mathf.Floor(_length * .5f);

        highlighterPos.x -= _isVertical ? 0 : hlen * _tileSize.x;
        highlighterPos.z -= _isVertical ? hlen * _tileSize.y : 0;

        _highlighter.transform.localPosition = highlighterPos;
    }

    private void InstantiateHighlighter()
    {
        if (_highlighter) Destroy(_highlighter);
        if (_length <= 0) return;

        _highlighter = new GameObject("Highlighter");
        _highlighter.transform.parent = _highlightLayer;
        _highlighter.transform.localScale = new Vector3(1, 1, 1);
        _highlighter.transform.localPosition = new Vector3(0, 0, 0);

        if (RegularHighlight)
        {
            for (int i = 0; i < _length; i++)
            {
                var instance = Instantiate(RegularHighlight, new Vector3(), Quaternion.identity, _highlighter.transform);
                instance.transform.localPosition = _isVertical ? new Vector3(0, 0, i * _tileSize.y) : new Vector3(i * _tileSize.x, 0, 0);
            }
        }
        else
            Debug.LogError("Highlight object is not provided");
    }

    private Vector3 GetHighlighterCenter(Vector2 tile)
    {
        float hlen = Mathf.Floor(_length * .5f);
        float odd = _length % 2 == 0 ? 0 : 1;

        // Apply restrictions to row & col - prevents object going outside grid boundaries
        float posX = _isVertical ? tile.x : Mathf.Min(Mathf.Max(tile.x, hlen), _controller.Model.Width - hlen - odd);
        float posY = _isVertical ? Mathf.Min(Mathf.Max(tile.y, hlen), _controller.Model.Height - hlen - odd) : tile.y;

        posX *= _tileSize.x;
        posY *= _tileSize.y;

        return new Vector3(posX, 0, posY);
    }
}
