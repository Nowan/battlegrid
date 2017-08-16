using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
public class GridModel
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Dictionary<string, LayerModel> Layers { get; private set; }
    public BuildingsController Buildings;

    public GridModel(string jsonData)
    {
        var jObject = JSON.Parse(jsonData);
        Width = jObject["width"].AsInt;
        Height = jObject["height"].AsInt;

        var layersArray = jObject["layers"].AsArray;
        Layers = new Dictionary<string, LayerModel>(layersArray.Count);
        foreach(JSONNode node in layersArray) Layers[node["name"]] = new LayerModel(node);

        Buildings = new BuildingsController(Layers["buildings"]);
    }

    public class LayerModel
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int[,] Tiles { get; private set; }

        public LayerModel(JSONNode layerData)
        {
            Width = layerData["width"].AsInt;
            Height = layerData["height"].AsInt;

            Tiles = new int[Height, Width];
            var tilesArray = layerData["data"].AsArray;
            for (int i = 0; i < tilesArray.Count; i++)
            {
                int row = Mathf.FloorToInt(i / Width);
                int col = i % Width;
                Tiles[row, col] = tilesArray[i].AsInt - 1;
            }
        }
    }

    public class BuildingsController
    {
        private LayerModel _layerModel;

        public BuildingsController(LayerModel buildingsLayer)
        {
            _layerModel = buildingsLayer;
        }

        public void Insert(Vector2 tile, int length, bool isVertical)
        {
            for (int i = 0; i < length; i++)
            {
                int row = (int)tile.y + (isVertical ? i : 0);
                int col = (int)tile.x + (isVertical ? 0 : i);
                _layerModel.Tiles[row, col] = 1;
            }
            Debug.Log(_layerModel.Tiles);
        }

        public TileInfo IsTileAvailable(Vector2 tile)
        {

            return null;
        }
    }

    public class TileInfo
    {

    }
}
