using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    public GameObject playerGrid;
    public GameObject enemyGrid;

    private GridController _playerBattleGrid;
    private GridController _enemyBattleGrid;

    private Ray _ray;
    private RaycastHit _hitInfo;
    private GameObject _objectUnderRay;

    private UIBuildBlock _draggedBlock;

    private void Awake()
    {
        Instance = this;
        _playerBattleGrid = playerGrid.GetComponent<GridController>();
        //_enemyBattleGrid = enemyGrid.GetComponent<GridController>();
    }

    void Update () {
        if (CastRay())
        {
            if (_objectUnderRay.GetComponent<GridController>())
            {
                var highlighter = _objectUnderRay.GetComponent<GridHighlighter>();
                var localHitPoint = _hitInfo.transform.worldToLocalMatrix.MultiplyPoint3x4(_hitInfo.point);
                highlighter.HighlightTileAtPoint(localHitPoint);
            }
        }
    }

    public void OnCrateDragStart(UIBuildBlock sender, PointerEventData eventData)
    {
        Debug.Log("DRAG START");
        _draggedBlock = sender;
        _playerBattleGrid.Highlighter.SetShape(_draggedBlock.segmentsNumber, false);
    }
    
    public void OnCrateDragMove(PointerEventData eventData)
    {
        if (_objectUnderRay && _objectUnderRay.Equals(playerGrid))
        {
            /*var battleGrid = _objectUnderRay.GetComponent<GridController>();
            var localHitPoint = _hitInfo.transform.worldToLocalMatrix.MultiplyPoint3x4(_hitInfo.point);
            var targetTile = battleGrid.GetTileAt(localHitPoint);

            var highligherCenter = battleGrid.GetHighlighterCenter(targetTile);
            //highligherCenter.x += ((_draggedBlock.segmentsNumber % 2 == 0 ? -1 : 0) + battleGrid.tileSize.x) * .5f;
            highligherCenter.y = 2;

            var worldTileCenterPoint = _hitInfo.transform.localToWorldMatrix.MultiplyPoint3x4(highligherCenter);
            var screenPosAboveTile = Camera.main.WorldToScreenPoint(worldTileCenterPoint);
            _draggedBlock.SetCratePosition(screenPosAboveTile);*/
        }
        else
        {
            _draggedBlock.SetCratePosition(eventData.position);
        }

    }

    public void OnCrateDragEnd(PointerEventData eventData)
    {
        Debug.Log("DRAG END");
        if (_objectUnderRay && _objectUnderRay.Equals(playerGrid))
        {
            //var battleGrid = _objectUnderRay.GetComponent<GridController>();
            //var localHitPoint = _hitInfo.transform.worldToLocalMatrix.MultiplyPoint3x4(_hitInfo.point);
            //var targetTile = battleGrid.GetTileAt(localHitPoint);
            //battleGrid.Model.Buildings.Insert(targetTile, _draggedBlock.segmentsNumber, false);
        }
        else
        {
            _draggedBlock.ReturnCrateToSlot();
        }

        //_playerBattleGrid.SetHighlighter(1, false);
        _draggedBlock = null;
    }

    private bool CastRay()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(_ray, out _hitInfo))
        {
            _objectUnderRay = _hitInfo.transform.gameObject;
            return true;
        }
        else
        {
            _objectUnderRay = null;
            return false;
        }
    }
}
