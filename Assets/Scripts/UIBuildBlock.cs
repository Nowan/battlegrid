using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBuildBlock : MonoBehaviour
{
    public int segmentsNumber = 1;

    private GameObject _crateObj;

    private void Start()
    {
        _crateObj = transform.Find("AspectConstrainer/Crate").gameObject;
        CrateDrag dragger = _crateObj.AddComponent<CrateDrag>();
        dragger.parentBlock = this;
    }

    public void SetCratePosition(Vector2 position)
    {
        _crateObj.transform.position = position;
    }

    public void ReturnCrateToSlot()
    {
        _crateObj.transform.localPosition = new Vector3(0, 0, 0);
    }

    public class CrateDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public UIBuildBlock parentBlock;

        public void OnBeginDrag(PointerEventData eventData)
        {
            GameController.Instance.OnCrateDragStart(parentBlock, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            GameController.Instance.OnCrateDragMove(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GameController.Instance.OnCrateDragEnd(eventData);
        }
    }
}
