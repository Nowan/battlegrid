using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIBlockGenerator : MonoBehaviour {

    public int segmentsNumber = 1;
    public Texture2D crateTexture;
    public Texture2D slotTexture;

    private RectTransform _rectTransform;

    private GameObject _crateObj;
    private RawImage _crateImgCmpt;

    private GameObject _slotObj;
    private RawImage _slotImgCmpt;

    private int _spriteTextureResolution = 64;

    private void Start()
    {
        if (_crateObj == null) AssignReferences();
        Invalidate();
    }

    private void OnValidate()
    {
        if (_crateObj == null) AssignReferences();
        Invalidate();
    }

    private void AssignReferences()
    {
        _crateObj = transform.Find("Crate").gameObject;
        _crateImgCmpt = _crateObj.GetComponent<RawImage>();

        _slotObj = transform.Find("Slot").gameObject;
        _slotImgCmpt = _slotObj.GetComponent<RawImage>();

        _rectTransform = GetComponent<RectTransform>();
    }

    private void Invalidate()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.rect.height * segmentsNumber, 0);

        if (slotTexture)
        {
            _slotImgCmpt.texture = slotTexture;
            _slotImgCmpt.uvRect = new Rect(0, 0, segmentsNumber, 1);
        }
        if (crateTexture)
        {
            _crateImgCmpt.texture = crateTexture;
            _crateImgCmpt.uvRect = new Rect(0, 0, segmentsNumber, 1);
        }
    }
}
