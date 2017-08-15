using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    public GameObject playerGrid;
    public GameObject enemyGrid;

    private GridGenerator _playerBattleGrid;
    private GridGenerator _enemyBattleGrid;

    private Ray _ray;
    private RaycastHit _hitInfo;
    private GameObject _objectUnderRay;

    private UIBuildBlock _draggedBlock;

    private void Awake()
    {
        Instance = this;
        _playerBattleGrid = playerGrid.GetComponent<GridGenerator>();
        _enemyBattleGrid = enemyGrid.GetComponent<GridGenerator>();
    }

    void Update () {
        if (CastRay())
        {
            if (_objectUnderRay.GetComponent<GridGenerator>())
            {
                var battleGrid = _objectUnderRay.GetComponent<GridGenerator>();
                var localHitPoint = _hitInfo.transform.worldToLocalMatrix.MultiplyPoint3x4(_hitInfo.point);
                var targetTile = battleGrid.GetTileAt(localHitPoint);
                battleGrid.HighlightTile(targetTile);
            }
        }
    }

    public void OnCrateDragStart(UIBuildBlock sender, PointerEventData eventData)
    {
        _draggedBlock = sender;
        _playerBattleGrid.SetHighlighter(_draggedBlock.segmentsNumber, false);
    }

    public void OnCrateDragMove(PointerEventData eventData)
    {
        if (_objectUnderRay && _objectUnderRay.transform.parent.name.Equals("PlayerGrid") &&
                                           _objectUnderRay.GetComponent<GridGenerator>())
        {
            var battleGrid = _objectUnderRay.GetComponent<GridGenerator>();
            var localHitPoint = _hitInfo.transform.worldToLocalMatrix.MultiplyPoint3x4(_hitInfo.point);
            var targetTile = battleGrid.GetTileAt(localHitPoint);

            var highligherCenter = battleGrid.GetHighlighterCenter(targetTile);
            highligherCenter.x += ((_draggedBlock.segmentsNumber % 2 == 0 ? -1 : 0) + battleGrid.tileSize.x) * .5f;
            highligherCenter.y = 2;

            var worldTileCenterPoint = _hitInfo.transform.localToWorldMatrix.MultiplyPoint3x4(highligherCenter);
            var screenPosAboveTile = Camera.main.WorldToScreenPoint(worldTileCenterPoint);
            _draggedBlock.SetCratePosition(screenPosAboveTile);
        }
        else
        {
            _draggedBlock.SetCratePosition(eventData.position);
        }

    }

    public void OnCrateDragEnd(PointerEventData eventData)
    {
        if (_objectUnderRay && _objectUnderRay.transform.parent.name.Equals("PlayerGrid") &&
                                           _objectUnderRay.GetComponent<GridGenerator>())
        {
            var battleGrid = _objectUnderRay.GetComponent<GridGenerator>();
        }
        else
        {
            _draggedBlock.ReturnCrateToSlot();
        }

        _playerBattleGrid.SetHighlighter(1, false);
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
