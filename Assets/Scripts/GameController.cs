using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private Ray _ray;
    private RaycastHit _hitInfo;
    
	void Update () {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(_ray, out _hitInfo))
        {
            var battleGrid = _hitInfo.transform.gameObject.GetComponent<GridGenerator>();
            if (battleGrid)
            {
                var localHitPoint = _hitInfo.transform.worldToLocalMatrix.MultiplyPoint3x4(_hitInfo.point);
                var targetTile = battleGrid.GetTileAt(localHitPoint);
            }
        }
	}
}
